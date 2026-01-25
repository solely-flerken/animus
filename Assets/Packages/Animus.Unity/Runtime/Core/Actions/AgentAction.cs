using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class AgentAction
    {
        public string Name { get; }
        public string Description { get; }
    
        private readonly Func<Dictionary<string, object>, CancellationToken, UniTask<string>> _executionLogic;
        private readonly Action<Dictionary<string, object>> _onSuccess; // TODO: Refactor (implemented this because of race conditions with interruptions)
        private readonly Func<bool> _condition;

        public List<(string paramName, string paramType)> Parameters { get; } = new();

        public AgentAction(string name, string description, Func<Dictionary<string, object>, CancellationToken, UniTask<string>> logic, Action<Dictionary<string, object>> onSuccess = null, Func<bool> condition = null)
        {
            Name = name;
            Description = description;
            _executionLogic = logic;
            _onSuccess = onSuccess;
            _condition = condition ?? (() => true);
        }

        public bool IsAvailable() => _condition.Invoke();

        public UniTask<string> ExecuteAsync(Dictionary<string, object> args, CancellationToken ct) => _executionLogic.Invoke(args, ct);

        public void InvokeSuccess(Dictionary<string, object> args)
        {
            _onSuccess?.Invoke(args);
        }
        
        public AgentAction AddParam<T>(string name)
        {
            Parameters.Add((name, typeof(T).Name));
            return this;
        }
    }
}