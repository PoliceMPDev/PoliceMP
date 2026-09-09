using System;
using System.Linq;
using System.Reflection;

namespace PoliceMP.Shared.Enums
{
    public static class Extensions
    {
        
        public static T GetCustomAttribute<T>(this Enum enumVal) where T : Attribute
        {
            return (T)enumVal.GetType()
                .GetMember(enumVal.ToString())[0]
                .GetCustomAttribute(typeof(T), false);
        }
    }
}