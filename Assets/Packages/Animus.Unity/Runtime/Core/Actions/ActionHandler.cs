using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public static class ActionHandler
    {
        public static async UniTask ProcessAction(ActionPayload actionPayload)
        {
            var targetAgent = AnimusGameManager.EntityRegistry.GetAll<AnimusAgent>().FirstOrDefault(a => a.gameKey == actionPayload.agentKey);
            if (targetAgent == null)
            {
                Debug.LogError($"Command failed: Agent '{actionPayload.agentKey}' not found in the registry.");
                return;
            }
    
            if (targetAgent.actionRunner == null)
            {
                Debug.LogError($"Command failed: Agent '{targetAgent.gameKey}' has no AgentActionRunner component.");
                return;
            }
    
            var paramsDict = new Dictionary<string, object>();
            
            if (actionPayload.parameters != null)
            {
                foreach (var param in actionPayload.parameters)
                {
                    paramsDict[param.name] = param.value; 
                }
            }

            Debug.Log($"Agent '{targetAgent.gameKey}' executing: {actionPayload.actionKey}");

            var outcome = await targetAgent.actionRunner.Execute(actionPayload.actionKey, paramsDict);
            
            Debug.Log($"Result: {outcome}");
        }
    }
}