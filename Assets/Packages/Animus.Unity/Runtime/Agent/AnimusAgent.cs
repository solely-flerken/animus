using System;
using Packages.Animus.Unity.Runtime.Core;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [Serializable]
    public class AnimusAgent : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Agent;
        [TextArea(3, 10)] public string backstory;
    }
}