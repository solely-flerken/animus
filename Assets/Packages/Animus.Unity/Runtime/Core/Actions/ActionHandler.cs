using System;
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
        // TODO: Rename this since it not only handles the action put the whole response payload.
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

            if (targetAgent.agentActionSystem.IsDuplicateRequest(actionPayload.goalKey, paramsDict))
            {
                // Debug.Log($"[Action] {targetAgent.gameKey} skipping duplicate action: {actionPayload.goalKey}");
                return;
            }
            
            // Set new motivation
            targetAgent.currentMotivation = actionPayload.motivation; // TODO: Should we consider the new motivation even if it's a duplicate action? (Yes?: Move before duplicate check)
            
            var paramsStr = $"[{string.Join(", ", paramsDict.Values.Select(v => v?.ToString().Length > 20 ? v.ToString()[..20] + "..." : v?.ToString()))}]";
            Debug.Log($"[Action] {targetAgent.gameKey} executing: {actionPayload.goalKey} -> {paramsStr}");

            try
            {
                targetAgent.actionStatus.StartAction($"{actionPayload.goalKey}", paramsStr);
             
                var outcome = await targetAgent.agentActionSystem.ExecuteAction(actionPayload.goalKey, paramsDict);

                targetAgent.actionStatus.Success();
                
                if (string.IsNullOrWhiteSpace(outcome))
                {
                    return;
                }

                targetAgent.memories.Add(outcome);
            }
            catch (OperationCanceledException)
            {
                // This is fine. The Agent simply performs a new action while the old one wasn't finished.
                // TODO: Capture cancellation in context with reason?
                targetAgent.actionStatus.Cancel();
                Debug.Log($"[Action] {targetAgent.gameKey} cancelled: {actionPayload.goalKey} -> {paramsStr}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ActionHandler] Error: {e}");
            }
        }
    }
}