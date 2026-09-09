using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Scripts.CameraScripts
{
    public class PitCameraScript : Script
    {
        private readonly ILogger<PitCameraScript> _log;
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _input;

        public PitCameraScript(ILogger<PitCameraScript> log, ITickManager ticks, IGameInputManager input)
        {
            _log = log;
            _ticks = ticks;
            _input = input;
        }

        protected override Task OnStartAsync()
        {
            //_log.Debug("Pit Camera Started");

            _ticks.On(PitCameraTick);

            return Task.FromResult(0);
        }

        Task PitCameraTick()
        {
            if (Game.PlayerPed.CurrentVehicle is not null 
                && !API.IsUsingKeyboard(0)
                && !API.IsPauseMenuActive()
                && API.IsNavigatingMenuContent() == 0)
            {
                if (_input.IsBeingHeld(Control.VehicleCinCam, holdMs: 50))
                {
                    API.SetGameplayCamRelativePitch(-70, 0.2f);
                }
                else if(_input.IsJustReleased(Control.VehicleCinCam))
                {
                    API.SetGameplayCamRelativePitch(0f, 0.2f);
                }
            }

            return Task.FromResult(0);
        }
    }
}
