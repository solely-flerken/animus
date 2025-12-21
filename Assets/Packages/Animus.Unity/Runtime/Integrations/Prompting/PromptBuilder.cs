using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Modules.GameTime;
using Packages.Animus.Unity.Runtime.Modules.Memory;
using UnityEngine;
using JsonUtility = Packages.Animus.Unity.Runtime.Infrastructure.Serialization.JsonUtility;

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
            _context.CurrentState += $"It is day {TimeManager.Instance.CurrentDay} and the time is {TimeManager.Instance.GetFormattedTime()}\n";
            
            // Is this agent is in an active anchor?
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_context.AgentKey, out var anchor))
            {
                // Get everyone else in the conversation
                var others = anchor.Participants.Where(p => p != _context.AgentKey).ToList();
                var othersStr = others.Count > 0 ? string.Join(", ", others) : "No one";
                
                _context.CurrentState += $"You are currently in a conversation with {othersStr}. {anchor.GetTurnContext()}\n";
                _context.TaskInstruction = "You are currently talking. Respond to the conversation history or leave the conversation to end it.";
            }
            else
            {
                _context.CurrentState += "You are currently Idle and have nothing specific to do.\n";
                _context.TaskInstruction = "You are free to choose any of the available actions.";
            }
            
            return this;
        }

        public PromptBuilder WithMotivation()
        {
            var agent = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(_context.AgentKey);
            if (agent == null)
            {
                Debug.Log($"[PromptBuilder.Motivation] Couldn't find agent with key {_context.AgentKey}");
                return this;
            }

            var schedule = agent.npcSchedule;
            _context.Motivation = schedule ? schedule.GetScheduleContext() : "I have no specific schedule right now.";

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