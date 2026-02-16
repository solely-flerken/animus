using System.Collections.Generic;

namespace Features.Interaction.Scripts.Core
{
    public interface Interactable
    {
        List<InteractionAction> Actions { get; }
    }
}