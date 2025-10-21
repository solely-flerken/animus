using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
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
                    var agent = AgentRegistry.Instance.allItems.First(x => x);
                    
                    var actionPayload = new ActionPayload<string>
                    {
                        gameKey = agent.agentEntity.gameKey,
                        actionKey = agent.actionCollection.GetRandomAction().actionKey,
                        details = ""
                    };

                    ActionHandler.ProcessAction(actionPayload);

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