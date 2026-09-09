using CitizenFX.Core.Native;
using CitizenFX.Core;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using System.Threading.Tasks;
using PoliceMP.Client.Services.Interfaces;

namespace PoliceMP.Client.Actions.SmashWindows
{
    public class SmashWindowsHandler : ActionHandler<SmashWindows>
    {
        #region Services

        private readonly ILogger<SmashWindowsHandler> _logger;

        #endregion


        public SmashWindowsHandler(ILogger<SmashWindowsHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<bool> Handle(SmashWindows action)
        {

            var player = Game.PlayerPed;
            var vehicle = action.Target.Handle;
            var windowIndex = (int)action.Window;
            var animDict = "melee@large_wpn@streamed_core";
            var animName = "ground_attack_on_spot";

            API.RequestAnimDict(animDict);
            while (!API.HasAnimDictLoaded(animDict))
            {
                await Delay(200);
            }
            API.TaskPlayAnim(player.Handle, animDict, animName, 8.0f, -8.0f, -1, 49, 0, false, false, false);
            await Delay(1000);
            API.DecorSetBool(vehicle, "WindowSmashed", true);
            API.DecorSetInt(vehicle, "WindowSmashedInt", windowIndex);
            var driverPed = API.GetPedInVehicleSeat(vehicle, 0);
            API.DecorSetBool(driverPed, "DriverWindowSmashed", true);
            API.DecorSetInt(driverPed, "DriverWindowSmashedInt", windowIndex);
            API.SmashVehicleWindow(vehicle, windowIndex);

            return true;
        }
    }
}