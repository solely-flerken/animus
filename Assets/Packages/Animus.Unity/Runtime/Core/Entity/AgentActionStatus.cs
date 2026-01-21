using System;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AgentActionStatus
    {
        private readonly AnimusAgent _animusAgent;
        
        public string CurrentActionStatus { get; private set; } = "None.";
        public event Action<string, string> OnStatusChanged;
        
        public AgentActionStatus(AnimusAgent animusAgent)
        {
            _animusAgent = animusAgent;
        }

        public void Set(string currentActionStatus)
        {
            CurrentActionStatus = currentActionStatus;
            
            // TODO: Maybe invalidate current request and send a new one
            // Debug.Log($"[AgentActionStatus] Set status: {CurrentActionStatus}");
            
            OnStatusChanged?.Invoke(_animusAgent.gameKey, CurrentActionStatus);
        }
    }
}