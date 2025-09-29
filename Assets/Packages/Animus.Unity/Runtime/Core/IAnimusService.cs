using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Data;

namespace Packages.Animus.Unity.Runtime.Core
{
    public interface IAnimusService
    {
        Task<AgentModel> RegisterAgent(AgentModel agent);
    }
}