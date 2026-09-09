using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Services
{
    public class InstructionalButtonsService : IInstructionalButtonsService
    {
        private readonly ITickManager _ticks;
        private readonly ILogger<InstructionalButtonsService> _logger;
        private List<ScaleformButton> _buttons = new List<ScaleformButton>();
        private int _scaleFormHandle = -1;
        private bool _isActive = false;

        public InstructionalButtonsService(ITickManager ticks, ILogger<InstructionalButtonsService> logger)
        {
            _ticks = ticks;
            _logger = logger;
        }

        private Task InstructionalButtonsTick()
        {
            API.DrawScaleformMovieFullscreen(_scaleFormHandle, 255, 255, 255, 255, 0);
            return Task.FromResult(0);
        }

        public async Task RemoveInstructionalButtons(List<string> names)
        {
            _buttons.RemoveAll(x => names.Contains(x.Name));
            await SetInstructions();

            if (_isActive && _buttons.Count < 1)
            {
                _isActive = false;
                _ticks.Off(InstructionalButtonsTick);
            }
        }

        public bool IsButtonShown(string name)
        {
            return _buttons.Any(x => x.Name == name);
        }

        public async Task AddInstructionalButton(string name, int control)
        {
            if (IsButtonShown(name)) return;

            _buttons.Add(new ScaleformButton { Name = name, Control = control });
            await SetInstructions();

            if (!_isActive)
            {
                _isActive = true;
                _ticks.On(InstructionalButtonsTick);
            }
        }

        public async Task AddInstructionalButton(string name, Control control)
        {
            await AddInstructionalButton(name, (int)control);
        }

        public async Task RemoveInstructionalButton(string name)
        {
            var button = _buttons.FirstOrDefault(x => x.Name == name);
            if (button == null) return;

            _buttons.Remove(button);
            await SetInstructions();

            if (_isActive && _buttons.Count < 1)
            {
                _isActive = false;
                _ticks.Off(InstructionalButtonsTick);
            }
        }

        private async Task SetInstructions()
        {
            _scaleFormHandle = await SetupScaleform("instructional_buttons");
        }

        private async Task<int> SetupScaleform(string scaleFormName)
        {
            int scaleFormHandle = API.RequestScaleformMovie(scaleFormName);
            while (!API.HasScaleformMovieLoaded(scaleFormHandle))
                await Script.Delay(1);

            API.PushScaleformMovieFunction(scaleFormHandle, "CLEAR_ALL");
            API.PopScaleformMovieFunctionVoid();

            API.PushScaleformMovieFunction(scaleFormHandle, "SET_CLEAR_SPACE");
            API.PushScaleformMovieFunctionParameterInt(200);
            API.PopScaleformMovieFunctionVoid();

            for (int i = 0; i < _buttons.Count; i++)
            {
                API.PushScaleformMovieFunction(scaleFormHandle, "SET_DATA_SLOT");
                API.PushScaleformMovieFunctionParameterInt(i);

                string controlName = API.GetControlInstructionalButton(2, _buttons[i].Control, 1);
                API.N_0xe83a3e3557a56640(controlName);

                API.BeginTextCommandScaleformString(_buttons[i].Name);
                API.EndTextCommandScaleformString();

                API.PopScaleformMovieFunctionVoid();
            }

            API.PushScaleformMovieFunction(scaleFormHandle, "DRAW_INSTRUCTIONAL_BUTTONS");
            API.PopScaleformMovieFunctionVoid();

            API.PushScaleformMovieFunction(scaleFormHandle, "SET_BACKGROUND_COLOUR");
            API.PushScaleformMovieFunctionParameterInt(0);
            API.PushScaleformMovieFunctionParameterInt(0);
            API.PushScaleformMovieFunctionParameterInt(0);
            API.PushScaleformMovieFunctionParameterInt(80);
            API.PopScaleformMovieFunctionVoid();

            return scaleFormHandle;
        }

        private class ScaleformButton
        {
            public string Name { get; set; }
            public int Control { get; set; }
        }
    }
}