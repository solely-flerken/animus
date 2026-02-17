using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Integrations.Prompting
{
    /// <summary>
    /// Attach components implementing this interface to an AnimusAgent 
    /// to inject custom situational awareness into the prompt.
    /// </summary>
    public interface ISituationalContextProvider
    {
        /// <summary>
        /// Returns a list of strings describing the current specific situation 
        /// </summary>
        List<string> GetSituationalContext();
    }
}