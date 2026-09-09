using System;

namespace PoliceMP.Core.Client.IoC
{
    public interface IServiceContainerBuilder
    {
        IServiceContainerBuilder Add<T>();
        IServiceContainerBuilder Add<T>(T instance);
        IServiceContainerBuilder Add<T>(Func<IServiceContainer, T> factoryMethod);
        IServiceContainerBuilder Add<T, TImplementation>() where TImplementation : T;
        IServiceContainerBuilder Add(Type type);
        IServiceContainerBuilder Add(Type type, Type implementationType);
        IServiceContainerBuilder Add(Type type, object instance);
        IServiceContainerBuilder Add(Type type, Func<IServiceContainer, object> factoryMethod);
        IServiceContainer Build();
    }
}