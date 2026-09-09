using CitizenFX.Core;
using PoliceMP.Core.Shared.Communications.Interfaces;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications
{
    public class FiveEventManager : IFiveEventManager
    {
        private readonly EventHandlerDictionary _eventHandlerDictionary;

        public FiveEventManager(EventHandlerDictionary eventHandlerDictionary)
        {
            _eventHandlerDictionary = eventHandlerDictionary;
        }

        public void On(string @event, Delegate callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On(string @event, Func<Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On<T>(string @event, Func<T, Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On<T1, T2>(string @event, Func<T1, T2, Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On<T1, T2, T3>(string @event, Func<T1, T2, T3, Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On<T1, T2, T3, T4>(string @event, Func<T1, T2, T3, T4, Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void On<T1, T2, T3, T4, T5>(string @event, Func<T1, T2, T3, T4, T5, Task> callback)
            => _eventHandlerDictionary[@event] += callback;

        public void Off(string @event, Delegate callback)
            => _eventHandlerDictionary[@event] -= callback;
    }
}