using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Core.Shared.Scripts;

namespace PoliceMP.Core.Client.Scripts
{

    public sealed class Blackboard<T>
        where T : PedBehaviorDefinition
    {
        // Ped might change, always check network id
        private int _pedNetworkId;

        public Ped Ped
        {
            get
            {
                if(API.NetworkDoesEntityExistWithNetworkId(_pedNetworkId))
                    return (Ped)Entity.FromNetworkId(_pedNetworkId);

                return null;
            }
        }

        private readonly string _prefix;

        internal Blackboard(Ped ped)
        {
            _pedNetworkId = ped?.NetworkId ?? 0;
            _prefix = $"bb_{typeof(T).Name}_";
        }

        public static Blackboard<T> Create(Ped ped)
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

        public void AddChangeHandler<TValue>(Expression<Func<T, TValue>> propertyExpression, ChangeBagChangeHandler<TValue> handler)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var name = memberExpression.Member.Name;

            // Use the proxy to calculate the bag name
            var prefixedName = GetPrefixedName(name);
            Debug.WriteLine($"Creating change handler for Blackboard key: {prefixedName}");
            var proxy = new EntityStateBagProxy<TValue>(Ped, prefixedName);
            proxy.AddStateBagChangeHandler(handler);
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