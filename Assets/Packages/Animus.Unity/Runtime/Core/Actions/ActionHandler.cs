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
    
            if (targetAgent.agentActionSystem == null)
            {
                Debug.LogError($"Command failed: Agent '{targetAgent.gameKey}' has no AgentActionSystem component.");
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

            var outcome = await targetAgent.agentActionSystem.ExecuteAction(actionPayload.goalKey, paramsDict);

            if (string.IsNullOrWhiteSpace(outcome))
            {
                return;
            }
            
            targetAgent.memories.Add(outcome);
        }
    }
}