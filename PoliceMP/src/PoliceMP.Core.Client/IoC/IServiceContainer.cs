using System;
using System.Collections.Generic;

namespace PoliceMP.Core.Client.IoC
{
    public interface IServiceContainer
    {
        T Get<T>();
        T GetAdhoc<T>();
        T GetRequired<T>();
        T GetAdhocRequired<T>();
        object Get(Type type);
        object GetAdhoc(Type type);
        object GetRequired(Type type);
        object GetAdhocRequired(Type type);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetAllRequired<T>();
        IEnumerable<object> GetAll(Type type);
        IEnumerable<object> GetAllRequired(Type type);
        void ClearCache(Type type);
    }
}