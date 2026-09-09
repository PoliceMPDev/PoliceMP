using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Communications.Interfaces
{
    public interface IClientRpcManager
    {
        void Emit(string @event, params object[] args);
        void EmitRaw(string @event, params object[] args);
        Task<T> Request<T>(string @event, params object[] args);
        Task HandleMessage(string json);
    }
}