using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications;
using PoliceMP.Core.Shared.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications
{
    public class ServerEventManager : IServerEventManager
    {
        private readonly ConcurrentDictionary<string, List<Delegate>> _subscriptions;
        private readonly ILogger<ServerEventManager> _logger;

        public ServerEventManager(ILogger<ServerEventManager> logger)
        {
            _subscriptions = new ConcurrentDictionary<string, List<Delegate>>();
            _logger = logger;
        }

        public async Task<object> Emit(Player player, RpcMessage rpcMessage)
        {
            try
            {
                if (!_subscriptions.TryGetValue(rpcMessage.Event, out var subscriptions))
                {
                    //_logger.Warn($"No subscriptions found for event {rpcMessage.Event}");
                    return null;
                }

                var subscription = subscriptions.FirstOrDefault();
                if (subscription == null)
                {
                    return null;
                }

                var parameters = new List<object>();
                ParameterInfo[] parametersToInject;

                if (subscription.Method.GetParameters().FirstOrDefault()?.ParameterType == typeof(Player))
                {
                    parameters.Add(player);
                    parametersToInject = subscription.Method.GetParameters().Skip(1).ToArray();
                }
                else
                {
                    parametersToInject = subscription.Method.GetParameters();
                }

                if (rpcMessage.Payload.Length < parametersToInject.Count(p => !p.HasDefaultValue))
                {
                    _logger.Trace(
                        $"Could not trigger {rpcMessage.Event}. Expected {parametersToInject.Length} parameters but got {rpcMessage.Payload.Length}");
                    return null;
                }

                parameters.AddRange(rpcMessage.Payload.Select((t, i) =>
                    JsonConvert.DeserializeObject(t, parametersToInject[i].ParameterType)));

                _logger.Trace($"Invoking {subscription.GetMethodDescription()} for {rpcMessage.Event}");

                var result = subscription.DynamicInvoke(parameters.ToArray());

                if (!(result is Task resultTask)) return result;
                
                await resultTask;

                if (result.GetType().IsGenericType &&
                    result.GetType().GetGenericTypeDefinition() == typeof(Task<>))
                    return result.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(result);

                return result;
            }
            catch (Exception e)
            {
                _logger.Error($"ERROR OCCURED WITH EVENT EMIT PLAYER: {player} {rpcMessage} {e}");
                return null;
            }
        }

    public void Emit(string @event, params object[] args)
    {
        try
        {
            if (!_subscriptions.TryGetValue(@event, out var subscriptions))
            {
                //_logger.Warn($"No subscriptions found for event {@event}");
                return;
            }

            foreach (var subscription in subscriptions)
            {
                var parametersToInject = subscription.Method.GetParameters();

                if (args.Length < parametersToInject.Count(p => !p.HasDefaultValue))
                {
                    _logger.Trace($"Could not trigger {@event}. Expected {parametersToInject.Length} parameters but got {args.Length}");
                    return;
                }

                _logger.Trace($"Invoking {subscription.GetMethodDescription()} for {@event}");
                subscription.DynamicInvoke(args);
            }
        }
        catch (Exception e)
        {
            _logger.Error($"ERROR OCCURED WITH EVENT EMIT: {@event} {e}");
         
        }
    }

        public void On(string @event, Delegate handler)
        {
            try
            {
                lock (_subscriptions)
                {
                    if (!_subscriptions.ContainsKey(@event))
                    {
                        _subscriptions.TryAdd(@event, new List<Delegate>());
                    }

                    _subscriptions[@event].Add(handler);
                    _logger.Trace($"On: {@event} attached to {handler.Method.DeclaringType?.Name}.{handler.Method.Name}");
                }
            }
            catch (Exception e)
            {
                _logger.Error($"ERROR OCCURED ON EVENT: {@event} {e}");
            }
        }
    }
}