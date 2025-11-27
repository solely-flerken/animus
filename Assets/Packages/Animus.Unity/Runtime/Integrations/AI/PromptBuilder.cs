using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using Packages.Animus.Unity.Runtime.Modules.Environment;

namespace Packages.Animus.Unity.Runtime.Integrations.AI
{
    public class PromptBuilder
    {
        private readonly PromptContext _context = new();

        public PromptBuilder SetAgent(AnimusAgent agent)
        {
            _context.AgentKey = agent.gameKey;
            _context.Persona = agent.persona;
            return this;
        }

        public PromptBuilder WithAvailableActions(AgentActionRunner runner)
        {
            _context.AvailableActions = runner.GenerateActionSchema();
            return this;
        }

        public PromptBuilder WithRelevantMemories(List<string> relevantMemories)
        {
            _context.RelevantMemories = relevantMemories;
            return this;
        }

        public PromptBuilder WithRecentEvents(List<AnimusEvent> recentEvents)
        {
            _context.RecentEvents = recentEvents;
            return this;
        }

        public PromptBuilder WithEnvironment(EnvironmentSnapshot environmentSnapshot)
        {
            _context.Environment = environmentSnapshot;
            return this;
        }

        public PromptBuilder WithActionHistory(List<ActionHistoryEntry> actionHistory)
        {
            _context.ActionHistory = actionHistory;
            return this;
        }
        
        public PromptBuilder WithConversationHistory(List<DialogLine> conversationHistory)
        {
            _context.ConversationHistory = conversationHistory;
            return this;
        }

        public PromptBuilder WithRules(List<string> rules)
        {
            _context.Rules = rules;
            return this;
        }

        public PromptBuilder WithTaskInstruction(string instruction)
        {
            if (!string.IsNullOrEmpty(instruction))
                _context.TaskInstruction = instruction;

            return this;
        }

        public PromptContext GetContext()
        {
            return _context;
        }

        public string Build(bool prettyPrint = false)
        {
            return JsonUtility.Serialize(_context, prettyPrint);
        }
    }
}