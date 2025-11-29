using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;
using Packages.Animus.Unity.Runtime.Integrations.Prompting.Constants;
using Packages.Animus.Unity.Runtime.Integrations.Service;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [Obsolete("Use ActionConsumer/ActionQueueManager instead")]
    public class BehaviorLoop : MonoBehaviour
    {
        private static BehaviorLoop Instance { get; set; }

        private CancellationTokenSource _cts;
        
        private readonly HashSet<string> _thinkingAgents = new();
        
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AnimusEventSystem.OnDialogEvent += HandleDialogEvent;

            _cts = new CancellationTokenSource();
            Loop().Forget();
        }

        private void OnDisable()
        {
            AnimusEventSystem.OnDialogEvent -= HandleDialogEvent;

            _cts.Cancel();
        }

        private void HandleDialogEvent(AnimusEvent animusEvent)
        {
            if (animusEvent is not DialogEvent dialogEvent || animusEvent.EventType != AnimusEventType.Dialog)
            {
                Debug.LogError($"HandleDialogEvent: Expected {nameof(DialogEvent)} but got {animusEvent.GetType().Name}");
                return;
            }

            var sourceAgent = dialogEvent.EventSource;
            var targetAgent = dialogEvent.EventTarget.FirstOrDefault() as AnimusAgent;
            
            if (sourceAgent == null || targetAgent == null)
            {
                Debug.LogWarning("HandleDialogEvent: Source or Target Agent invalid.");
                return;
            }

            // Add to conversation history
            AnimusAgent.SharedHistory.AddLine(new List<string> { sourceAgent.gameKey, targetAgent.gameKey }, sourceAgent.gameKey, dialogEvent.Text);
            
            var prompt = new PromptBuilder()
                .SetAgent(targetAgent)
                .WithAvailableActions(targetAgent.actionRunner)
                // .WithRecentEvents(targetAgent.eventHistory.Events)
                // .WithActionHistory(targetAgent.actionHistory.GetHistory())
                .WithConversationHistory(AnimusAgent.SharedHistory.GetHistoryFor(new HashSet<string> { sourceAgent.gameKey, targetAgent.gameKey }, 50))
                .WithEnvironment(EnvironmentScanner.CreateSnapshot(targetAgent))
                .WithRelevantMemories(targetAgent.memories)
                .WithRules(PredefinedRulesets.CommonAgent)
                .WithTaskInstruction("");

            ProcessEventAsync(prompt.GetContext()).Forget();
        }

        private async UniTaskVoid Loop()
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));

            var token = this.GetCancellationTokenOnDestroy();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var agents = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>();
                    
                    foreach (var agent in agents)
                    {
                        if (agent.actionRunner == null)
                        {
                            // Agent can't execute actions
                            continue;
                        }
                        
                        // Is this agent already waiting for a response?
                        if (!_thinkingAgents.Add(agent.gameKey))
                        {
                            continue;
                        }

                        var prompt = new PromptBuilder()
                            .SetAgent(agent)
                            .WithAvailableActions(agent.actionRunner)
                            // .WithRecentEvents(agent.eventHistory.Events)
                            // .WithActionHistory(agent.actionHistory.GetHistory())
                            .WithConversationHistory(AnimusAgent.SharedHistory.GetHistoryFor(new HashSet<string> { agent.gameKey }, 10))
                            .WithEnvironment(EnvironmentScanner.CreateSnapshot(agent))
                            .WithRelevantMemories(agent.memories)
                            .WithRules(PredefinedRulesets.CommonAgent)
                            .WithTaskInstruction("");

                        ProcessEventAsync(prompt.GetContext()).Forget();
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(AnimusSettings.Instance.pollingInterval), cancellationToken: token);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"BehaviorLoop Loop Error: {ex.Message}");
                    break;
                }
            }
        }

        private async UniTaskVoid ProcessEventAsync(PromptContext context)
        {
            try
            {
                LocalJsonSaveSystem.Save(context);

                var response = await AnimusService.Chat(context);

                if (response != null && response.Payload != null)
                {
                    if (string.IsNullOrEmpty(response.Payload.agentKey))
                    {
                        response.Payload.agentKey = context.AgentKey;
                    }

                    // Execute Action
                    await ActionHandler.ProcessAction(response.Payload);
                }
                else
                {
                    Debug.LogWarning($"[{context.AgentKey}] Animus-Backend returned no response.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{context.AgentKey}] Error processing event: {ex.Message}");
            }
            finally
            {
                _thinkingAgents.Remove(context.AgentKey);
            }
        }
    }
}