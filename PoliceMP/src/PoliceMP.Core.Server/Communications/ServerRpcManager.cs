using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications;
using PoliceMP.Core.Shared.Communications.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Constants;
using System.Threading;

namespace PoliceMP.Core.Server.Communications
{
    public class ServerRpcManager : IServerRpcManager
    {
        private readonly ILogger<ServerRpcManager> _logger;
        private readonly IFiveEventManager _fiveEvents;
        private readonly IServerEventManager _events;

        private readonly ConcurrentDictionary<Guid, string> _pendingCommands = new();

        public ServerRpcManager(ILogger<ServerRpcManager> logger,
            IFiveEventManager fiveEvents,
            IServerEventManager events)
        {
            _logger = logger;
            _fiveEvents = fiveEvents;
            _events = events;

            Timer timer = new Timer(TimerCallback, null, 0, 10000);
        }

        private void TimerCallback(Object o)
        {
            if (_pendingCommands.Count == 0) return;
            
            _logger.Debug($"Current pending commands: {_pendingCommands.Count}");
            foreach (var command in _pendingCommands)
            {
                _logger.Debug($"Pending command: {command.Value}");
            }
        }

        public async Task Emit(Player player, string @event, params object[] args)
        {
            Guid guid = Guid.NewGuid();
            _pendingCommands.TryAdd(guid, @event);
            try
            {
                _logger.Trace($"Emitting {@event} to client {player.Name}.");
                var message = new RpcMessage
                {
                    Id = guid,
                    Event = @event,
                    Payload = args.Select(JsonConvert.SerializeObject).ToArray()
                };

                _logger.Trace(JsonConvert.SerializeObject(message));
                player.TriggerEvent(RpcConstants.RpcMessage, JsonConvert.SerializeObject(message));
            }
            catch (Exception e)
            {
                _logger.Error($"ERROR OCCURED WITH RPC EVENT PLAYER: {player} {@event} {e}");
            }
            finally
            {
                _pendingCommands.TryRemove(guid, out var _);
            }
        }

        public async Task Emit(string @event, params object[] args)
        {
            Guid guid = Guid.NewGuid();
            _pendingCommands.TryAdd(guid, @event);
            try
            {
                _logger.Trace($"Emitting {@event} to all clients.");
                var message = new RpcMessage
                {
                    Id = guid,
                    Event = @event,
                    Payload = args.Select(JsonConvert.SerializeObject).ToArray()
                };

                BaseScript.TriggerClientEvent(RpcConstants.RpcMessage, JsonConvert.SerializeObject(message));
            }
            catch (Exception e)
            {
                _logger.Error($"ERROR OCCURED WITH RPC EVENT TO ALL PLAYERS: {@event} {e}");
            }
            finally
            {
                _pendingCommands.TryRemove(guid, out var _);
            }
        }

        public async Task HandleMessage(Player player, string json)
        {
            // Create a CancellationTokenSource with a 10-second timeout
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            RpcMessage rpcMessage;
            try
            {
                rpcMessage = JsonConvert.DeserializeObject<RpcMessage>(json);
            }
            catch (Exception e)
            {
                _logger.Error($"Failed to deserialize RpcMessage: {e}");
                return;
            }
            _logger.Trace($"Deserialized RpcMessage {rpcMessage.Id} for event {rpcMessage.Event}");

            try
            {
                var resultTask = _events.Emit(player, rpcMessage);

                // Wait for the result or timeout, whichever comes first
                var result = await Task.WhenAny(resultTask, Task.Delay(Timeout.Infinite, cancellationTokenSource.Token));

                if (result != resultTask)
                {
                    // The operation timed out
                    _logger.Warn($"Operation timed out after 10 seconds. {player.Name} - {rpcMessage.Event}");
                    return;
                }
                if (string.IsNullOrEmpty(rpcMessage.ReplyEvent))
                {
                    return;
                }
                rpcMessage.Payload = new[] { JsonConvert.SerializeObject(resultTask.Result) };

                _logger.Trace($"Returning RpcMessage {rpcMessage.Id} back to client {player.Name} on {rpcMessage.ReplyEvent}");
                player.TriggerEvent(rpcMessage.ReplyEvent, JsonConvert.SerializeObject(rpcMessage));
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation due to timeout
                _logger.Warn("Operation canceled due to timeout.");
            }
        }

        public async Task<T> Request<T>(Player player, string @event, params object[] args)
        {
            Guid id = Guid.NewGuid();
            _pendingCommands.TryAdd(id, @event);
            string replyEvent = $"Request:{id}";
            var rpcMessage = new RpcMessage
            {
                Id = id,
                Event = @event,
                Payload = args.Select(JsonConvert.SerializeObject).ToArray(),
                ReplyEvent = replyEvent
            };

            var tcs = new TaskCompletionSource<RpcMessage>();
            var callback = new Action<string>(data =>
            {
                var cbRpcMessage = JsonConvert.DeserializeObject<RpcMessage>(data);
                _logger.Trace($"Received RpcMessage Reply {cbRpcMessage.Id} from client {player.Name} on {cbRpcMessage.Event}.");
                tcs.SetResult(cbRpcMessage);
            });

            _fiveEvents.On(replyEvent, callback);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // 10-second timeout

            try
            {
                _logger.Trace($"Sending RpcMessage Request {rpcMessage.Id} to server on {rpcMessage.Event} which replies to {rpcMessage.ReplyEvent}");
                player.TriggerEvent(RpcConstants.RpcMessage, JsonConvert.SerializeObject(rpcMessage));

                // Wait for the result or timeout, whichever comes first
                var result = await Task.WhenAny(tcs.Task, Task.Delay(Timeout.Infinite, cancellationTokenSource.Token));

                if (result != tcs.Task)
                {
                    // The operation timed out
                    _logger.Warn("Operation timed out after 10 seconds.");
                    return default(T); // You can return a default value or throw an exception here
                }

                var response = await tcs.Task;
                return JsonConvert.DeserializeObject<T>(response.Payload[0]);
            }
            finally
            {
                _fiveEvents.Off(replyEvent, callback);
                _pendingCommands.TryRemove(id, out var _);
            }
        }
    }
}