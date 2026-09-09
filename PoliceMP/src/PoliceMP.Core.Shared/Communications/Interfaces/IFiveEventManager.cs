using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Shared.Communications.Interfaces
{
    public interface IFiveEventManager
    {
        void On(string @event, Delegate callback);
        void On(string @event, Func<Task> callback);
        void On<T>(string @event, Func<T, Task> callback);
        void On<T1, T2>(string @event, Func<T1, T2, Task> callback);
        void On<T1, T2, T3>(string @event, Func<T1, T2, T3, Task> callback);
        void On<T1, T2, T3, T4>(string @event, Func<T1, T2, T3, T4, Task> callback);
        void On<T1, T2, T3, T4, T5>(string @event, Func<T1, T2, T3, T4, T5, Task> callback);
        void Off(string @event, Delegate callback);
    }
}