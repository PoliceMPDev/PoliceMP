using CitizenFX.Core;
using PoliceMP.Core.Shared.Communications;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Core.Server.Communications.Interfaces
{
    [Obsolete("Use the new server communications manager!")]
    public interface ILegacyServerCommunicationsManager
    {
        void ToServer(string @event, params object[] args);
        Task ToClient(Player player, RpcMessage rpcMessage);
        void ToClient(string @event, params object[] args);
        void ToClient(Player player, string @event, params object[] args);
        Task<T> Request<T>(Player player, string @event, params object[] args);

        void OnRequest<TReturn>(string @event, Func<Task<TReturn>> handler);
        void OnRequest<TIn, TReturn>(string @event, Func<TIn, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TReturn>(string @event, Func<TIn1, TIn2, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TReturn>(string @event, Func<TIn1, TIn2, TIn3, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, TIn5, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TReturn>(string @event, Func<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, Task<TReturn>> handler);

        void OnRequest<TReturn>(string @event, Func<Player, Task<TReturn>> handler);
        void OnRequest<TIn, TReturn>(string @event, Func<Player, TIn, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TReturn>(string @event, Func<Player, TIn1, TIn2, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, TIn5, Task<TReturn>> handler);
        void OnRequest<TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, TReturn>(string @event, Func<Player, TIn1, TIn2, TIn3, TIn4, TIn5, TIn6, Task<TReturn>> handler);

        void On(string @event, Func< Task> handler);
        void On<T>(string @event, Func<T, Task> handler);
        void On<T1, T2>(string @event, Func<T1, T2, Task> handler);
        void On<T1, T2, T3>(string @event, Func<T1, T2, T3, Task> handler);
        void On<T1, T2, T3, T4>(string @event, Func<T1, T2, T3, T4, Task> handler);
        void On<T1, T2, T3, T4, T5>(string @event, Func<T1, T2, T3, T4, T5, Task> handler);
        void On<T1, T2, T3, T4, T5, T6>(string @event, Func<T1, T2, T3, T4, T5, T6, Task> handler);

        void On(string @event, Action handler);
        void On<T>(string @event, Action<T> handler);
        void On<T1, T2>(string @event, Action<T1, T2> handler);
        void On<T1, T2, T3>(string @event, Action<T1, T2, T3> handler);
        void On<T1, T2, T3, T4>(string @event, Action<T1, T2, T3, T4> handler);
        void On<T1, T2, T3, T4, T5>(string @event, Action<T1, T2, T3, T4, T5> handler);
        void On<T1, T2, T3, T4, T5, T6>(string @event, Action<T1, T2, T3, T4, T5, T6> handler);
        void On<T1, T2, T3, T4, T5, T6, T7>(string @event, Action<T1, T2, T3, T4, T5, T6, T7> handler);

        void On(string @event, Action<Player> handler);
        void On<T>(string @event, Action<Player, T> handler);
        void On<T1, T2>(string @event, Action<Player, T1, T2> handler);
        void On<T1, T2, T3>(string @event, Action<Player, T1, T2, T3> handler);
        void On<T1, T2, T3, T4>(string @event, Action<Player, T1, T2, T3, T4> handler);
        void On<T1, T2, T3, T4, T5>(string @event, Action<Player, T1, T2, T3, T4, T5> handler);
        void On<T1, T2, T3, T4, T5, T6>(string @event, Action<Player, T1, T2, T3, T4, T5, T6> handler);

        void On(string @event, Func<Player, Task> handler);
        void On<T>(string @event, Func<Player, T, Task> handler);
        void On<T1, T2>(string @event, Func<Player, T1, T2, Task> handler);
        void On<T1, T2, T3>(string @event, Func<Player, T1, T2, T3, Task> handler);
        void On<T1, T2, T3, T4>(string @event, Func<Player, T1, T2, T3, T4, Task> handler);
        void On<T1, T2, T3, T4, T5>(string @event, Func<Player, T1, T2, T3, T4, T5, Task> handler);
        void On<T1, T2, T3, T4, T5, T6>(string @event, Func<Player, T1, T2, T3, T4, T5, T6, Task> handler);
    }
}