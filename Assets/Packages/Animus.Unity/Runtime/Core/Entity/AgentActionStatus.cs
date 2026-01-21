using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AgentActionStatus
    {
        public enum ActionState
        {
            None,
            Ongoing,
            Success,
            Failure
        }

        private readonly AnimusAgent _animusAgent;

        public string ActionStatusContext { get; private set; } = "None";

        private string CurrentAction { get; set; } = "idle";
        private string Parameters { get; set; } = "[]";
        private ActionState CurrentState { get; set; } = ActionState.None;

        public event Action<string, string> OnStatusChanged;

        public AgentActionStatus(AnimusAgent animusAgent)
        {
            _animusAgent = animusAgent;
        }

        public void StartAction(string action, string parameters = "")
        {
            CurrentAction = action;
            Parameters = parameters;
            CurrentState = ActionState.Ongoing;

            UpdateContext();
        }

        public void SetState(ActionState state)
        {
            CurrentState = state;
            UpdateContext();
        }
        
        public void Success()
        {
            SetState(ActionState.Success);
        }
        
        public void Failure()
        {
            SetState(ActionState.Failure);
        }
        
        private void UpdateContext()
        {
            if (CurrentState == ActionState.None)
            {
                ActionStatusContext = "None";
            }
            else
            {
                ActionStatusContext = $"{CurrentAction} -> {Parameters} -> {CurrentState.ToString()}";
            }

            _animusAgent.actionStatusString = ActionStatusContext; // TODO: Remove, only for debugging
            
            // TODO: Maybe invalidate current request and send a new one
            
            Debug.Log($"[AgentActionStatus] {_animusAgent.gameKey}: {ActionStatusContext}");
            OnStatusChanged?.Invoke(_animusAgent.gameKey, ActionStatusContext);
        }
    }
}