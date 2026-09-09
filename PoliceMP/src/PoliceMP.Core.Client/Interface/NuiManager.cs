using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Interface
{
    public class NuiManager : INuiManager
    {
        private readonly EventHandlerDictionary _eventHandlerDictionary;
        private readonly ILogger<NuiManager> _logger;

        public NuiManager(EventHandlerDictionary eventHandlerDictionary, ILogger<NuiManager> logger)
        {
            _eventHandlerDictionary = eventHandlerDictionary;
            _logger = logger;
        }

        public void On(string eventName, Action action)
        {
            API.RegisterNuiCallbackType(eventName.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{eventName.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                action();
                callback("{}");
            });

            _logger.Trace($"[NUI] {eventName.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<T>(string eventName, Action<T> action)
        {
            API.RegisterNuiCallbackType(eventName.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{eventName.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                T typedData = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));

                action(typedData);

                callback("{}");
            });

            _logger.Trace($"[NUI] {eventName.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<TReturn>(string eventName, Func<TReturn> action)
        {
            API.RegisterNuiCallbackType(eventName.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{eventName.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                var result = action();

                callback(JsonConvert.SerializeObject(result));
            });

            _logger.Trace($"[NUI] {eventName.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void On<T, TReturn>(string eventName, Func<T, TReturn> action)
        {
            API.RegisterNuiCallbackType(eventName.ToLower());

            _eventHandlerDictionary[$"__cfx_nui:{eventName.ToLower()}"] += new Action<dynamic, CallbackDelegate>((data, callback) =>
            {
                var typedData = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));

                var result = action(typedData);

                if (typeof(TReturn) == typeof(string)
                    || typeof(TReturn) == typeof(int)
                    || typeof(TReturn) == typeof(long)
                    || typeof(TReturn) == typeof(bool)
                    || typeof(TReturn) == typeof(float)
                    || typeof(TReturn) == typeof(double))
                {
                    callback(result);
                }
                else
                {
                    callback(JsonConvert.SerializeObject(result));
                }
            });

            _logger.Trace($"[NUI] {eventName.ToLower()} attached to {action.Method.DeclaringType?.Name}.{action.Method.Name}");
        }

        public void Emit(string eventName, object message)
        {
            _logger.Trace($"[NUI] Emiting NUI message: {eventName}({message})");

            var nuiMessage = new
            {
                EventName = $"policemp:{eventName}",
                Message = message
            };

            API.SendNuiMessage(JsonConvert.SerializeObject(nuiMessage));
        }

        public void Focus(bool hasFocus, bool showCursor)
        {
            _logger.Trace($"[NUI] Focus called (hasFocus: {hasFocus} | showCursor: {showCursor})");
            API.SetNuiFocus(hasFocus, showCursor);
        }
    }
}
