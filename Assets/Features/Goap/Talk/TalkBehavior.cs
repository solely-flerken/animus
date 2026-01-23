using System;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Goap.Talk
{
    public class TalkBehavior : MonoBehaviour
    {
        public string text;
        public AnimusActor targetActor;

        public bool hasStartedTalking;
        public bool hasFinishedTalking = true;

        public async UniTask TalkAsync(string sender)
        {
            try
            {
                hasStartedTalking = true;
                hasFinishedTalking = false;
                await Chat.Scripts.Chat.Instance.LogMessage(sender, text, false);
                hasFinishedTalking = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"TalkBehavior Error: {e}");
                hasFinishedTalking = true;
            }
        }
    }
}