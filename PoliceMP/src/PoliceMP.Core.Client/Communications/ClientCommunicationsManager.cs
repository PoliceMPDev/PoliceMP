using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;

namespace PoliceMP.Core.Client.Communications
{
    /// <summary>
    /// Mediates client and server communications for client
    /// </summary>
    public class ClientCommunicationsManager : IClientCommunicationsManager
    {
        private readonly ILogger<ClientCommunicationsManager> _log;
        private readonly IFiveEventManager _fiveEvents;
        private readonly HashSet<string> _requestHandlers = new HashSet<string>();

        public ClientCommunicationsManager(ILogger<ClientCommunicationsManager> log, IFiveEventManager fiveEvents)
        {
            _log = log;
            _fiveEvents = fiveEvents;
        }

        public void SendToServer<TRequest>(TRequest request) where TRequest : IServerRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();
            var payload = JsonConvert.SerializeObject(request);

            BaseScript.TriggerServerEvent(eventName, null, payload);
        }

        public async Task<TResponse> SendToServer<TResponse>(IServerRequest<TResponse> request, int timeoutMs = 10000)
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
                BaseScript.TriggerServerEvent(eventName, requestId, payload);

                if (timeoutMs > 0)
                {
                    await Task.WhenAny(tcs.Task, BaseScript.Delay(timeoutMs));
                    if (!tcs.Task.IsCompleted)
                    {
                        _log.Error($"Message {eventName} failed to get a response in the timeout time of {timeoutMs}");
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

        public void SendToClient<TRequest>(TRequest request) where TRequest : IClientRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();
            var payload = JsonConvert.SerializeObject(request);

            BaseScript.TriggerEvent(eventName, null, payload);
        }

        public async Task<TResponse> SendToClient<TResponse>(IClientRequest<TResponse> request, int timeoutMs = 10000)
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
                BaseScript.TriggerEvent(eventName, requestId, payload);

                if (timeoutMs > 0)
                {
                    await Task.WhenAny(tcs.Task, BaseScript.Delay(timeoutMs));
                    if (!tcs.Task.IsCompleted)
                    {
                        _log.Error(
                            $"Message {eventName} failed to get a response in the timeout time of {timeoutMs}");
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

        public void PublishToAll<TNotification>(TNotification notification) where TNotification : INotification
        {
            PublishToClient(notification);
            PublishToServer(notification);
        }

        public void PublishToServer<TNotification>(TNotification notification) where TNotification : INotification
        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var payload = JsonConvert.SerializeObject(notification);

            BaseScript.TriggerServerEvent(eventName, null, payload);
        }

        public void PublishToClient<TNotification>(TNotification notification) where TNotification : INotification
        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            var payload = JsonConvert.SerializeObject(notification);

            BaseScript.TriggerEvent(eventName, null, payload);
        }

        public void AddRequestHandler<TRequest>(Action handler) where TRequest : IClientRequest
        {
            AddRequestHandler<TRequest>(_ => handler());
        }

        public void AddRequestHandler<TRequest>(Action<TRequest> handler) where TRequest : IClientRequest
        {
            AddRequestHandler<TRequest>(request =>
            {
                handler(request);
                return Task.FromResult(0);
            });
        }

        public void AddRequestHandler<TRequest>(Func<Task> handler) where TRequest : IClientRequest
        {
            AddRequestHandler<TRequest>(_ => handler());
        }

        public void AddRequestHandler<TRequest>(Func<TRequest, Task> handler) where TRequest : IClientRequest
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists! {eventName}");
                    return;
                }
            }

            _fiveEvents.On(eventName, new Func<string, string, Task>(async (requestId, payload) =>
            {
                if (requestId != null)
                {
                    _log.Error(
                        $"EventHandler {eventName} expects a response but the payload type {payload.GetType().FullName} doesn't! This should never happen! See Fish!");
                }

                var obj = JsonConvert.DeserializeObject<TRequest>(payload);

                try
                {
                    await handler(obj);
                }
                catch (Exception e)
                {
                    _log.Error($"Error in request handler for {eventName}", e);
                }
            }));

            _log.Debug($"Added request handler for {eventName} with no response.");
        }

        public void AddRequestHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
            where TRequest : IClientRequest<TResponse>
        {
            AddRequestHandler<TRequest, TResponse>(request => Task.FromResult(handler(request)));
        }

        public void AddRequestHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
            where TRequest : IClientRequest<TResponse>
        {
            var eventName = MediatorUtils.GetEventNameForType<TRequest>();

            // Only one handler per IRequest
            lock (_requestHandlers)
            {
                if (!_requestHandlers.Add(eventName))
                {
                    _log.Error($"EventHandler for request already exists! {eventName}");
                    return;
                }
            }

            _fiveEvents.On(eventName, new Func<string, string, Task>(async (requestId, payload) =>
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
                    _log.Error($"Error in request handler for {eventName}", e);
                    return;
                }

                var responsePayload = JsonConvert.SerializeObject(response);

                //TODO: Track when request is from client/server and only respond to the right place
                BaseScript.TriggerServerEvent(eventName + "_response", requestId, responsePayload);
                BaseScript.TriggerEvent(eventName + "_response", requestId, responsePayload);
            }));

            _log.Debug($"Added request handler for {eventName} with response {eventName}_response.");
        }

        public void AddNotificationHandler<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification
        {
            var eventName = MediatorUtils.GetEventNameForType<TNotification>();
            _log.Debug($"Binding notification {typeof(TNotification).Name} to handler in {handler.Target}");
            _fiveEvents.On(eventName, new Func<string, string, Task>(async (requestId, payload) =>
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
                    _log.Error($"Error in notification handler for {eventName}", e);
                }
            }));
        }
    }
}