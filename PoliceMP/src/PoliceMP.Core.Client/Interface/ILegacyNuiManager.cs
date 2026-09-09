using System;

namespace PoliceMP.Core.Client.Interface
{
    public interface ILegacyNuiManager
    {
        void On(string @event, Action action);
        void Emit(object data);
        void Focus(bool hasFocus, bool showCursor);
        void On<T>(string @event, Action<T> action);
        void On<TReturn>(string @event, Func<TReturn> action);
        void On<T, TReturn>(string @event, Func<T, TReturn> action);
    }
}