using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Goap.Talk
{
    public class TalkBehavior : MonoBehaviour
    {
        public string text;
        public AnimusActor targetActor;
        public bool hasFinishedTalking = true;
    }
}