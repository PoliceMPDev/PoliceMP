using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Extensions
{
    public delegate Task AsyncStateBagChangeHandlerDelegate<in T>(string bag, string key, T newValue, bool replicated);

    public class StateBagExtensionLog {}
    public static class StateBagExtensions
    {
        private static readonly ILogger Log = new Logger<StateBagExtensionLog>();

        public static T Get<T>(this StateBag stateBag, string key)
        {
            var value = stateBag.Get(key);
            if (value == null)
            {
                return default;
            }
            
            if (value is T tValue)
            {
                return tValue;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                var callStack = new StackTrace();
                var callee = callStack.GetFrame(1);
                
                Log.Error($"Failed to convert {key}. Could not deserialize json into <{typeof(T).FullName}>. Value \"{stateBag.Get(key).ToString()}\". Callee: {callee.GetType().FullName}.{callee.GetMethod()}", ex);
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
                
                Log.Error($"Failed to convert {key}. Could not deserialize json into <{type.FullName}>. Value \"{stateBag.Get(key).ToString()}\". Callee: {callee.GetType().FullName}.{callee.GetMethod()}", ex);
                return default;
            }
        }

        public static void Set<T>(this StateBag stateBag, string key, T value, bool replicated = true)
        {
            var actualValue = typeof(T) == typeof(string) ? value.ToString() : JsonConvert.SerializeObject(value);
            stateBag.Set(key, actualValue, replicated);
        }
    }
}