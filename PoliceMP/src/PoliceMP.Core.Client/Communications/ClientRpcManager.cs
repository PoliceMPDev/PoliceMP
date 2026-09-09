using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications;
using PoliceMP.Core.Shared.Communications.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Constants;
using CitizenFX.Core.NaturalMotion;

namespace PoliceMP.Core.Client.Communications
{
    public class ClientRpcManager : IClientRpcManager
    {
        private readonly ILogger<ClientRpcManager> _logger;
        private readonly IFiveEventManager _fiveEvents;
        private readonly IClientEventManager _events;

        public ClientRpcManager(ILogger<ClientRpcManager> logger,
            IFiveEventManager fiveEvents,
            IClientEventManager events)
        {
            _logger = logger;
            _fiveEvents = fiveEvents;
            _events = events;
        }

        public void Emit(string @event, params object[] args)
        {
            _logger.Trace($"Emitting {@event} to server.");
            var message = new RpcMessage
            {
                Id = Guid.NewGuid(),
                Event = @event,
                Payload = args.Select(JsonConvert.SerializeObject).ToArray()
            };

            EmitRaw(@event, JsonConvert.SerializeObject(message));
        }

        public void EmitRaw(string @event, params object[] args)
        {
            BaseScript.TriggerServerEvent(RpcConstants.RpcMessage, args);
        }

        public async Task HandleMessage(string json)
        {
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
                var result = await _events.Emit(rpcMessage);

                if (result == null) return;
                if (string.IsNullOrEmpty(rpcMessage.ReplyEvent)) return;

                rpcMessage.Payload = new[] { JsonConvert.SerializeObject(result) };

                _logger.Trace($"Returning RpcMessage {rpcMessage.Id} back to server on {rpcMessage.ReplyEvent}");
                BaseScript.TriggerServerEvent(rpcMessage.ReplyEvent, JsonConvert.SerializeObject(rpcMessage));
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occured inside rpc {rpcMessage.Id} {rpcMessage.Event}");
            }
        }

        public async Task<T> Request<T>(string @event, params object[] args)
        {
            Guid id = Guid.NewGuid();
            string replyEvent = $"Request:{id}";
            var rpcMessage = new RpcMessage
            {
                Id = id,
                Event = @event,
                ReplyEvent = replyEvent,
                Payload = args.Select(JsonConvert.SerializeObject).ToArray()
            };

            var tcs = new TaskCompletionSource<RpcMessage>();
            var callback = new Action<string>(data =>
            {
                var cbRpcMessage = JsonConvert.DeserializeObject<RpcMessage>(data);
                _logger.Trace($"Received RpcMessage Reply {cbRpcMessage.Id} from server on {cbRpcMessage.Event}.");
                tcs.SetResult(cbRpcMessage);
            });

            // TODO: Add a timeout
            _fiveEvents.On(replyEvent, callback);
            try
            {
                _logger.Trace($"Sending RpcMessage Request {rpcMessage.Id} to server on {rpcMessage.Event} which replies to {rpcMessage.ReplyEvent}");
                BaseScript.TriggerServerEvent(RpcConstants.RpcMessage, JsonConvert.SerializeObject(rpcMessage));
                var result = await tcs.Task;
                return JsonConvert.DeserializeObject<T>(result.Payload[0]);
            }
            finally
            {
                _fiveEvents.Off(replyEvent, callback);
            }
        }
    }
}