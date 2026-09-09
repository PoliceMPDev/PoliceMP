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
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Mediator;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Client.Actions.BagBody
{
    public class BagBodyHandler : ActionHandler<BagBody>
    {
        #region Services

        private readonly ILogger<BagBodyHandler> _logger;
        private readonly INotificationService _notification;

        #endregion


        public BagBodyHandler(ILogger<BagBodyHandler> logger, INotificationService notification)
        {
            _logger = logger;
            _notification = notification;
        }

        protected override async Task<bool> Handle(BagBody action)
        {
            action.Target.SetBoolDecor(PedStates.BagBodyActionActive, true);
            var targetPed = action.Target.Handle;
            var player = Game.PlayerPed.Handle;
            var playerPos = Game.PlayerPed.Position;
            var pedPos = API.GetEntityCoords(targetPed, true);
            var pedHeading = API.GetEntityHeading(targetPed);
            var distance = 0.85f;
            var prop = API.GetClosestObjectOfType(playerPos.X, playerPos.Y, playerPos.Z, distance, (uint)API.GetHashKey("xm_prop_body_bag"), false, false, false);

            if (prop == 0)
            {
                _notification.Error("Body Bag", "You must place a bodybag near the body!");
                return true;
            }

            API.SetEntityHeading(player, pedHeading);
            API.ExecuteCommand("e medic2");
            Screen.ShowSubtitle("~y~You are now moving the ~r~body ~y~into the bag...", 9000);
            await Delay(2000);
            API.ExecuteCommand("e mechanic");
            await Delay(7000);
            API.DeleteEntity(ref targetPed);
            API.ExecuteCommand("e medic2");
            await Delay(1000);
            API.ExecuteCommand("e c");
            Screen.ShowSubtitle("~y~You are now removing ~r~blood ~y~and ~r~debris ~y~from the scene...", 9000);
            API.ExecuteCommand("e medic2");
            await Delay(500);
            API.ExecuteCommand("e clean");
            await Delay(8000);
            API.ExecuteCommand("e c");
            API.RemoveDecalsInRange(pedPos.X, pedPos.Y, pedPos.Z, 5f);

            action.Target.SetBoolDecor(PedStates.BagBodyActionActive, false);
            return true;
        }
    }
}