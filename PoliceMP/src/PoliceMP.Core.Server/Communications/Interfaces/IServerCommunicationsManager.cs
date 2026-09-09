using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Mediator;

namespace PoliceMP.Core.Server.Communications.Interfaces
{
    /// <summary>
    /// Mediates messages to server/client
    /// </summary>
    public interface IServerCommunicationsManager
    {
        /// <summary>
        /// Sends a fire and forget message to the server ensuring only one handler handles the request.
        /// For multiple handlers use <see cref="PublishToServer"/>
        /// </summary>
        /// <param name="request"></param>
        void SendToClients<TRequest>(TRequest request) where TRequest : IClientRequest;

        /// <summary>
        /// Sends a fire and forget message to the server ensuring only one handler handles the request.
        /// For multiple handlers use <see cref="PublishToServer"/>
        /// </summary>
        /// <param name="player">The player to send the request to</param>
        /// <param name="request"></param>
        void SendToClient<TRequest>(Player player, TRequest request) where TRequest : IClientRequest;

        /// <summary>
        /// Sends an event to the server and waits for a response
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="player">The player to send the request to</param>
        /// <param name="request">Request to send</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns></returns>
        Task<TResponse> SendToClient<TResponse>(Player player, IClientRequest<TResponse> request,
            int timeoutMs = 10000);


        /// <summary>
        /// Publishes an event to both client and server. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification">event to emit</param>
        void PublishAll<TNotification>(TNotification notification)
            where TNotification : INotification;

        /// <summary>
        /// Publishes an event to just the server. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification">event to emit</param>
        void PublishToServer<TNotification>(TNotification notification)
            where TNotification : INotification;

        /// <summary>
        /// Publishes an event to all clients. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="notification"></param>
        void PublishToClients<TNotification>(TNotification notification)
            where TNotification : INotification;

        /// <summary>
        /// Publishes an event to just one client. Events should be past tense ie. "ThisThingJustHappenedEvent"
        /// </summary>
        /// <param name="player">The player to send the notification to</param>
        /// <param name="notification"></param>
        void PublishToClient<TNotification>(Player player, TNotification notification)
            where TNotification : INotification;


        /// <summary>
        /// Adds a handler for a request with no response
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Func<TRequest, Task> handler)
            where TRequest : IServerRequest;

        /// <summary>
        /// Adds a handler for a request with no response with player
        /// </summary>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest>(Func<Player, TRequest, Task> handler)
            where TRequest : IServerRequest;


        /// <summary>
        /// Adds a handler for a request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
            where TRequest : IServerRequest<TResponse>;

        /// <summary>
        /// Adds a handler for a request with the player
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="handler"></param>
        void AddRequestHandler<TRequest, TResponse>(Func<Player, TRequest, Task<TResponse>> handler)
            where TRequest : IServerRequest<TResponse>;

        /// <summary>
        /// Adds a handler for a notification
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <param name="handler"></param>
        void AddNotificationHandler<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification;

        /// <summary>
        /// Adds a handler for a notification
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        /// <param name="handler"></param>
        void AddNotificationHandler<TNotification>(Func<Player, TNotification, Task> handler)
            where TNotification : INotification;

        /// <summary>
        /// Forwards all notifications of a specific type to all clients when it's received from a client
        /// </summary>
        /// <typeparam name="TNotification"></typeparam>
        void AddNotificationClientForwarder<TNotification>()
            where TNotification : INotification;

        void RemoveNotificationClientForwarder<TNotification>()
            where TNotification : INotification;
    }
}