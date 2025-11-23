using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Goap.Wander
{
    public class WanderTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var position = GetRandomPosition(agent);
            return new PositionTarget(position);
        }

        private static Vector3 GetRandomPosition(IActionReceiver agent)
        {
            for (var i = 0; i < 5; i++)
            {
                var random = Random.insideUnitCircle * 5;
                var position = agent.Transform.position + new Vector3(random.x, 0, random.y);

                if (NavMesh.SamplePosition(position, out var hit, 1, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return agent.Transform.position;
        }
    }
}