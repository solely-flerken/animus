using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public class ActionHistoryEntry
    {
        public string ActionKey { get; }
        public List<ActionPayloadParameter> Parameters { get; }
        public string Incentive { get; }
        public string Outcome { get; } // TODO: Maybe make this a bool

        public ActionHistoryEntry(string actionKey, List<ActionPayloadParameter> parameters, string incentive, string outcome)
        {
            ActionKey = actionKey;
            Parameters = parameters;
            Incentive = incentive;
            Outcome = outcome;
        }

        public static ActionHistoryEntry CreateFromPayload(ActionPayload payload, string incentive, string outcome)
        {
            return new ActionHistoryEntry(payload.actionKey, payload.parameters, incentive, outcome);
        }
    }
}