using System;

namespace PoliceMP.Core.Shared.Extensions
{
    public static class DelegateExtensions
    {
        public static string GetMethodDescription(this Delegate @delegate) =>
            $"{@delegate.Method.DeclaringType?.Name}.{@delegate.Method.Name}";
    }
}