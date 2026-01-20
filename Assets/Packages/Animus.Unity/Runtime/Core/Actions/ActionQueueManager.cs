using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;
using Packages.Animus.Unity.Runtime.Integrations.Prompting.Constants;
using Packages.Animus.Unity.Runtime.Integrations.Service;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class ActionQueueManager : MonoBehaviour
    {
        public static ActionQueueManager Instance { get; set; }
        
        [Header("Debug Info (Read Only)")] 
        [SerializeField] private List<string> thinkingAgentsDebug = new();
        [SerializeField] private List<ActionPayload> queuedActionsDebug = new();
        [SerializeField] private List<string> blockedAgentsDebug = new();
        
        // Tracks agents currently "Thinking" (waiting for LLM's Response)
        private readonly Dictionary<string, PendingRequest> _activeRequests = new();

        // Tracks agents whose requests are blocked (e.g., during player interaction)
        // Key: Who is blocked, Value: Reason for the block (e.g., "Conversation")
        private readonly Dictionary<string, HashSet<string>> _blockingTokens = new();
        
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
            if (Time.frameCount % 30 == 0)
            {
                SyncDebugViews();
            }
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
            
            // Visualize Blocked Agents
            blockedAgentsDebug.Clear();
            foreach (var kvp in _blockingTokens)
            {
                var blockingTokens = kvp.Value;
                if (blockingTokens.Count != 0)
                {
                    blockedAgentsDebug.Add($"[{kvp.Key}] blocked through {kvp.Value.First()}");
                }
                else
                {
                    Debug.Log("[Blocked Agents] Agent blocked with no blocking token. IMPOSSIBLE]");
                }
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
                    var activeAnchors = new HashSet<ConversationAnchor>(ConversationAnchor.ConversationAnchors.Values);
                    foreach (var anchor in activeAnchors)
                    {
                        anchor.CheckStalemate();
                    }
                    
                    var agents = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>();
                    foreach (var agent in agents)
                    {
                        TryAgentThink(agent);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(AnimusSettings.Instance.pollingInterval), cancellationToken: token);
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

        private void TryAgentThink(AnimusAgent agent)
        {
            // Can Agent perform actions?
            if (agent.agentActionSystem == null) return;

            // Is Agent blocked from making requests?
            if (IsAgentBlocked(agent.gameKey)) return;
                        
            // Is Agent already thinking?
            if (_activeRequests.ContainsKey(agent.gameKey)) return;

            // Does Agent already have a finished action waiting in the queue?
            if (HasPendingActionInQueue(agent.gameKey)) return;
                        
            var prompt = new PromptBuilder(agent)
                .WithIdentity()
                .WithAvailableActions()
                .WithCurrentState()
                .WithSchedule()
                .WithMotivation()
                .WithLastAction()
                .WithRelevantMemories()
                .WithConversationHistory(AnimusAgent.SharedHistory.GetHistoryFor(new HashSet<string> { agent.gameKey }, 10))
                .WithEnvironment(EnvironmentScanner.CreateSnapshot(agent))
                .WithRules(PredefinedRulesets.CommonAgent);

            SendRequestAsync(agent.gameKey, prompt.GetContext()).Forget();
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
                var fileName = $"save_{agentKey}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.json";
                LocalJsonSaveSystem.SaveAsync(context, fileName).Forget();

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
                // Debug.Log($"[{agentKey}] Thinking process cancelled.");
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
        /// Adds a specific reason to block the agent.
        /// </summary>
        public void AddBlockToken(string agentKey, string reasonToken)
        {
            if(IsPlayer(agentKey)) return; // Never block a player (wouldn't do actually anything, only for clarity)
            
            if (!_blockingTokens.ContainsKey(agentKey))
            {
                _blockingTokens[agentKey] = new HashSet<string>();
            }

            _blockingTokens[agentKey].Add(reasonToken);
            // Debug.Log($"[{agentKey}] Added Block: {reasonToken}. Total Blocks: {_blockingTokens[agentKey].Count}");
        }

        /// <summary>
        /// Removes a specific block reason. 
        /// The agent is only free if ALL tokens are removed.
        /// </summary>
        public void RemoveBlockToken(string agentKey, string reasonToken)
        {
            if (_blockingTokens.TryGetValue(agentKey, out var tokens))
            {
                tokens.Remove(reasonToken);
                
                if (tokens.Count == 0)
                {
                    _blockingTokens.Remove(agentKey);
                    TryAgentThink(AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(agentKey));
                    // Debug.Log($"[{agentKey}] All blocks removed. Agent is freed.");
                }
            }
        }
        
        public bool IsAgentBlocked(string agentKey)
        {
            return _blockingTokens.ContainsKey(agentKey);
        }
        
        /// <summary>
        /// Forces an agent to stop thinking.
        /// TODO: Maybe instantly send another request to the LLM
        /// </summary>
        public void CancelAgentRequest(string agentKey)
        {
            if (_activeRequests.TryGetValue(agentKey, out var request))
            {
                Debug.Log($"[Outdated context] Canceling requests for: {agentKey}");
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
        
        private static bool IsPlayer(string agentKey)
        {
            var entity = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusEntity>(agentKey);
            return entity is AnimusPlayer;
        }
    }
}