using System;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class ActionConsumer : MonoBehaviour
    {
        private static ActionConsumer Instance { get; set; }
        
        [Header("Dependencies")]
        [SerializeField] private ActionQueueManager queueManager;

        [Header("Settings")] 
        [Tooltip("Delay between processing actions")]
        [SerializeField] private float processInterval = 0.1f;

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
            if (queueManager == null)
            {
                Debug.LogError("[ActionConsumer] QueueManager is not set in the inspector!");
            }

            // Start the infinite loop
            ConsumeLoop().Forget();
        }

        private async UniTaskVoid ConsumeLoop()
        {
            var token = this.GetCancellationTokenOnDestroy();

            while (!token.IsCancellationRequested)
            {
                // Attempt to pull the next action from the queue
                if (queueManager.ActionQueue.TryDequeue(out var queuedAction))
                {
                    await HandleAction(queuedAction);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(processInterval), cancellationToken: token);
            }
        }

        private async UniTask HandleAction(QueuedAction action)
        {
            try
            {
                var actorKey = action.agentKey;
                var payload = action.responsePayload;

                Debug.Log($"[Arbiter] Executing: {actorKey} -> {payload.actionKey}");

                await ActionHandler.ProcessAction(payload);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Arbiter] Execution failed for {action.agentKey}: {ex.Message}");
            }
        }
    }
}