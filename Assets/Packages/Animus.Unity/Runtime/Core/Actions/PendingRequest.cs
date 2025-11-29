using System;
using System.Threading;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class PendingRequest
    {
        public string agentKey;
        public DateTime timestamp;
        public PromptContext contextSnapshot; // The exact state when request was made
        public CancellationTokenSource cts; // Allows us to kill this specific request
    }
}