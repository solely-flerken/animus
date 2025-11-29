using System;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class QueuedAction
    {
        public string agentKey;
        public DateTime requestTimestamp;
        public PromptContext originalContext;
        public ActionPayload responsePayload;
    }
}