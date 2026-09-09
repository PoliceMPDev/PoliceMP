using System;
using System.Linq.Expressions;
using System.Reflection;
using CitizenFX.Core;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Core.Server.Scripts
{

    public sealed class Blackboard<T>
        where T : PedBehaviorDefinition
    {
        private Ped Ped { get; }
        private readonly string _prefix;

        internal Blackboard(Ped ped)
        {
            Ped = ped;
            _prefix = $"bb_{typeof(T).Name}_";
        }

        public static Blackboard<T> Get(Ped ped)
        {
            // TODO: Add behavior check
            return new Blackboard<T>(ped);
        }

        private string GetPrefixedName(string name) => $"{_prefix}{name}";

        public TValue Get<TValue>(Expression<Func<T, TValue>> propertyExpression)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var name = memberExpression.Member.Name;

            return Ped.State.Get<TValue>(GetPrefixedName(name));
        }

        public object Get(PropertyInfo property)
        {
            return Ped.State.Get(GetPrefixedName(property.Name), property.PropertyType);
        }

        /// <summary>
        /// Sets a blackboard value
        /// </summary>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <param name="propertyExpression">Property to change</param>
        /// <param name="value">Value to set</param>
        /// <param name="setAlways">Sets the value even if it has not changed - Causes event handlers to be raised</param>
        public void Set<TValue>(Expression<Func<T, TValue>> propertyExpression, TValue value, bool setAlways = false)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var name = memberExpression.Member.Name;

            if (!setAlways)
            {
                var curState = Ped.State.Get<TValue>(GetPrefixedName(name));

                if (curState == null || curState.Equals(value))
                {
                    return;
                }
            }

            Ped.State.Set(GetPrefixedName(name), value);
        }

        public void ResetData()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var value = property.PropertyType == typeof(string) 
                    ? string.Empty 
                    : Activator.CreateInstance(property.PropertyType);
                
                var name = GetPrefixedName(property.Name);
                Debug.WriteLine($"Resetting bb property: {name} on ped {Ped.Handle} to {value}");
                Ped.State.Set(name, value);
            }
        }
    }
}