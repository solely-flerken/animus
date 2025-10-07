using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public class ActionQueueMonitor : MonoBehaviour
    {
        [SerializeField] private AnimusSettings settings;

        public static ActionQueueMonitor Instance { get; private set; }

        private IAnimusService _service;
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
            _service = AnimusServiceManager.Service;

            _cts = new CancellationTokenSource();
            PollForActionsLoop().Forget();
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }

        private async UniTaskVoid PollForActionsLoop()
        {
            var token = this.GetCancellationTokenOnDestroy();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var action = await _service.PollAction();
                    Debug.Log(action.action);
                    AnimusEventSystem.InvokeActionReceived(action);

                    await UniTask.Delay(TimeSpan.FromSeconds(settings.pollingInterval), cancellationToken: token);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}