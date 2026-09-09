using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Communications.Interfaces
{
    [Obsolete("Use the new client communications manager!")]
    public interface ILegacyClientCommunicationsManager
    {
        void ToServer(string @event, params object[] args);
        void ToServerRaw(string @event, params object[] args);

        void ToClient(string @event, params object[] args);
        void ToClientRaw(string @event, params object[] args);

        Task<T> Request<T>(string @event, params object[] args);

        void OnRequest<TIn, TReturn>(string @event, Func<TIn, Task<TReturn>> handler);

        void On(string @event, Action handler);

        void On<T>(string @event, Action<T> handler);

        void On<T1, T2>(string @event, Action<T1, T2> handler);

        void On<T1, T2, T3>(string @event, Action<T1, T2, T3> handler);

        void On<T1, T2, T3, T4>(string @event, Action<T1, T2, T3, T4> handler);

        void On<T1, T2, T3, T4, T5>(string @event, Action<T1, T2, T3, T4, T5> handler);

        void On<T1, T2, T3, T4, T5, T6>(string @event, Action<T1, T2, T3, T4, T5, T6> handler);

        void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<T1, T2, T3, T4, T5, T6, T7> handler);

        void On(string @event, Func<Task> handler);

        void On<T>(string @event, Func<T, Task> handler);

        void On<T1, T2>(string @event, Func<T1, T2, Task> handler);

        void On<T1, T2, T3>(string @event, Func<T1, T2, T3, Task> handler);

        void On<T1, T2, T3, T4>(string @event, Func<T1, T2, T3, T4, Task> handler);

        void On<T1, T2, T3, T4, T5>(string @event, Func<T1, T2, T3, T4, T5, Task> handler);

        void On<T1, T2, T3, T4, T5, T6>(string @event, Func<T1, T2, T3, T4, T5, T6, Task> handler);

        void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Func<T1, T2, T3, T4, T5, T6, T7, Task> handler);
    }
}