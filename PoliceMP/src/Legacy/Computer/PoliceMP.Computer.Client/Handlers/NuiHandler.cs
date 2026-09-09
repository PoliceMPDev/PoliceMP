using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;

namespace PoliceMP.Computer.Client.Handlers
{
    public class NuiHandler
    {
        /// <summary>
        /// A dictionary of all NUI events and all the methods that subscribe
        /// to them.
        /// </summary>
        private readonly IDictionary<string, List<Delegate>> _subscriptions;

        /// <summary>
        /// Create a new instance of the class.
        /// </summary>
        public NuiHandler()
        {
            _subscriptions = new Dictionary<string, List<Delegate>>();
        }

        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="handler">The event handler.</param>
        public void On(string eventName, Action<IDictionary<string, object>, CallbackDelegate> handler)
        {
            lock (_subscriptions)
            {
                if (!_subscriptions.ContainsKey(eventName))
                {
                    _subscriptions.Add(eventName, new List<Delegate>());
                    API.RegisterNuiCallbackType(eventName);
                }

                Main.RegisterNuiCallback(eventName, handler);

                _subscriptions[eventName].Add(handler);

                Debug.WriteLine($"OnNui: {eventName} attached to {handler.Method.DeclaringType?.Name}.{handler.Method.Name}");
            }
        }
    }
}
