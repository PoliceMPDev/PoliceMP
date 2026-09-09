using System;
using System.Diagnostics;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Abstraction;
using PoliceMP.Core.Shared.Constants;
using Debug = CitizenFX.Core.Debug;

namespace PoliceMP.Core.Server.Extensions
{
    public static class StateBagExtensions
    {
        public static T Get<T>(this StateBag stateBag, string key)
        {
            try
            {
                var value = stateBag.Get(key);
                if (value == null) return default;
                if (value is T) return value;
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{ConsoleColors.Red}[PoliceMP] [Error] [StateBagExtensions] {e.Message} {ConsoleColors.White}");
                return default;
            }
        }

        public static object Get(this StateBag stateBag, string key, Type type)
        {
            var value = stateBag.Get(key);

            if (value == null) return default;
            if (value.GetType() == type) return value;

            try
            {
                return JsonConvert.DeserializeObject(value, type);
            }
            catch (Exception ex)
            {
                var callStack = new StackTrace();
                var callee = callStack.GetFrame(1);

                Debug.WriteLine($"Failed to convert {key}. Could not deserialize json into <{type.FullName}>. Value \"{stateBag.Get(key).ToString()}\". Callee: {callee.GetType().FullName}.{callee.GetMethod()}", ex);
                return default;
            }
        }


        public static void Set<T>(this StateBag stateBag, string key, T value, bool replicated = true)
        {
            var actualValue = typeof(T) == typeof(string) ? value.ToString() : JsonConvert.SerializeObject(value);
            stateBag.Set(key, actualValue, replicated);
        }

        public static StateBagProxy<T> CreateProxy<T>(this StateBag stateBag, string key, bool replicated = true, T defaultValue = default)
        {
            return new StateBagProxy<T>(stateBag, key, replicated, defaultValue);
        }
    }
}
