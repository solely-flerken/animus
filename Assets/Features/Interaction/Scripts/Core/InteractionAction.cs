using UnityEngine;

namespace Features.Interaction.Scripts.Core
{
    public abstract class InteractionAction
    {
        public abstract string Label { get; }
        public abstract void Execute(GameObject interactor);
    }
}