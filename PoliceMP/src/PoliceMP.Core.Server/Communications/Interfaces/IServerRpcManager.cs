using CitizenFX.Core;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications.Interfaces
{
    public interface IServerRpcManager
    {
        Task Emit(Player player, string @event, params object[] args);
        Task Emit(string @event, params object[] args);
        Task<T> Request<T>(Player player, string @event, params object[] args);
        Task HandleMessage(Player player, string json);
    }
}