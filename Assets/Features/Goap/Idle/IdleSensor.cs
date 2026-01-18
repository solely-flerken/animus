using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Features.Goap.Idle
{
    public class IdleSensor : LocalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }
        
        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            return false;
        }
    }
}