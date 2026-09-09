using PoliceMP.Core.Shared.Communications;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Communications.Interfaces
{
    public interface IClientEventManager
    {
        void Emit(string @event, params object[] args);
        void EmitRaw(string @event, params object[] args);
        Task<object> Emit(RpcMessage rpcMessage);
        void On(string @event, Delegate handler);
    }
}