using System;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Interface
{
    public abstract class LegacyOverlay
    {
        public string Name => GetType().Name;

        private readonly ILegacyNuiManager _nuiManager;
        protected readonly ILogger<LegacyOverlay> _logger;

        public LegacyOverlay(ILegacyNuiManager nuiManager, ILogger<LegacyOverlay> logger)
        {
            _nuiManager = nuiManager;
            _logger = logger;

            On("policemp:ready", OnReady);
            Emit("policemp:load", new
            {
                overlay = Name,
                file = $"{Name}.html"
            });
        }

        public virtual void Show()
        {
            //_logger.Trace($"[NUI] Showing overlay {Name}");
            Emit("policemp:visible", true);
        }

        public virtual void Hide()
        {
            //_logger.Trace($"[NUI] Hiding overlay {Name}");
            Emit("policemp:visible", false);
        }

        protected void Emit(string @event, object data = null)
        {
            _nuiManager.Emit(new
            {
                overlay = Name,
                eventName = @event,
                data
            });
        }

        public void Focus(bool hasFocus, bool showCursor)
        {
            _nuiManager.Focus(hasFocus, showCursor);
        }

        protected void On(string @event, Action action)
        {
            _nuiManager.On($"{Name}/{@event}", action);
        }

        protected void On<T>(string @event, Action<T> action)
        {
            _nuiManager.On<T>($"{Name}/{@event}", action);
        }

        protected void On<TReturn>(string @event, Func<TReturn> action)
        {
            _nuiManager.On<TReturn>($"{Name}/{@event}", action);
        }

        protected void On<T, TReturn>(string @event, Func<T, TReturn> action)
        {
            _nuiManager.On<T, TReturn>($"{Name}/{@event}", action);
        }

        protected virtual void OnReady()
        {
        }
    }
}