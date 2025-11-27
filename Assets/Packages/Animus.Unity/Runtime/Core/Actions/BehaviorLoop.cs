using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.AI.Rules;
using Packages.Animus.Unity.Runtime.Core.AI.Service;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Integrations.AI;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class BehaviorLoop : MonoBehaviour
    {
        [SerializeField] private AnimusSettings settings;

        private static BehaviorLoop Instance { get; set; }

        private CancellationTokenSource _cts;

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

        private static void HandleDialogEvent(AnimusEvent animusEvent)
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
                    var agents = AnimusEntityRegistry.Instance.GetAll<AnimusAgent>();
                    
                    foreach (var agent in agents)
                    {
                        if (agent.actionRunner == null)
                        {
                            // Agent can't execute actions
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

                    await UniTask.Delay(TimeSpan.FromSeconds(settings.pollingInterval), cancellationToken: token);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private static async UniTaskVoid ProcessEventAsync(PromptContext context)
        {
            LocalJsonSaveSystem.Save(context);

            var response = await AnimusService.Chat(context);

            if (response != null && response.Payload != null)
            {
                // Ensure the payload has the agent key, if missing, fill it from context
                if (string.IsNullOrEmpty(response.Payload.agentKey))
                {
                    response.Payload.agentKey = context.AgentKey;
                }
                
                await ActionHandler.ProcessAction(response.Payload);
            }
            else
            {
                Debug.LogWarning("Animus-Backend returned no response.");
            }
        }
    }
}