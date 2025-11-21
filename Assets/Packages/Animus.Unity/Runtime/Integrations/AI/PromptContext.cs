using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core.AI;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Environment;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Integrations.AI
{
    public class PromptContext
    {
        [JsonProperty("AgentKey")]
        public string AgentKey { get; set; }
        
        [JsonProperty("Persona", NullValueHandling = NullValueHandling.Ignore)]
        public string Persona { get; set; }

        [JsonProperty("AvailableActions", NullValueHandling = NullValueHandling.Ignore)]
        public List<NpcAction> AvailableActions { get; set; }

        [JsonProperty("RelevantMemories", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RelevantMemories { get; set; }

        [JsonProperty("RecentEvents", NullValueHandling = NullValueHandling.Ignore)]
        public List<AnimusEvent> RecentEvents { get; set; }

        [JsonProperty("EnvironmentDescription", NullValueHandling = NullValueHandling.Ignore)]
        public EnvironmentSnapshot Environment { get; set; }

        [JsonProperty("ActionHistory", NullValueHandling = NullValueHandling.Ignore)]
        public List<ActionHistoryEntry> ActionHistory { get; set; }
        
        [JsonProperty("ConversationHistory", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogLine> ConversationHistory { get; set; }

        [JsonProperty("Rules", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Rules { get; set; }

        [JsonProperty("TaskInstruction")] 
        public string TaskInstruction { get; set; } = TaskInstructions.NextAction;
    }
}