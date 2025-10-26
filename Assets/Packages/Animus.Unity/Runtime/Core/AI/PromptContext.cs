using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.AI
{
    public class PromptContext
    {
        [JsonProperty("persona", NullValueHandling = NullValueHandling.Ignore)]
        public string Persona { get; set; }

        [JsonProperty("available_actions", NullValueHandling = NullValueHandling.Ignore)]
        public List<NpcAction> AvailableActions { get; set; }

        [JsonProperty("relevant_memories", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RelevantMemories { get; set; }

        [JsonProperty("recent_events", NullValueHandling = NullValueHandling.Ignore)]
        public List<AnimusEvent> RecentEvents { get; set; }

        [JsonProperty("environment_description", NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentDescription { get; set; }

        [JsonProperty("conversation_history", NullValueHandling = NullValueHandling.Ignore)]
        public List<DialogLine> ConversationHistory { get; set; }

        [JsonProperty("rules", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Rules { get; set; }

        [JsonProperty("task_instruction")] 
        public string TaskInstruction { get; set; } = TaskInstructions.NextAction;
    }
}