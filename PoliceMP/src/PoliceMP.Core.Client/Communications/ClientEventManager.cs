using Newtonsoft.Json;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications;
using PoliceMP.Core.Shared.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Core.Client.Communications
{
    public class ClientEventManager : IClientEventManager
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _subscriptions;
        private readonly ILogger<ClientEventManager> _logger;

        public ClientEventManager(ILogger<ClientEventManager> logger)
        {
            _subscriptions = new ConcurrentDictionary<string, List<Delegate>>();
            _logger = logger;
        }

        public void Emit(string @event, params object[] args)
        {
            if (!_subscriptions.TryGetValue(@event, out var subscriptions))
            {
                _logger.Warn($"No subscriptions found for event {@event}");
                return;
            }

            foreach (var subscription in subscriptions)
            {
                _logger.Trace($"Invoking {subscription.GetMethodDescription()} for {@event}");
                subscription.DynamicInvoke(args);
            }
        }

        public void EmitRaw(string @event, params object[] args)
        {
            BaseScript.TriggerEvent(@event, args);
        }

        public async Task<object> Emit(RpcMessage rpcMessage)
        {
            if (!_subscriptions.TryGetValue(rpcMessage.Event, out var subscriptions))
            {
                _logger.Warn($"No subscriptions found for event {rpcMessage.Event}");
                return null;
            }

            var subscription = subscriptions.FirstOrDefault();
            if (subscription == null) return null;

            var parameters = new List<object>();
            var parametersToInject = subscription.Method.GetParameters();

            if (rpcMessage.Payload.Length < parametersToInject.Length)
            {
                _logger.Trace($"Could not trigger {rpcMessage.Event}. Expected {parametersToInject.Length} but got {rpcMessage.Payload.Length}");
                return null;
            }

            for (int i = 0; i < rpcMessage.Payload.Length; i++)
            {
                parameters.Add(JsonConvert.DeserializeObject(rpcMessage.Payload[i], parametersToInject[i].ParameterType));
            }

            _logger.Trace($"Invoking {subscription.GetMethodDescription()} for {rpcMessage.Event}");

            dynamic result = subscription.DynamicInvoke(parameters.ToArray());
            return result == null ? null : await result;
        }

        public void On(string @event, Delegate handler)
        {
            if (!_subscriptions.ContainsKey(@event))
            {
                if (!_subscriptions.TryAdd(@event, new List<Delegate>()))
                {
                    _logger.Warn($"Failed subscribe {handler.GetMethodDescription()} to {@event}");
                    return;
                }
            }
            _subscriptions[@event].Add(handler);
            _logger.Trace($"On: {@event} attached to {handler.GetMethodDescription()}");
        }
    }
}