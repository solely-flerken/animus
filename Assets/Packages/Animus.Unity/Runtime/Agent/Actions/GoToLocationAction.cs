using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Environment;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToLocationAction", menuName = "Animus/NPC/Action/GoToLocationAction")]
    public class GoToLocationAction : NpcAction
    {
        [HideInInspector] public string locationKey;

        protected override async UniTask<string> OnExecute(AnimusAgent animusAgent)
        {
            var poi = AnimusEntityRegistry.Instance.FindByGameKey<AnimusLocation>(locationKey);
            if (poi == null)
            {
                Debug.Log("There is no Location named: " + locationKey);
                return $"There is no Location named: '{locationKey}'";
            }

            Debug.Log($"Action: {actionKey} - Going to {poi.name}");
            var success = await animusAgent.GoToPoi(poi);
            return success ? $"Successfully arrived at '{locationKey}'" : $"Failed to travel to '{locationKey}'.";
        }
    }
}