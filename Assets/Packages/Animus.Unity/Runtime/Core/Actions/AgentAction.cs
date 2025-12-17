using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class AgentAction
    {
        public string Name { get; }
        public string Description { get; }
    
        private readonly Func<Dictionary<string, object>, UniTask<string>> _executionLogic;
        private readonly Func<bool> _condition;

        public List<(string paramName, string paramType)> Parameters { get; } = new();

        public AgentAction(string name, string description, Func<Dictionary<string, object>, UniTask<string>> logic, Func<bool> condition = null)
        {
            Name = name;
            Description = description;
            _executionLogic = logic;
            _condition = condition ?? (() => true);
        }

        public bool IsAvailable() => _condition.Invoke();

        public UniTask<string> ExecuteAsync(Dictionary<string, object> args) => _executionLogic.Invoke(args);

        public AgentAction AddParam<T>(string name)
        {
            Parameters.Add((name, typeof(T).Name));
            return this;
        }
    }
}