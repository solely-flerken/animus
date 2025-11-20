using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.AI;
using Packages.Animus.Unity.Runtime.Core.AI.Rules;
using Packages.Animus.Unity.Runtime.Core.AI.Service;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Core.Utils.Save;
using Packages.Animus.Unity.Runtime.Environment;
using Packages.Animus.Unity.Runtime.Player;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
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
            // Loop().Forget();
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


            if (dialogEvent.EventTarget.Count != 1 && dialogEvent.EventTarget.First() is AnimusAgent or AnimusPlayer)
            {
                Debug.Log($"{nameof(DialogEvent)} can only have one event target which must be of type {nameof(AnimusAgent)} or  {nameof(AnimusPlayer)}");
            }

            var sourceAgent = dialogEvent.EventSource;
            var targetAgent = dialogEvent.EventTarget.First() as AnimusAgent;

            if (sourceAgent == null || targetAgent == null)
            {
                Debug.Log("HandleDialogEvent: Source or Target Agent is null");
                return;
            }

            // Add the input to the conversation history
            targetAgent.conversationHistory.AddLine(new List<string> { sourceAgent.gameKey, targetAgent.gameKey }, sourceAgent.gameKey, dialogEvent.Text);

            var prompt = new PromptBuilder()
                .SetAgent(targetAgent)
                .WithAvailableActions(targetAgent.actionCollection.actions)
                .WithRecentEvents(targetAgent.eventHistory.Events)
                .WithActionHistory(targetAgent.actionHistory.GetHistory())
                .WithConversationHistory(targetAgent.conversationHistory.GetHistoryFor(new List<string> { sourceAgent.gameKey, targetAgent.gameKey }, 50))
                .WithEnvironment(EnvironmentScanner.CreateSnapshot(targetAgent))
                // .WithRelevantMemories(new List<string>())
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
                    // TODO
                    foreach (var agent in AnimusEntityRegistry.Instance.GetAll<AnimusAgent>())
                    {
                        // TODO: If interacts with player or another NPC we retrieve that conversation history:
                        // AnimusEntity interactingEntity = null;

                        var prompt = new PromptBuilder()
                            .SetAgent(agent)
                            .WithAvailableActions(agent.actionCollection.actions)
                            .WithRecentEvents(agent.eventHistory.Events)
                            .WithConversationHistory(new List<DialogLine>())
                            .WithEnvironment(EnvironmentScanner.CreateSnapshot(agent))
                            .WithRelevantMemories(new List<string>())
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

            if (response != null)
            {
                ActionHandler.ProcessAction(response.Payload);
            }
            else
            {
                Debug.LogWarning("Animus-Backend did not return a valid response.");
            }
        }
    }
}