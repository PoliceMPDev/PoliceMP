using PoliceMP.Computer.Client.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Computer.Client.Handlers
{
    public class CommunicationsHandler
    {
        private readonly IDictionary<string, List<Delegate>> _subscriptions;
        private readonly ILogger _logger;

        public CommunicationsHandler(ILogger logger)
        {
            _subscriptions = new Dictionary<string, List<Delegate>>();
            _logger = logger;
        }

        public void ToClient(string eventName, params object[] args)
        {
            lock (_subscriptions)
            {
                if (!_subscriptions.ContainsKey(eventName))
                    return;

                _logger.Log($"Emiting mate");

                foreach (var subscription in _subscriptions[eventName])
                {
                    if (args.Count() > 0)
                    {
                        if (args[0] is List<dynamic>)
                        {
                            var payload = (List<dynamic>)args[0];
                            subscription.DynamicInvoke(payload.ToArray());
                        }
                        else
                            subscription.DynamicInvoke(args.ToArray());
                    }
                    else
                        subscription.DynamicInvoke();
                }
            }
        }

        private void _On(string eventName, Delegate handler)
        {
            lock (_subscriptions)
            {
                if (!_subscriptions.ContainsKey(eventName))
                {
                    _subscriptions.Add(eventName, new List<Delegate>());
                }

                _subscriptions[eventName].Add(handler);
                _logger.Log($"On: {eventName} attached to {handler.Method.DeclaringType?.Name}.{handler.Method.Name}");
            }
        }

        public void On(string eventName, Action handler) => _On(eventName, handler);

        public void On<T>(string eventName, Action<T> handler) => _On(eventName, handler);

        public void On<T1, T2>(string eventName, Action<T1, T2> handler) => _On(eventName, handler);

        public void On<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler) => _On(eventName, handler);

        public void On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> handler) => _On(eventName, handler);

        public void On<T1, T2, T3, T4, T5>(string eventName, Action<T1, T2, T3, T4, T5> handler) => _On(eventName, handler);
    }
}
