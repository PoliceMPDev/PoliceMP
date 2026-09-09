using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using PoliceMP.Core.Client.Extensions;

namespace PoliceMP.Core.Mediator
{
    public static class MediatorUtils
    {
        private static ConcurrentDictionary<Type, string> _eventNameTypeCache = new ConcurrentDictionary<Type, string>();

        public static string GetEventNameForType<TRequest>()
            => GetEventNameForType(typeof(TRequest));

        public static string GetEventNameForType(IBaseRequest request)
            => GetEventNameForType(request.GetType());

        public static string GetEventNameForType(Type type)
        {
            if (_eventNameTypeCache.TryGetValue(type, out var name)) 
                return name;

            name = $"{type.Namespace?.Replace("PoliceMP.Shared.", "PMP::")?.Replace("NetworkMessages.", string.Empty)}.{type.Name}";
            _eventNameTypeCache.TryAdd(type, name);

            return name;
        }
    }
}