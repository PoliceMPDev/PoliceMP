using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PoliceMP.Core.Client.IoC
{
    public class ServiceContainerBuilder : IServiceContainerBuilder
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public IServiceContainerBuilder Add<T>()
            => Add(typeof(T));

        public IServiceContainerBuilder Add<T>(T instance)
            => Add(typeof(T), instance);

        public IServiceContainerBuilder Add<T>(Func<IServiceContainer, T> factoryMethod)
            => Add(typeof(T), factoryMethod);

        public IServiceContainerBuilder Add<T, TImplementation>() where TImplementation : T
            => Add(typeof(T), typeof(TImplementation));

        public IServiceContainerBuilder Add(Type type)
        {
            AddInternal(type, type);
            return this;
        }

        public IServiceContainerBuilder Add(Type type, Type implementationType)
            => Add(type, (object)implementationType);

        public IServiceContainerBuilder Add(Type type, object instance)
        {
            AddInternal(type, instance);
            return this;
        }

        public IServiceContainerBuilder Add(Type type, Func<IServiceContainer, object> factoryMethod)
            => Add(type, (object)factoryMethod);

        IServiceContainer IServiceContainerBuilder.Build()
        {
            return Build();
        }

        private void AddInternal(Type type, object instance)
        {
            _instances.Add(type, instance);
        }

        public IServiceContainer Build()
        {
            var locator = new ServiceContainer(_instances);
            return locator;
        }
    }
}