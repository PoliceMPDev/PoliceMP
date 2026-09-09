using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    public class PlayerAction
    {
        private readonly Func<bool> _filter;

        public string Name { get; }

        public string Text { get; }

        public Control MouseAndKeyboardControl { get; }
        public Control GamepadControl { get; set; }
        public Func<Task> Callback { get; }

        public PlayerAction(string name, string text, Control mouseAndKeyboardControl,
            Control gamepadControl, Func<Task> callback, Func<bool> filter = null)
        {
            _filter = filter;
            Name = name;
            MouseAndKeyboardControl = mouseAndKeyboardControl;
            Callback = callback;
            GamepadControl = gamepadControl;
            Text = text;

            API.AddTextEntry(Name, text);
        }

        public PlayerAction(string name, string text, Control mouseAndKeyboardControl, Control GamepadControl, Action callback, Func<bool> filter = null)
            : this(name, text, mouseAndKeyboardControl, GamepadControl, () =>
            {
                callback();
                return Task.FromResult(0);
            }, filter)
        {
        }

        public PlayerAction(string name, string text, Control control, Func<Task> callback, Func<bool> filter = null)
        : this(name, text, control, control, callback, filter)
        {
        }

        public PlayerAction(string name, string text, Control control, Action callback, Func<bool> filter = null)
            : this(name, text, control, control, callback, filter)
        {
        }

        public bool IsValid()
        {
            if (_filter != null) return _filter();

            return true;
        }
    }
}
