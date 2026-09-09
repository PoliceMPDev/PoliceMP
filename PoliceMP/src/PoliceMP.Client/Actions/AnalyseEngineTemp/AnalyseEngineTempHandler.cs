using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Extensions;
using PoliceMP.Client.Scripts.Afk;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Actions.AnalyseEngineTemp
{
    public class AnalyseEngineTempHandler : ActionHandler<AnalyseEngineTemp>
    {
        #region Services

        private readonly ILogger<AnalyseEngineTempHandler> _logger;
        private readonly INotificationService _notification;

        #endregion


        public AnalyseEngineTempHandler(ILogger<AnalyseEngineTempHandler> logger, INotificationService notification)
        {
            _logger = logger;
            _notification = notification;
            _notification = notification;
        }

        protected override async Task<bool> Handle(AnalyseEngineTemp action)
        {

            var player = Game.PlayerPed;
            var engineTemp = API.GetVehicleEngineTemperature(action.Target.Handle);
            API.SetVehicleDoorOpen(action.Target.Handle, 4, false, false);

            var roundedTemp = (int)Math.Ceiling(engineTemp);

            Screen.ShowSubtitle("~y~You are now checking the temperature of the ~r~engine...", 13000);
            API.ExecuteCommand("e mechanic");
            await Delay(9500);
            API.ExecuteCommand("e clipboard");
            await Delay(3700);

            var message = "";
            switch (engineTemp)
            {
                case < 30:
                    message = "You can tell that this vehicle has not been driven recently!";
                    break;
                case < 60:
                    message = "You can tell that this vehicle could have been driven a while ago!";
                    break;
                case > 60:
                    message = "You can tell that this vehicle has recently been driven!";
                    break;
            }

            Screen.ShowSubtitle($"~y~After analysing the temperature, you know that the ~r~engine~y~ is ~r~{roundedTemp}~y~ degrees celsius! " + message, 10000);
            API.ExecuteCommand("e c"); //Cancels emote
            API.SetVehicleDoorShut(action.Target.Handle, 4, false);

            return true;
        }
    }
}