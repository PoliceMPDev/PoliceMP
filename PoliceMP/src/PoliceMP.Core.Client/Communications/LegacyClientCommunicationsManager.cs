using PoliceMP.Core.Client.Communications.Interfaces;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Communications
{
    public class LegacyClientCommunicationsManager : ILegacyClientCommunicationsManager
    {
        private readonly IClientEventManager _events;
        private readonly IClientRpcManager _rpc;

        public LegacyClientCommunicationsManager(IClientEventManager events, IClientRpcManager rpc)
        {
            _events = events;
            _rpc = rpc;
        }

        public void ToServer(string @event, params object[] args)
        {
            _rpc.Emit(@event, args);
        }

        public void ToServerRaw(string @event, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void ToClient(string @event, params object[] args)
        {
            _events.Emit(@event, args);
        }

        public void ToClientRaw(string @event, params object[] args)
        {
            _events.EmitRaw(@event, args);
        }

        public async Task<T> Request<T>(string @event, params object[] args) => await _rpc.Request<T>(@event, args);

        public void OnRequest<TIn, TReturn>(string @event, Func<TIn, Task<TReturn>> handler) => _events.On(@event, handler);

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

        public void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler) => _events.On(@event, handler);
    }
}