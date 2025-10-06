using System;
using System.Threading;
using System.Threading.Tasks;
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
            _ = PollForActionsLoop(_cts.Token);
        }

        private void OnDisable()
        {
            _cts.Cancel();
        }

        private async Task PollForActionsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var action = await _service.PollAction();
                    Debug.Log(action.action);

                    await Task.Delay((int)settings.pollingInterval * 1000, token);
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
    }
}