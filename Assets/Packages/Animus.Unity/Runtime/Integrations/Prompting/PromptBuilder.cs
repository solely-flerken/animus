using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Modules.Memory;

namespace Packages.Animus.Unity.Runtime.Integrations.Prompting
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

        public PromptBuilder WithCurrentState()
        {
            // Is this agent is in an active anchor?
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_context.AgentKey, out var anchor))
            {
                // Get everyone else in the conversation
                var others = anchor.Participants.Where(p => p != _context.AgentKey).ToList();
                var othersStr = others.Count > 0 ? string.Join(", ", others) : "No one";

                _context.CurrentState =
                    $"STATUS: IN CONVERSATION\n" +
                    $"PARTICIPANTS: {othersStr}\n" +
                    $"{anchor.GetTurnContext()}\n" +
                    $"INSTRUCTION: You are currently talking. Respond to the conversation history or leave the conversation to end it.";
            }
            else
            {
                _context.CurrentState =
                    "STATUS: IDLE\n" +
                    "INSTRUCTION: You are free to choose any of the available actions.";
            }
            
            return this;
        }
        
        public PromptBuilder WithAvailableActions(AgentActionSystem agentActionSystem)
        {
            _context.AvailableActions = agentActionSystem.GenerateSchema();
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

        public PromptContext GetContext()
        {
            return _context;
        }

        public string Build(bool prettyPrint = false)
        {
            return JsonUtility.Serialize(_context, prettyPrint);
        }

        public string BuildString()
        {
            return "";
        }
    }
}