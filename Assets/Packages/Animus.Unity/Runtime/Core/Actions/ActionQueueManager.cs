using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;
using Packages.Animus.Unity.Runtime.Integrations.Prompting.Constants;
using Packages.Animus.Unity.Runtime.Integrations.Service;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class ActionQueueManager : MonoBehaviour
    {
        private static ActionQueueManager Instance { get; set; }
        
        [Header("Debug Info (Read Only)")] 
        [SerializeField] private List<string> thinkingAgentsDebug = new();
        [SerializeField] private List<ActionPayload> queuedActionsDebug = new();

        // Tracks agents currently "Thinking" (waiting for LLM's Response)
        private readonly Dictionary<string, PendingRequest> _activeRequests = new();

        public ConcurrentQueue<QueuedAction> ActionQueue { get; } = new();
        
        private void Awake()
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
            Loop().Forget();
        }

        #region Debug/Testing

        private void Update()
        {
#if UNITY_EDITOR
            SyncDebugViews();
#endif
        }

        private void SyncDebugViews()
        {
            // Visualize Active Requests (Thinking Agents)
            thinkingAgentsDebug.Clear();
            foreach (var kvp in _activeRequests)
            {
                var req = kvp.Value;
                var duration = DateTime.UtcNow - req.timestamp;
                var detail = $"[{req.agentKey}] request age: ({duration.TotalSeconds:F2}s) @ {req.timestamp.ToLocalTime():HH:mm:ss}";
                thinkingAgentsDebug.Add(detail);
            }

            // Visualize Action Queue
            queuedActionsDebug.Clear();
            var snapshot = ActionQueue.ToArray();
            foreach (var action in snapshot)
            {
                queuedActionsDebug.Add(action.responsePayload);
            }
        }
        
        public void DebugCancelRandomAgent()
        {
            if (_activeRequests.Count == 0)
            {
                Debug.LogWarning("No agents are currently thinking. Cannot cancel.");
                return;
            }

            // Pick a random key from the dictionary
            var keys = new List<string>(_activeRequests.Keys);
            var randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];

            Debug.Log($"Kill request for: {randomKey}");
        
            CancelAgentRequest(randomKey);
        }
        
        #endregion
      
        private async UniTaskVoid Loop()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            var token = this.GetCancellationTokenOnDestroy();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var agents = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>();

                    foreach (var agent in agents)
                    {
                        // Can Agent perform actions?
                        if (agent.actionRunner == null) continue;

                        // Is Agent already thinking?
                        if (_activeRequests.ContainsKey(agent.gameKey)) continue;

                        // Does Agent already have a finished action waiting in the queue?
                        if (HasPendingActionInQueue(agent.gameKey)) continue;

                        var prompt = new PromptBuilder()
                            .SetAgent(agent)
                            .WithAvailableActions(agent.actionRunner)
                            .WithConversationHistory(
                                AnimusAgent.SharedHistory.GetHistoryFor(new HashSet<string> { agent.gameKey }, 10))
                            .WithEnvironment(EnvironmentScanner.CreateSnapshot(agent))
                            .WithRelevantMemories(agent.memories)
                            .WithRules(PredefinedRulesets.CommonAgent)
                            .WithTaskInstruction("");

                        SendRequestAsync(agent.gameKey, prompt.GetContext()).Forget();
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(AnimusSettings.Instance.pollingInterval),
                        cancellationToken: token);
                }
                catch (OperationCanceledException)
                {
                    // This is normal behavior when the object is destroyed or play mode ends.
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"BehaviorLoop Loop Error: {ex.Message}");
                    break;
                }
            }
        }

        private async UniTaskVoid SendRequestAsync(string agentKey, PromptContext context)
        {
            var cts = new CancellationTokenSource();

            var request = new PendingRequest
            {
                agentKey = agentKey,
                timestamp = DateTime.UtcNow,
                contextSnapshot = context,
                cts = cts
            };

            // Register Request
            _activeRequests[agentKey] = request;

            try
            {
                LocalJsonSaveSystem.Save(context);

                var response = await AnimusService.Chat(context, cts.Token);

                if (response != null && response.Payload != null)
                {
                    if (string.IsNullOrEmpty(response.Payload.agentKey))
                    {
                        response.Payload.agentKey = context.AgentKey;
                    }

                    // Enqueue Result
                    var queuedAction = new QueuedAction
                    {
                        agentKey = agentKey,
                        requestTimestamp = request.timestamp,
                        originalContext = context,
                        responsePayload = response.Payload
                    };

                    ActionQueue.Enqueue(queuedAction);
                }
                else
                {
                    Debug.LogWarning($"[{context.AgentKey}] Animus-Backend returned no response.");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"[{agentKey}] Thinking process cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{context.AgentKey}] Error processing event: {ex.Message}");
            }
            finally
            {
                // Cleanup active request
                if (_activeRequests.ContainsKey(agentKey) && _activeRequests[agentKey] == request)
                {
                    _activeRequests.Remove(agentKey);
                }

                cts.Dispose();
            }
        }

        #region Public API for Interaction

        /// <summary>
        /// Forces an agent to stop thinking.
        /// TODO: Maybe instantly send another request to the LLM
        /// </summary>
        public void CancelAgentRequest(string agentKey)
        {
            if (_activeRequests.TryGetValue(agentKey, out var request))
            {
                request.cts.Cancel();
                _activeRequests.Remove(agentKey);
            }
        }

        /// <summary>
        /// Helper to check if an agent has an action sitting in the queue waiting to be consumed.
        /// </summary>
        private bool HasPendingActionInQueue(string agentKey)
        {
            foreach (var action in ActionQueue)
            {
                if (action.agentKey == agentKey)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}