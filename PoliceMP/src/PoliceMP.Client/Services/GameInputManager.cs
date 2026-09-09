using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Services
{
    public class GameInputManager : IGameInputManager
    {
        private readonly ILogger<GameInputManager> _log;
        public const int DefaultHoldThresholdMs = 300;

        private List<int> _controlValues;
        private List<int> _releasedControls;
        private InputMode _inputMode;
        private Dictionary<int, int> _presses;

        private InputMode ResolveInput(InputMode input) => (int)input == -1 ? _inputMode : input;

        public GameInputManager(ILogger<GameInputManager> log, ITickManager ticks)
        {
            _log = log;
            _controlValues = Enum.GetValues(typeof(Control)).Cast<int>().ToList();
            _releasedControls = new List<int>(_controlValues.Count);
            _presses = new(_controlValues.Count);

            ticks.On(InputManagerTick);
        }

        private Task InputManagerTick()
        {
            _releasedControls.Clear();

            if(_inputMode != Game.CurrentInputMode)
            {
                _presses.Clear();
                _inputMode = Game.CurrentInputMode;
            }

            var pressesCount = _presses.Count();
            for (int i = 0; i < pressesCount; i++)
            {
                var kvp = _presses.ElementAt(i);
                if (!API.IsControlPressed((int)_inputMode, kvp.Key)
                    && !API.IsDisabledControlPressed((int)_inputMode, kvp.Key))
                {
                    _log.Trace($"Input Released: {(Control)kvp.Key}");
                    _presses.Remove(kvp.Key);
                    _releasedControls.Add(kvp.Key);
                    pressesCount--;
                    i--;
                    continue;
                }
            }

            var controlCount = _controlValues.Count;
            for (int i = 0; i < controlCount; i++)
            {
                var control = _controlValues[i];
                if (API.IsControlJustPressed((int)_inputMode, control)
                    || API.IsDisabledControlJustPressed((int)_inputMode, control))
                {
                    if(_presses.ContainsKey(control))
                    {
                        _presses[control] = Game.GameTime;
                    }
                    else
                    {
                        _log.Trace($"Input Pressed: {(Control)control}");
                        _presses.Add(control, Game.GameTime);
                    }
                }
            }

            return Task.FromResult(0);
        }

        public bool IsJustBeenTapped(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode) (-1), int holdMs = InputManagerDefaults.HoldThresholdMs)
        {
            if (ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsJustBeenTapped(keyboardMouseControl, input, holdMs);
            }
            else
            {
                return IsJustBeenTapped(gamepadControl, input, holdMs);
            }
        }

        public bool IsBeingHeld(Control control, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs)
        {
            if(!_presses.TryGetValue((int)control, out var pressStart))
            {
                return false;
            }

            var retValue = Game.GameTime - pressStart > holdMs;
            if (retValue && pressStart != 0)
            {
                _presses[(int)control] = 0;
            }
            
            return retValue;
        }

        public bool IsBeingHeld(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1), int holdMs = InputManagerDefaults.HoldThresholdMs)
        {
            if(ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsBeingHeld(keyboardMouseControl, input, holdMs);
            }
            else
            {
                return IsBeingHeld(gamepadControl, input, holdMs);
            }
        }

        public bool IsJustPressed(Control control, InputMode input = (InputMode)(-1))
        {
            var index = (int)ResolveInput(input);
            if (Game.IsControlEnabled(index, control))
            {
                return Game.IsControlJustPressed(index, control);
            }
            else
            {
                return Game.IsDisabledControlJustPressed(index, control);
            }
        }

        public bool IsJustPressed(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1))
        {
            if (ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsJustPressed(keyboardMouseControl, input);
            }
            else
            {
                return IsJustPressed(gamepadControl, input);
            }
        }

        public bool IsJustBeenTapped(Control control, InputMode input = (InputMode) (-1), int holdMs = InputManagerDefaults.HoldThresholdMs)
        {
            if (!_presses.TryGetValue((int)control, out var pressStart))
            {
                return false;
            }

            var retValue = (Game.IsControlJustReleased((int)input, control) || Game.IsDisabledControlJustReleased((int)input, control)) 
                           && Game.GameTime - pressStart < holdMs;

            return retValue;
        }

        public bool IsPressed(Control control, InputMode input = (InputMode)(-1))
        {
            var index = (int)ResolveInput(input);
            if (Game.IsControlEnabled(index, control))
            {
                return Game.IsControlPressed(index, control);
            }
            else
            {
                return Game.IsDisabledControlPressed(index, control);
            }
        }

        public bool IsPressed(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1))
        {
            if (ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsPressed(keyboardMouseControl, input);
            }
            else
            {
                return IsPressed(gamepadControl, input);
            }
        }

        public bool IsJustBeingHeld(Control control, InputMode input = (InputMode)(-1), int holdMs = 300)
        {
            if (!_presses.TryGetValue((int)control, out var pressStart))
            {
                return false;
            }

            var retValue = pressStart != 0 && Game.GameTime - pressStart > holdMs;
            if (retValue && pressStart != 0)
            {
                _presses[(int)control] = 0;
            }

            return retValue;
        }

        public bool IsJustBeingHeld(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1), int holdMs = 300)
        {
            if (ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsJustBeingHeld(keyboardMouseControl, input);
            }
            else
            {
                return IsJustBeingHeld(gamepadControl, input);
            }
        }

        public bool IsJustReleased(Control control, InputMode input = (InputMode)(-1))
        {
            return _releasedControls.Contains((int)control);
        }

        public bool IsJustReleased(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1))
        {
            if (ResolveInput(input) == InputMode.MouseAndKeyboard)
            {
                return IsJustReleased(keyboardMouseControl, input);
            }
            else
            {
                return IsJustReleased(gamepadControl, input);
            }
        }

        public float GetControlNormal(Control control, InputMode input = (InputMode) (-1))
        {
            return GetControlNormal(control, control, input);
        }

        public float GetControlNormal(Control keyboardMouseControl, Control gamepadControl, InputMode input = (InputMode)(-1))
        {
            var realInput = ResolveInput(input);
            var control = realInput == InputMode.MouseAndKeyboard ? keyboardMouseControl : gamepadControl;

            if (Game.IsControlEnabled((int)realInput, control))
            {
                return Game.GetControlNormal((int)realInput, control);
            }
            else
            {
                return Game.GetDisabledControlNormal((int)realInput, control);
            }
        }
    }
}