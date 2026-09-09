using CitizenFX.Core;
using PoliceMP.Core.Shared.Communications;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications.Interfaces
{
    public interface IServerEventManager
    {
        Task<object> Emit(Player player, RpcMessage rpcMessage);
        void Emit(string @event, params object[] args);
        void On(string @event, Delegate handler);
    }
}