using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Talk
{
    public class HasTalkedSensor : LocalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var behavior = references.GetCachedComponent<TalkBehavior>();

            if (!behavior)
            {
                return false;
            }

            return behavior.hasFinishedTalking;
        }
    }
}