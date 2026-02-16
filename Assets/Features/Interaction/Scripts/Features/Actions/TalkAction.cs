using Features.Interaction.Scripts.Core;
using Features.Player.Scripts;
using UnityEngine;

namespace Features.Interaction.Scripts.Features.Actions
{
    public class TalkAction : InteractionAction
    {
        private readonly string _npcKey;

        public override string Label => $"Talk to {_npcKey}";
        
        public TalkAction(string npcKey)
        {
            _npcKey = npcKey;
        }
        
        public override void Execute(GameObject interactor)
        {
            var player = interactor.GetComponent<AnimusPlayerController>();
            player?.InitiateConversation(_npcKey);
        }
    }
}