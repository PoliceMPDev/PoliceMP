using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared.Communications;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications
{
    public class LegacyServerCommunicationsManager : ILegacyServerCommunicationsManager
    {
        private readonly IServerRpcManager _rpc;
        private readonly IServerEventManager _events;

        public LegacyServerCommunicationsManager(IServerEventManager events, IServerRpcManager rpc)
        {
            _events = events;
            _rpc = rpc;
        }

        public void ToServer(string @event, params object[] args)
        {
            _events.Emit(@event, args);
        }

        public async Task ToClient(Player player, RpcMessage rpcMessage)
        {
            await _events.Emit(player, rpcMessage);
        }

        public void ToClient(string @event, params object[] args)
        {
            _rpc.Emit(@event, args);
        }

        public void ToClient(Player player, string @event, params object[] args)
        {
            _rpc.Emit(player, @event, args);
        }

        public void OnRequest<TReturn>(string @event, Func<Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn, TReturn>(string @event, Func<TIn, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TReturn>(string @event, Func<TIn1, TIn2, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TReturn>(string @event, Func<TIn1, TIn2, TIn3, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, TIn5, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, Task<TReturn>> handler) => _events.On(@event, handler);

        public void OnRequest<TReturn>(string @event, Func<Player, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn, TReturn>(string @event, Func<Player, TIn, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TReturn>(string @event, Func<Player, TIn1, TIn2, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, TIn5, Task<TReturn>> handler) => _events.On(@event, handler);
        public void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, Task<TReturn>> handler) => _events.On(@event, handler);
        public async Task<T> Request<T>(Player player, string @event, params object[] args) => await _rpc.Request<T>(player, @event, args);

        public void On(string @event, Action handler) => _events.On(@event, handler);
        public void On<T>(string @event, Action<T> handler) => _events.On(@event, handler);
        public void On<T1, T2>(string @event, Action<T1, T2> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3>(string @event, Action<T1, T2, T3> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4>(string @event, Action<T1, T2, T3, T4> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5>(string @event, Action<T1, T2, T3, T4, T5> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6>(string @event, Action<T1, T2, T3, T4, T5, T6> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<T1, T2, T3, T4, T5, T6, T7> handler) => _events.On(@event, handler);

        public void On(string @event, Func<Task> handler) => _events.On(@event, handler);
        public void On<T>(string @event, Func<T, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2>(string @event, Func<T1, T2, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3>(string @event, Func<T1, T2, T3, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4>(string @event, Func<T1, T2, T3, T4, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5>(string @event, Func<T1, T2, T3, T4, T5, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6>(string @event, Func<T1, T2, T3, T4, T5, T6, Task> handler) => _events.On(@event, handler);

        public void On(string @event, Action<Player> handler) => _events.On(@event, handler);
        public void On<T>(string @event, Action<Player, T> handler) => _events.On(@event, handler);
        public void On<T1, T2>(string @event, Action<Player, T1, T2> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3>(string @event, Action<Player, T1, T2, T3> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4>(string @event, Action<Player, T1, T2, T3, T4> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5>(string @event, Action<Player, T1, T2, T3, T4, T5> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6>(string @event, Action<Player, T1, T2, T3, T4, T5, T6> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<Player, T1, T2, T3, T4, T5, T6, T7> handler) => _events.On(@event, handler);
        
        public void On(string @event, Func<Player, Task> handler) => _events.On(@event, handler);
        public void On<T>(string @event, Func<Player, T, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2>(string @event, Func<Player, T1, T2, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3>(string @event, Func<Player, T1, T2, T3, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4>(string @event, Func<Player, T1, T2, T3, T4, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5>(string @event, Func<Player, T1, T2, T3, T4, T5, Task> handler) => _events.On(@event, handler);
        public void On<T1, T2, T3, T4, T5, T6>(string @event, Func<Player, T1, T2, T3, T4, T5, T6, Task> handler) => _events.On(@event, handler);

    }
}