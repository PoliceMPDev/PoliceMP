using System;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;

namespace PoliceMP.Core.Client.Overlays
{
    public abstract class Overlay : IOverlay
    {
        public bool Enabled { get; private set; }

        public string Id { get; set; }
        private readonly INuiManager _nuiManager;
        protected readonly ILogger<Overlay> _logger;

        public Overlay(string id, INuiManager nuiManager, ILogger<Overlay> logger)
        {
            Id = id;
            _nuiManager = nuiManager;
            _logger = logger;

            nuiManager.On<string>("OverlayShown", Show_Callback);
            nuiManager.On<string>("OverlayHidden", Hide_Callback);
            nuiManager.On<GetInstructionalButtonMessage, string>("GetInstructionalButton", GetInstructionalButton);
        }

        public virtual void Enable()
        {
            _logger.Debug($"[NUI] Showing overlay {Id}");
            _nuiManager.Emit("EnableOverlay", Id);
        }

        // Rozzers:OverlayShown
        private void Show_Callback(string id)
        {
            if (id == Id)
                Enabled = true;
        }

        public virtual void Disable()
        {
            _logger.Debug($"[NUI] Hiding overlay {Id}");
            _nuiManager.Emit("DisableOverlay", Id);
        }

        // Rozzers:OverlayHidden
        private void Hide_Callback(string id)
        {
            if (id == Id)
                Enabled = false;
        }

        protected void Emit(string eventName, object message = null)
        {
            _nuiManager.Emit("OverlayEvent", new
            {
                Overlay = Id,
                EventName = eventName,
                Message = message
            });
        }

        protected void Focus(bool hasFocus, bool showCursor)
        {
            _nuiManager.Focus(hasFocus, showCursor);
        }

        protected void On(string eventName, Action action)
        {
            _nuiManager.On($"{Id}/{eventName}", action);
        }

        protected void On<TAction>(string eventName, Action<TAction> action)
        {
            _nuiManager.On($"{Id}/{eventName}", action);
        }

        protected void On<TReturn>(string eventName, Func<TReturn> action)
        {
            _nuiManager.On<TReturn>($"{Id}/{eventName}", action);
        }

        protected void On<TInput, TReturn>(string eventName, Func<TInput, TReturn> action)
        {
            _nuiManager.On<TInput, TReturn>($"{Id}/{eventName}", action);
        }

        private string GetInstructionalButton(GetInstructionalButtonMessage message)
        {
            _logger.Trace($"Received GetInstructionalButton message: {message.GetType().FullName}");
            var mapping = API.GetControlInstructionalButton((int)message.InputMode, (int)message.Control, 2);

            return mapping;
        }
    }
}
