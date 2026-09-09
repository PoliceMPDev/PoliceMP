using System;

namespace PoliceMP.Core.Client.Interface
{
    public interface INuiManager
    {
        void On(string eventName, Action action);
        void Emit(string eventName, object data);
        void Focus(bool hasFocus, bool showCursor);
        void On<T>(string eventName, Action<T> action);
        void On<TReturn>(string eventName, Func<TReturn> action);
        void On<T, TReturn>(string eventName, Func<T, TReturn> action);
    }
}
