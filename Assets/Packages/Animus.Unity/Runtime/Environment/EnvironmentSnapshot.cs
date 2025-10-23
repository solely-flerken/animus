using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public class EnvironmentSnapshot
    {
        public List<object> VisibleObjects { get; set; }
        public List<object> PointsOfInterest { get; set; }
    }
}