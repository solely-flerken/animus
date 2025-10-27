using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Packages.Animus.Unity.Runtime.Environment;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.AI
{
    public class PromptBuilder
    {
        private readonly PromptContext _context = new();

        public PromptBuilder WithPersona(AnimusAgent agent)
        {
            _context.Persona = agent.persona;
            return this;
        }

        public PromptBuilder WithAvailableActions(List<NpcAction> actions)
        {
            _context.AvailableActions = actions;
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

        public string Build(bool prettyPrint = false)
        {
            var formatting = prettyPrint ? Formatting.Indented : Formatting.None;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new IgnoreUnityBasePropertiesResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(_context, formatting, settings);
        }
    }
}