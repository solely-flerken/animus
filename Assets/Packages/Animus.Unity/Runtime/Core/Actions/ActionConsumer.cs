using System;
using Cysharp.Threading.Tasks;
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
                    var payload = queuedAction.responsePayload;
                    
                    ActionHandler.ProcessAction(payload).Forget();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(processInterval), cancellationToken: token);
            }
        }
    }
}