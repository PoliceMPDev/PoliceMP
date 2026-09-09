using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Interface
{
    public class LegacyNuiManager : ILegacyNuiManager
    {
        private readonly EventHandlerDictionary _eventHandlerDictionary;
        private readonly ILogger<NuiManager> _logger;

        public LegacyNuiManager(EventHandlerDictionary eventHandlerDictionary, ILogger<NuiManager> logger)
        {
            _eventHandlerDictionary = eventHandlerDictionary;
            _logger = logger;
        }

        public void On(string @event, Action action)
        {
            API.RegisterNuiCallbackType(@event.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{@event.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                action();
                callback("{}");
            });

            _logger.Trace($"[NUI] {@event.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<T>(string @event, Action<T> action)
        {
            API.RegisterNuiCallbackType(@event.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{@event.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                T typedData = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));

                action(typedData);

                callback("{}");
            });

            _logger.Trace($"[NUI] {@event.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<TReturn>(string @event, Func<TReturn> action)
        {
            API.RegisterNuiCallbackType(@event.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{@event.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                var result = action();

                callback(JsonConvert.SerializeObject(result));
            });

            _logger.Trace($"[NUI] {@event.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<T, TReturn>(string @event, Func<T, TReturn> action)
        {
            API.RegisterNuiCallbackType(@event.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{@event.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                var typedData = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));

                var result = action(typedData);

                callback(JsonConvert.SerializeObject(result));
            });

            _logger.Trace($"[NUI] {@event.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void Emit(object data)
        {
            _logger.Trace($"[NUI] Emiting NUI message: {data}");
            API.SendNuiMessage(JsonConvert.SerializeObject(data));
        }

        public void Focus(bool hasFocus, bool showCursor)
        {
            _logger.Trace($"[NUI] Focus called (hasFocus: {hasFocus} | showCursor: {showCursor})");
            API.SetNuiFocus(hasFocus, showCursor);
        }
    }
}