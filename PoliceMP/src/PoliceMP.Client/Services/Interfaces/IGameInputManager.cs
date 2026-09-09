using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{

    public interface IGameInputManager
    {
        bool IsPressed(Control control, InputMode input = (InputMode)(-1));
        bool IsJustPressed(Control control, InputMode input = (InputMode)(-1));
        bool IsPressed(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1));
        bool IsJustPressed(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1));
        bool IsJustBeenTapped(Control control, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsJustBeenTapped(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsBeingHeld(Control control, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsBeingHeld(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsJustBeingHeld(Control control, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsJustBeingHeld(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs);
        bool IsJustReleased(Control control, InputMode input = (InputMode)(-1));
        bool IsJustReleased(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1));
        float GetControlNormal(Control control, InputMode input = (InputMode)(-1));
        float GetControlNormal(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1));
        //bool IsCombinationPressed(params Control[] controls);
        //bool IsCombinationHeld(params Control[] controls);

        //bool IsPressedWhileHolding(Control heldControl, Control actionControl, int holdMs = InputManagerDefaults.HoldThresholdMs);
    }
}
