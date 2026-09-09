using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;

namespace PoliceMP.Core.Server.Communications
{
    /// <summary>
    /// Mediates client and server communications for client
    /// </summary>
    public class ServerCommunicationsManager : IServerCommunicationsManager
    {
        private readonly ILogger<ServerCommunicationsManager> _log;
        private readonly IFiveEventManager _fiveEvents;

        private readonly HashSet<string> _requestHandlers = new HashSet<string>();

        // SourceDelegate, HandlerImplementingDelegate
        private readonly Dictionary<Delegate, Delegate> _notificationHandlers = new Dictionary<Delegate, Delegate>();

        private readonly Dictionary<Type, Delegate> _notificationForwarders =
            new Dictionary<Type, Delegate>();

        public ServerCommunicationsManager(ILogger<ServerCommunicationsManager> log, IFiveEventManager fiveEvents)
        {
            _log = log;
            _fiveEvents = fiveEvents;
        }

        public void SendToClients<TRequest>(TRequest request) where TRequest : IClientRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();
            var payload = JsonConvert.SerializeObject(request);

            BaseScript.TriggerClientEvent(eventName, null, payload);
        }

        public void SendToClient<TRequest>(Player player, TRequest request) where TRequest : IClientRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();
            var payload = JsonConvert.SerializeObject(request);

            BaseScript.TriggerClientEvent(player, eventName, payload);
        }

        public async Task<TResponse> SendToClient<TResponse>(Player player, IClientRequest<TResponse> request,
            int timeoutMs = 10000)

        {
            var eventName = MediatorUtils.GetEventNameForType(request);
            var responseEventName = eventName + "_response";
            var payload = JsonConvert.SerializeObject(request);

            var tcs = new TaskCompletionSource<TResponse>();
            var requestId = Guid.NewGuid().ToString();

            // ReSharper disable once ConvertToLocalFunction
            Action<string, string> responseHandler = (string responseRequestId, string responseData) =>


            {
                if (responseRequestId == requestId)
                {
                    TResponse response = JsonConvert.DeserializeObject<TResponse>(responseData);
                    tcs.SetResult(response);
                }
            };

            _fiveEvents.On(responseEventName, responseHandler);

            try
            {
                BaseScript.TriggerClientEvent(player, eventName, requestId, payload);

                if (timeoutMs > 0)
                {
                    await Task.WhenAny(tcs.Task, BaseScript.Delay(timeoutMs));
                    if (!tcs.Task.IsCompleted)
                    {
                        _log.Error(
                            $"Message {eventName} failed to get a response in the timeout time of {timeoutMs}");
                        return default;
                    }
                }
                else
                {
                    await tcs.Task;
                }
            }
            finally
            {
                _fiveEvents.Off(responseEventName, responseHandler);
            }

            return tcs.Task.Result;
        }

        public void PublishAll<TNotification>(TNotification notification) where TNotification : INotification
        {
            PublishToClients(notification);
            PublishToServer(notification);
        }

        public void PublishToServer<TNotification>(TNotification notification) where TNotification : INotification
        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var payload = JsonConvert.SerializeObject(notification);

            BaseScript.TriggerEvent(eventName, null, payload);
        }

        public void PublishToClients<TNotification>(TNotification notification) where TNotification : INotification
        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var payload = JsonConvert.SerializeObject(notification);

            BaseScript.TriggerClientEvent(eventName, null, payload);
        }

        public void PublishToClient<TNotification>(Player player, TNotification notification)
            where TNotification : INotification

        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var payload = JsonConvert.SerializeObject(notification);

            BaseScript.TriggerClientEvent(player, eventName, null, payload);
        }

        public void AddRequestHandler<TRequest>(Func<TRequest, Task> handler) where TRequest : IServerRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists! in {eventName}");
                    return;
                }
            }

            _fiveEvents.On(eventName, new Func<string, string, Task>(async (requestId, payload) =>
            {
                if (requestId != null)
                {
                    _log.Error(
                        $"EventHandler {eventName} expects a response but the payload type {payload.GetType().FullName} doesn't! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TRequest>(payload);

                try
                {
                    await handler(obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                }
            }));

            _log.Debug($"Added request handler for {eventName} with no response.");
        }

        public void AddRequestHandler<TRequest>(Func<Player, TRequest, Task> handler) where TRequest : IServerRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists in {eventName}");
                    return;
                }
            }

            async Task Handle([FromSource] Player player, string requestId, string payload)
            {
                if (requestId != null)
                {
                    _log.Error(
                        $"EventHandler {eventName} expects a response but the payload type {payload.GetType().FullName} doesn't! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TRequest>(payload);

                try
                {
                    await handler(player, obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                }
            }

            _fiveEvents.On(eventName, new Func<Player, string, string, Task>(Handle));

            _log.Debug($"Added request handler for {eventName} with no response.");
        }

        public void AddRequestHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
            where TRequest : IServerRequest<TResponse>


        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists! in {eventName}");
                    return;
                }
            }

            async Task Handle([FromSource] Player player, string requestId, string payload)
            {
                if (requestId == null)
                {
                    _log.Error(
                        $"EventHandler {eventName} does not expect a response but the payload type {payload.GetType().FullName} does! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TRequest>(payload);

                TResponse response;
                try
                {
                    response = await handler(obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                    return;
                }

                var responsePayload = JsonConvert.SerializeObject(response);

                BaseScript.TriggerClientEvent(player, eventName + "_response", requestId, responsePayload);
            }

            _fiveEvents.On(eventName, new Func<Player, string, string, Task>(Handle));

            _log.Debug($"Added request handler for {eventName} with response {eventName}_response.");
        }

        public void AddRequestHandler<TRequest, TResponse>(Func<Player, TRequest, Task<TResponse>> handler)
            where TRequest : IServerRequest<TResponse>

        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists! in {eventName}");
                    return;
                }
            }

            async Task Handle([FromSource] Player player, string requestId, string payload)
            {
                if (eventName == "PMP::Dog.Commands.ServerSpawnDogCommand")
                {
                    _log.Debug("Spawn the damn dog!");
                }

                if (requestId == null)
                {
                    _log.Error(
                        $"EventHandler {eventName} does not expect a response but the payload type {payload.GetType().FullName} does! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TRequest>(payload);

                TResponse response;
                try
                {
                    response = await handler(player, obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                    return;
                }

                var responsePayload = JsonConvert.SerializeObject(response);

                BaseScript.TriggerClientEvent(player, eventName + "_response", requestId, responsePayload);
            }

            _fiveEvents.On(eventName, new Func<Player, string, string, Task>(Handle));

            _log.Debug($"Added request handler for {eventName} with response {eventName}_response.");
        }

        public void AddNotificationHandler<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification

        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var handlerWrapper = new Func<string, string, Task>(async (requestId, payload) =>
            {
                if (requestId != null)
                {
                    _log.Error(
                        $"EventHandler for {eventName} expects a response but the payload type {payload.GetType().FullName} doesn't! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TNotification>(payload);

                try
                {
                    await handler(obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                }
            });

            DoAddNotificationHandler<TNotification>(eventName, handler, handlerWrapper);
        }

        public void AddNotificationHandler<TNotification>(Func<Player, TNotification, Task> handler)
            where TNotification : INotification

        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();

            async Task Handle([FromSource] Player player, string requestId, string payload)
            {
                if (requestId != null)
                {
                    _log.Error(
                        $"EventHandler for {eventName} expects a response but the payload type {payload.GetType().FullName} doesn't! This should never happen! See Fish!");
                    return;
                }

                var obj = JsonConvert.DeserializeObject<TNotification>(payload);

                try
                {
                    await handler(player, obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in handler for {eventName}", e);
                }
            }

            var handlerWrapper = new Func<Player, string, string, Task>(Handle);

            DoAddNotificationHandler<TNotification>(eventName, handler, handlerWrapper);
        }

        private void DoAddNotificationHandler<TNotification>(string eventName, Delegate handler,
            Delegate handlerWrapper)
            where TNotification : INotification
        {
            lock (_notificationHandlers)
            {
                if (_notificationHandlers.ContainsKey(handler))
                {
                    return;
                }

                _fiveEvents.On(eventName, handlerWrapper);

                _notificationHandlers.Add(handler, handlerWrapper);
            }
        }

        public void AddNotificationClientForwarder<TNotification>() where TNotification : INotification
        {
            Func<TNotification, Task> handlerWrapper = notification =>
            {
                PublishToClients(notification);
                return Task.FromResult(0);
            };

            lock (_notificationForwarders)
            {
                if (_notificationForwarders.ContainsKey(typeof(TNotification)))
                {
                    return;
                }

                AddNotificationHandler(handlerWrapper);

                _notificationForwarders.Add(typeof(TNotification), handlerWrapper);
            }
        }

        public void RemoveNotificationClientForwarder<TNotification>() where TNotification : INotification
        {
            lock (_notificationForwarders)
            {
                if (!_notificationForwarders.TryGetValue(typeof(TNotification), out var handler))
                {
                    return;
                }

                _notificationForwarders.Add(typeof(TNotification), handler);
            }
        }
    }
}