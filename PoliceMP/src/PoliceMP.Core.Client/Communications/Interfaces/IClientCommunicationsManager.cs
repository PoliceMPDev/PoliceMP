using System;
using System.Threading.Tasks;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared.Communications;

namespace PoliceMP.Core.Client.Communications.Interfaces
{
    /// <summary>
    /// Mediates messages to server/client
    /// </summary>
    public interface IClientCommunicationsManager
    {
        /// <summary>
        /// Sends a fire and forget message to the server ensuring only one handler handles the request.
        /// For multiple handlers use <see cref="PublishToServer"/>
        /// </summary>
        /// <param name="request"></param>
        void SendToServer<TRequest>(TRequest request) where TRequest : IServerRequest;

        /// <summary>
        /// Sends an event to the server and waits for a response
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="request">Request to send</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns></returns>
        Task<TResponse> SendToServer<TResponse>(IServerRequest<TResponse> request, int timeoutMs = 10000);
        
        /// <summary>
        /// Sends a fire and forget message to the client ensuring only one handler handles the request.
        /// For multiple handlers use <see cref="PublishToClient"/>
        /// </summary>
        /// <param name="request"></param>
        void SendToClient<TRequest>(TRequest request) where TRequest : IClientRequest;
        
        /// <summary>
        /// Sends an event to the client and waits for a response
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="request">Request to send</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns></returns>
        Task<TResponse> SendToClient<TResponse>(IClientRequest<TResponse> request, int timeoutMs = 10000);

        /// <summary>
        /// Publishes an event to both client and server. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification">event to emit</param>
        void PublishToAll<TNotification>(TNotification notification)
            where TNotification : INotification;

        /// <summary>
        /// Publishes an event to just the server. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification">event to emit</param>
        void PublishToServer<TNotification>(TNotification notification)
            where TNotification : INotification;

        /// <summary>
        /// Publishes an event to just the client. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification"></param>
        void PublishToClient<TNotification>(TNotification notification)
            where TNotification : INotification;
        
        /// <summary>
        /// Adds a handler for a request with no response
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Action handler)
            where TRequest : IClientRequest;
    
        /// <summary>
        /// Adds a handler for a request with no response
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Action<TRequest> handler)
            where TRequest : IClientRequest;
        
        /// <summary>
        /// Adds a handler for a request with no response
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Func<Task> handler)
            where TRequest : IClientRequest;
        
        /// <summary>
        /// Adds a handler for a request with no response
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Func<TRequest, Task> handler)
            where TRequest : IClientRequest;
        
        /// <summary>
        /// Adds a handler for a request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest, TResponse>(Func<TRequest, TResponse> handler)
            where TRequest : IClientRequest<TResponse>;

        /// <summary>
        /// Adds a handler for a request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
            where TRequest : IClientRequest<TResponse>;

        /// <summary>
        /// Adds a handler for a notification
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <param name="handler"></param>
        void AddNotificationHandler<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification;
    }
}
