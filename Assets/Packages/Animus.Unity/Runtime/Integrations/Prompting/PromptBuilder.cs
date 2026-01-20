using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Modules.GameTime;
using UnityEngine;
using JsonUtility = Packages.Animus.Unity.Runtime.Infrastructure.Serialization.JsonUtility;

namespace Packages.Animus.Unity.Runtime.Integrations.Prompting
{
    public class PromptBuilder
    {
        private readonly PromptContext _context = new();
        private readonly AnimusAgent _agent;
        
        public PromptBuilder(AnimusAgent agent)
        {
            _agent = agent;
        }
        
        public PromptBuilder WithIdentity()
        {
            _context.AgentKey = _agent.gameKey;
            _context.Persona = _agent.persona;
            return this;
        }

        public PromptBuilder WithCurrentState()
        {
            _context.CurrentState += $"It is day {TimeManager.Instance.CurrentDay} and the time is {TimeManager.Instance.GetFormattedTime()}.\n";
            
            // Position
            _context.CurrentState += $"{LocationContextHelper.GetLocationContext(_agent)} ";
            
            // Is this agent is in an active anchor?
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_context.AgentKey, out var anchor))
            {
                // Get everyone else in the conversation
                var others = anchor.Participants.Where(p => p != _context.AgentKey).ToList();
                var othersStr = others.Count > 0 ? string.Join(", ", others) : "No one";
                
                _context.CurrentState += $"You are currently in a conversation with {othersStr}. {anchor.GetTurnContext()}\n";
                _context.TaskInstruction = "You are currently talking. You must reply directly to the last statement while considering the context of the whole conversation history. Respond to the conversation history or leave the conversation to end it.";
            }
            else
            {
                _context.CurrentState += "You are currently Idle.\n";
                _context.TaskInstruction = "You are free to choose any of the available actions.";
            }

            _context.CurrentState = _context.CurrentState.Trim();
            
            return this;
        }

        public PromptBuilder WithSchedule()
        {
            _context.Schedule = _agent?.npcSchedule.GetScheduleContext() ?? "You have no specific schedule right now.";
            return this;
        }
        
        public PromptBuilder WithMotivation()
        {
            _context.Motivation = _agent?.currentMotivation ?? "You have no special motivation.";
            return this;
        }

        public PromptBuilder WithLastAction()
        {
            _context.LastActionResult = _agent?.currentActionResult ?? "None.";
            return this;
        }
        
        public PromptBuilder WithAvailableActions()
        {
            _context.AvailableActions = _agent.agentActionSystem.GenerateSchema();
            return this;
        }

        public PromptBuilder WithRelevantMemories()
        {
            _context.RelevantMemories = _agent.memories;
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
    
    public static class LocationContextHelper
    {
        private const string MsgAt = "You are at the {0}.";
        private const string MsgNear = "You are near the {0}.";
        private const string MsgNone = "You are not near any significant location.";
        
        public static string GetLocationContext(AnimusAgent agent, float atDistance = 10f, float nearDistance = 50f)
        {
            if (agent == null)
            {
                Debug.LogError("[LocationContextHelper] No agent provided.");
                return string.Empty;
            }
            
            var allLocations = AnimusGameManager.EntityRegistry.GetLocationsRelevantTo(agent);

            if (!TryGetNearestLocation(agent.transform.position, allLocations, out var nearestLocation, out var distanceSqr))
            {
                return MsgNone;
            }
            
            if (distanceSqr <= atDistance * atDistance)
            {
                return string.Format(MsgAt, nearestLocation.entityName);
            }

            if (distanceSqr <= nearDistance * nearDistance)
            {
                return string.Format(MsgNear, nearestLocation.entityName);
            }

            return MsgNone;
        }

        private static bool TryGetNearestLocation(Vector3 origin, IEnumerable<AnimusLocation> locations, out AnimusLocation result, out float closestDistSqr)
        {
            result = null;
            closestDistSqr = float.MaxValue;

            foreach (var location in locations)
            {
                if (location == null) continue;

                var distanceSqr = (location.transform.position - origin).sqrMagnitude;

                if (distanceSqr < closestDistSqr)
                {
                    closestDistSqr = distanceSqr;
                    result = location;
                }
            }

            return result;
        }
    }
}