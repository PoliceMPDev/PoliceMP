using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    public class PlayerAction
    {
        private readonly Func<bool> _filter;

        public string Name { get; }

        public string Text { get; }

        public Control MouseAndKeyboardControl { get; }
        public Control GamepadControl { get; set; }
        public PlayerActionFlags Flags { get; }
        public Func<Task> Callback { get; }
        public Func<Task<Entity>> EntityChangeCallback { get; }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="mouseAndKeyboardControl">Control to use while using keyboard and mouse input</param>
        /// <param name="gamepadControl">Control to use while using controller input</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="callback">Callback that's called when the player invokes this action.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control mouseAndKeyboardControl,
            Control gamepadControl, Func<Task> callback, Func<bool> filter = null, PlayerActionFlags flags = default)
        {
            _filter = filter;
            Name = name;
            MouseAndKeyboardControl = mouseAndKeyboardControl;
            Callback = callback;
            GamepadControl = gamepadControl;
            Text = text;
            Flags = flags;

            var onScreenTest = Flags.HasFlag(PlayerActionFlags.HoldControl) ? $"{text} (hold)" : text;
            API.AddTextEntry(Name, onScreenTest);
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="mouseAndKeyboardControl">Control to use while using keyboard and mouse input</param>
        /// <param name="gamepadControl">Control to use while using controller input</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="callback">Callback that's called when the player invokes this action.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control mouseAndKeyboardControl, Control gamepadControl, Action callback, Func<bool> filter = null, PlayerActionFlags flags = default)
            : this(name, text, mouseAndKeyboardControl, gamepadControl, () =>
            {
                callback();
                return Task.FromResult(0);
            }, filter, flags)
        {
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="control">Control to use for all inputs</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="callback">Callback that's called when the player invokes this action.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control control, Func<Task> callback, Func<bool> filter = null, PlayerActionFlags flags = default)
        : this(name, text, control, control, callback, filter, flags)
        {
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="control">Control to use for all inputs</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="callback">Callback that's called when the player invokes this action.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control control, Action callback, Func<bool> filter = null, PlayerActionFlags flags = default)
            : this(name, text, control, control, callback, filter, flags)
        {
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="mouseAndKeyboardControl">Control to use while using keyboard and mouse input</param>
        /// <param name="gamepadControl">Control to use while using controller input</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="interactionEntityChange">When this action is invokes, change the targetted entity to this entity.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control mouseAndKeyboardControl,
            Control gamepadControl, Func<Task<Entity>> interactionEntityChange, Func<bool> filter = null, PlayerActionFlags flags = default)
        {
            _filter = filter;
            Name = name;
            MouseAndKeyboardControl = mouseAndKeyboardControl;
            EntityChangeCallback = interactionEntityChange;
            GamepadControl = gamepadControl;
            Text = text;
            Flags = flags;
            API.AddTextEntry(Name, text);
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="mouseAndKeyboardControl">Control to use while using keyboard and mouse input</param>
        /// <param name="gamepadControl">Control to use while using controller input</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="interactionEntityChange">When this action is invokes, change the targetted entity to this entity.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control mouseAndKeyboardControl, Control gamepadControl, Func<Entity> interactionEntityChange, Func<bool> filter = null, PlayerActionFlags flags = default)
            : this(name, text, mouseAndKeyboardControl, gamepadControl, () => Task.FromResult(interactionEntityChange()), filter, flags)
        {
        }

        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="control">Control to use for all inputs</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="interactionEntityChange">When this action is invokes, change the targetted entity to this entity.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control control, Func<Task<Entity>> interactionEntityChange, Func<bool> filter = null, PlayerActionFlags flags = default)
            : this(name, text, control, control, interactionEntityChange, filter, flags)
        {
        }


        /// <summary>
        /// A player action that can be used during interactions based on certain optional conditions.
        /// </summary>
        /// <param name="name">Unique identifier for the action</param>
        /// <param name="text">Display text for the action</param>
        /// <param name="control">Control to use for all inputs</param>
        /// <param name="flags">Disable player movement while performing the action</param>
        /// <param name="interactionEntityChange">When this action is invokes, change the targetted entity to this entity.</param>
        /// <param name="filter">Filter to use for this action. Only when the filter is happy will the action appear.</param>
        public PlayerAction(string name, string text, Control control, Func<Entity> interactionEntityChange, Func<bool> filter = null, PlayerActionFlags flags = default)
            : this(name, text, control, control, interactionEntityChange, filter, flags)
        {
        }

        public bool IsValid()
        {
            if (_filter != null) return _filter();

            return true;
        }

        public override bool Equals(object obj)
        {

            if (obj is not PlayerAction otherAction)
            {
                return false;
            }

            return Equals(otherAction);
        }

        protected bool Equals(PlayerAction other)
        {
            return Equals(_filter, other._filter) && Name == other.Name && Text == other.Text && MouseAndKeyboardControl == other.MouseAndKeyboardControl && GamepadControl == other.GamepadControl && Flags == other.Flags && Equals(Callback, other.Callback) && Equals(EntityChangeCallback, other.EntityChangeCallback);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_filter != null ? _filter.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)MouseAndKeyboardControl;
                hashCode = (hashCode * 397) ^ (int)GamepadControl;
                hashCode = (hashCode * 397) ^ (int)Flags;
                hashCode = (hashCode * 397) ^ (Callback != null ? Callback.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EntityChangeCallback != null ? EntityChangeCallback.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
