using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Properties;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.ClothingLocker
{
    public class OutfitCommands : Script
    {
        private readonly ICommandManager _commands;
        private readonly IPermissionService _permissionService;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private int _oldBeardComponent = 0;
        private int _oldBeardTexture = 0;
        private int _oldBeardPalette = 0;
        private int _oldHatComp = -1, _oldHatText = -1;
        private int _oldJacketComp = -1, _oldJacketText = -1;
        private int _maskId = 15;

        public OutfitCommands(ICommandManager commands, IPermissionService permissionService, INewNotificationOverlay newNotificationOverlay)
		{
            _commands = commands;
			_newNotificationOverlay = newNotificationOverlay;
			_permissionService = permissionService;
        }

        protected override Task OnStartAsync()
        {
            _commands.Register("mask").WithHandler(() =>
            {
                var ped = API.PlayerPedId();

                var playerPed = Game.PlayerPed.Model;
                bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                if (isMale)
                {
                    if (API.GetPedDrawableVariation(ped, 1) == 15)
                    {
                        API.SetPedComponentVariation(ped, 1, _oldBeardComponent, _oldBeardTexture, _oldBeardPalette);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Success!", "success", "You've taken off your disposable mask!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    _oldBeardComponent = API.GetPedDrawableVariation(ped, 1);
                    _oldBeardTexture = API.GetPedTextureVariation(ped, 1);
                    _oldBeardPalette = API.GetPedPaletteVariation(ped, 1);

                    API.SetPedComponentVariation(ped, 1, 15, 0, 0);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Success!", "success", "You've put on your disposable mask!", new NewNotificationMessageContent[0]));
                }
                else
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error", "This has not yet been implimented for females!", new NewNotificationMessageContent[0]));
                }

                
            });

            _commands.Register("hat").WithHandler(async () =>
            {
                var ped = API.PlayerPedId();
                API.RequestAnimDict("missheist_agency2ahelmet");
                API.RequestAnimDict("mp_masks@standard_car@ds@");

                var playerPed = Game.PlayerPed.Model;
                bool isMale = (playerPed == (uint)PedHash.FreemodeMale01);

                if (isMale)
                {
                    if (API.GetPedPropIndex(ped, 0) == 11)
                    {
                        if (_oldHatComp == -1)
                        {
                            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Hats!", "warning", "You have no hat to put back on!", new NewNotificationMessageContent[0]));
                            return;
                        }

                        var currentJacket = API.GetPedDrawableVariation(ped, 3);
                        if (currentJacket != _oldJacketComp)
                        {
                            _oldHatComp = -1;
                            return;
                        }

                        API.TaskPlayAnim(ped, "mp_masks@standard_car@ds@", "put_on_mask", 8f, 8f, 600, 51, 1f, false, false, false);
                        await Delay(600);
                        API.SetPedPropIndex(ped, 0, _oldHatComp, _oldHatText, true);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Hats!", "success", "You have put your hat back on!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    _oldHatComp = API.GetPedPropIndex(ped, 0);
                    _oldHatText = API.GetPedPropTextureIndex(ped, 0);
                    _oldJacketComp = API.GetPedDrawableVariation(ped, 3);
                    _oldJacketText = API.GetPedPropTextureIndex(ped, 3);

                    API.TaskPlayAnim(ped, "missheist_agency2ahelmet", "take_off_helmet_stand", 8f, 8f, 1200, 51, 1f, false, false, false);
                    await Delay(1200);
                    API.SetPedPropIndex(ped, 0, 11, 0, true);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Hats!", "success", "You have taken your hat off!", new NewNotificationMessageContent[0]));
                }
                else
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Error!", "error", "This has not yet been implimented for females!", new NewNotificationMessageContent[0]));
                }


                
            });
            
            return Task.FromResult(0);
        }
    }
}