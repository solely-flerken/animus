using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [Serializable]
    public class AgentDetails
    {
        [TextArea(3, 10)] public string backstory;
    }
}