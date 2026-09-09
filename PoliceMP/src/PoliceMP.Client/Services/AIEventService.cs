using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Shared.Communications.Interfaces;

namespace PoliceMP.Client.Services
{
    public class AiEventService : IAiEventService
    {
        private readonly IFiveEventManager _fiveEvents;

        public struct ListenerEntry
        {
            public int PedHandle { get; }
            public string ShockingEventName { get; }
            public Func<List<object>, object, List<object>, Task> HandlerWrapper { get; }
            public OnShockingEventDelegate DownstreamHandler { get; }

            public ListenerEntry(int pedHandle, string shockingEventName, Func<List<object>, object, List<object>, Task> handlerWrapper, OnShockingEventDelegate downstreamHandler)
            {
                PedHandle = pedHandle;
                ShockingEventName = shockingEventName;
                HandlerWrapper = handlerWrapper;
                DownstreamHandler = downstreamHandler;
            }
        }

        // Stores listener info for a given event
        public ConcurrentDictionary<string, List<ListenerEntry>> _handlers = new ConcurrentDictionary<string, List<ListenerEntry>>();

        public AiEventService(IFiveEventManager fiveEvents)
        {
            _fiveEvents = fiveEvents;
        }

        public void AddShockingEventListener(Ped sensoryPed, string shockingEventName, OnShockingEventDelegate handler)
        {
            AddShockingEventListener(sensoryPed.Handle, shockingEventName, handler);
        }

        public void AddShockingEventListener(int sensoryPedHandle, string shockingEventName, OnShockingEventDelegate handler)
        {
            if (!API.DoesEntityExist(sensoryPedHandle))
                throw new ArgumentException($"Entity {sensoryPedHandle} does not exist.", nameof(sensoryPedHandle));

            if (!API.IsEntityAPed(sensoryPedHandle))
                throw new ArgumentException($"Entity {sensoryPedHandle} is not a ped.", nameof(sensoryPedHandle));

            lock (_handlers)
            {
                if (!_handlers.TryGetValue(shockingEventName, out var listeners))
                {
                    listeners = new List<ListenerEntry>();
                    _handlers[shockingEventName] = listeners;
                }

                if (!listeners.Any(l =>
                        l.PedHandle == sensoryPedHandle && 
                        l.DownstreamHandler == handler))
                {
                    var handlerWrapper = new Func<List<object>, object, List<object>, Task>(
                        async (involved, source, data) =>
                        {
                            if (involved.Select(i => int.Parse(i.ToString())).Contains(sensoryPedHandle))
                            {
                                await handler(involved.Select(i => Entity.FromHandle(int.Parse(i.ToString()))).ToList().AsReadOnly(),
                                    Entity.FromHandle((int)source), data.AsReadOnly());
                            }
                        });

                    _fiveEvents.On(shockingEventName, handlerWrapper);
                    listeners.Add(new ListenerEntry(sensoryPedHandle, shockingEventName, handlerWrapper, handler));
                }
            }
        }

        public void RemoveShockingEventListener(Ped sensoryPed, string shockingEventName, OnShockingEventDelegate handler)
        {
            RemoveShockingEventListener(sensoryPed.Handle, shockingEventName, handler);
        }

        public void RemoveShockingEventListener(int sensoryPedHandle, string shockingEventName, OnShockingEventDelegate handler)
        {
            lock (_handlers)
            {
                if (_handlers.TryGetValue(shockingEventName, out var listeners))
                {
                    var toRemove = listeners.Where(l => l.PedHandle == sensoryPedHandle && l.DownstreamHandler == handler);

                    foreach (var listener in toRemove)
                    {
                        _fiveEvents.Off(shockingEventName, listener.HandlerWrapper);
                    }
                }
            }
        }
    }
}
