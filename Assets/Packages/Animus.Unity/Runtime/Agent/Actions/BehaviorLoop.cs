using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.AI;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public class BehaviorLoop : MonoBehaviour
    {
        [SerializeField] private AnimusSettings settings;

        public static BehaviorLoop Instance { get; private set; }

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
            _cts = new CancellationTokenSource();
            Loop().Forget();
        }

        private void OnDisable()
        {
            _cts.Cancel();
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
                        AnimusEntity interactingEntity = null;
                        
                        var prompt = new PromptBuilder()
                            .WithPersona(agent)
                            .WithAvailableActions(agent.actionCollection.actions)
                            .WithRecentEvents(new List<AnimusEvent>())
                            .WithConversationHistory(agent.conversationHistory.GetHistoryFor(interactingEntity.gameKey,50))
                            .Build();

                        var actionPayload = new ActionPayload<string>
                        {
                            gameKey = agent.gameKey,
                            actionKey = agent.actionCollection.GetRandomAction().actionKey,
                            details = ""
                        };

                        ActionHandler.ProcessAction(actionPayload);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(settings.pollingInterval), cancellationToken: token);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void Cycle()
        {
            // TODO:
            // Get world state
            // Get agent state
            // Get memories
        }
    }
}