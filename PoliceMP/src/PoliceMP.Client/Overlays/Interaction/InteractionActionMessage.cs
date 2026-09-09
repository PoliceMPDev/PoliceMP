using CitizenFX.Core;

namespace PoliceMP.Client.Overlays.Interaction
{
    public class InteractionActionMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public Control GamepadControl { get; set; }
        public Control MouseAndKeyboardControl { get; set; }
        public bool IsHold { get; }

        public InteractionActionMessage(string id, string text, Control gamepadControl, Control mouseAndKeyboardControl, bool isHold)
        {
            Id = id;
            Text = text;
            GamepadControl = gamepadControl;
            MouseAndKeyboardControl = mouseAndKeyboardControl;
            IsHold = isHold;
        }
    }
}