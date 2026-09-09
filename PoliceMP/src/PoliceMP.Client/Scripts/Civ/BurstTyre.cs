using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Civ
{
    public class BurstTyre : Script
    {
        private readonly ICommandManager _command;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly IPermissionService _permissionService;

        public BurstTyre(ICommandManager command, INewNotificationOverlay newNotificationOverlay, IPermissionService permissionService)
        {
            _command = command;
			_newNotificationOverlay = newNotificationOverlay;
			_permissionService = permissionService;
        }

        protected override async Task OnStartAsync()
        {
            var aces = _permissionService.GetUserAces();
            
            _command.Register("poptyre").HasGreedyArgs().WithHandler(tyreNum =>
            {
                if (!aces.Result.IsCivTrained)
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Civ Required!", "error", "You must be a civ to use this", new NewNotificationMessageContent[0]));
					return;
                }
                
                if (Game.PlayerPed.CurrentVehicle == null || !Game.PlayerPed.IsInVehicle())
				{
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Can't pop that!", "error", "You must be in a vehicle to use this", new NewNotificationMessageContent[0]));
                    return;
                }
                
                var tyre = -1;
                if(!Int32.TryParse(tyreNum, out tyre))
				{
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Can't pop that!", "error", "You must input a number as a parameter. e.g. /poptyre 1", new NewNotificationMessageContent[0]));
                    return;
                }
                
                var vehHandle = Game.PlayerPed.CurrentVehicle.Handle;
                API.NetworkRequestControlOfEntity(vehHandle);

                if (!API.NetworkHasControlOfEntity(vehHandle))
                {
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Can't control entity", "error", "Can't control entity, try again :)", new NewNotificationMessageContent[0]));
				}
                API.SetVehicleTyresCanBurst(vehHandle, true);
                API.SetVehicleTyreBurst(vehHandle, tyre, true, 1000);
            });
            
            _command.Register("popall").WithHandler(() =>
            {
                if (!aces.Result.IsCivTrained)
				{
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Civ Required!", "error", "You must be a civ to use this", new NewNotificationMessageContent[0]));
					return;
                }
                
                if (Game.PlayerPed.CurrentVehicle == null || !Game.PlayerPed.IsInVehicle())
				{
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Can't pop that!", "error", "You must be in a vehicle to use this", new NewNotificationMessageContent[0]));
					return;
                }
                
                var vehHandle = Game.PlayerPed.CurrentVehicle.Handle;
                API.NetworkRequestControlOfEntity(vehHandle);

                if (!API.NetworkHasControlOfEntity(vehHandle))
				{
					_newNotificationOverlay.SendNotification(new NewNotificationMessage("Can't control entity", "error", "Can't control entity, try again :)", new NewNotificationMessageContent[0]));
				}
                API.SetVehicleTyresCanBurst(vehHandle, true);
                for (var i = 0; i <= 100; i++)
                {
                    API.SetVehicleTyreBurst(vehHandle, i, true, 1000);
                }
                
            });
        }
    }
}