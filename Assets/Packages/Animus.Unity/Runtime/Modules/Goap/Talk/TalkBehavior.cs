using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Talk
{
    public class TalkBehavior : MonoBehaviour
    {
        public string text;
        public AnimusActor targetActor;
        public bool hasFinishedTalking = true;
    }
}