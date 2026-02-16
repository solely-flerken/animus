using System;
using UnityEngine;

namespace Features.Interaction.Scripts.Core
{
    public class DelegateAction : InteractionAction
    {
        private readonly Func<string> _label;
        private readonly Action<GameObject> _onExecute;
        
        public override string Label => _label();

        public DelegateAction(Func<string> label, Action<GameObject> execute, Func<GameObject, bool> isAvailable = null)
        {
            _label = label;
            _onExecute = execute;
        }

        public DelegateAction(string label, Action<GameObject> execute, Func<GameObject, bool> isAvailable = null) : this(() => label, execute, isAvailable)
        {
        }
        
        public override void Execute(GameObject interactor)
        {
            _onExecute?.Invoke(interactor);
        }
    }
}