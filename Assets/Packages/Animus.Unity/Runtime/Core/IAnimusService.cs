using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Agent.Actions;

namespace Packages.Animus.Unity.Runtime.Core
{
    public interface IAnimusService
    {
        Task<AnimusEntity<T>> RegisterAgent<T>(AnimusEntity<T> agent);
        Task ActivateAgent<T>(AnimusEntity<T> agent);
        Task<ActionPayload> PollAction();
    }
}