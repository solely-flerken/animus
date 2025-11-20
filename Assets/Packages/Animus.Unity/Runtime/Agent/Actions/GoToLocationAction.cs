using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Environment;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToLocationAction", menuName = "Animus/NPC/Action/GoToLocationAction")]
    public class GoToLocationAction : NpcAction
    {
        [HideInInspector] public string locationKey;

        public override void OnExecute(AnimusAgent animusAgent)
        {
            var poi = AnimusEntityRegistry.Instance.FindByGameKey<AnimusLocation>(locationKey);
            if (poi == null)
            {
                Debug.Log("There is no Location named: " + locationKey);
            }

            Debug.Log($"Action: {actionKey} - Going to {poi.name}");
            animusAgent.GoToPoi(poi);
        }
    }
}