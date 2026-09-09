using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Scripts.Admin;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Commands
{
    public class OtherCommands : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly INewNotificationOverlay _newNotificationOverlay;
        private bool HandsUp;
        private bool RifleRelax;
        private bool weaponHolstered;
        public static bool enableReachingAnim;
        private bool animPlaying;
        private string animDict = "move_m@intimidation@cop@unarmed";
        private string animName = "idle";
        public int clampObject;
        bool stopClearingTasks;
        private int prevHands;
        private int prevMask;
        private bool _isGodModeEnabled;

        public OtherCommands(ICommandManager commandManager, ITickManager ticks, IPermissionService permissionService, INewNotificationOverlay newNotificationOverlay)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            API.DecorRegister("WindowSmashed", 2);
            API.DecorRegister("DriverWindowSmashed", 2);
            API.DecorRegister("DriverWindowSmashedInt", 3);
            API.DecorRegister("WindowSmashedInt", 3);
            _ticks.On(CheckWindowSmash);
            _ticks.On(CheckWindowSmashDriver);
            _ticks.On(AutoBreakLights);
            
            if (DateTime.Now.Day == 1 && DateTime.Now.Month == 4 && DateTime.Now.Year == 2025) // April Fools 2024
            {
                _commandManager.Register("godmode").WithHandler(async () =>
                {
                    _isGodModeEnabled = !_isGodModeEnabled;
                    _newNotificationOverlay.SendNotification(_isGodModeEnabled
                        ? new NewNotificationMessage("God Mode", "success", "God Mode enabled",
                            new NewNotificationMessageContent[0])
                        : new NewNotificationMessage("God Mode", "error", "God Mode disabled",
                            new NewNotificationMessageContent[0]));
                    Game.PlayerPed.Kill();
                    // await BaseScript.Delay(3000); // give them 3 seconds to think about it
                    // _newNotificationOverlay.SendNotification(new NewNotificationMessage("Hmm...", "info", "Nope... Wait, what day is it?", new NewNotificationMessageContent[0]));
                });
            }

            //_commandManager.Register("shuff").WithHandler(OnShuffCommand);

            //API.DecorRegister("clampStatus", 2);

            _ticks.On(WeaponReaching);
            _ticks.On(ReachingCancel);
            //_ticks.On(VehicleClampedCheck);

            //_commandManager.Register("clamp").WithHandler(OnClamp);

            _commandManager.Register("riflerelax").WithHandler(OnRifeRelaxCommand);
            API.RegisterKeyMapping("riflerelax", "Toggle RifleRelax Emote", "keyboard", "Z");

            API.RegisterKeyMapping("handsup", "Toggle HandsUp Emote", "keyboard", "M");
            _commandManager.Register("handsup").WithHandler(OnHandsUpCommand);

            _commandManager.Register("gloves").WithHandler(OnGlovesCommand);

            _commandManager.Register("rollall").WithHandler(OnRollAllCommand);

            _commandManager.Register("rollfront").WithHandler(OnRollFrontCommand);

            _commandManager.Register("rollback").WithHandler(OnRollBackCommand);

            _commandManager.Register("rollwindow").HasGreedyArgs().WithHandler(inWindowID =>
            {
                var player = Game.PlayerPed.Handle;

                if (!Int32.TryParse(inWindowID, out int outWindowID))
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "You must input a window number as a parameter. e.g. /rollwindow 1", new NewNotificationMessageContent[0]));
                    return;
                }

                if (outWindowID > 3)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "That is not a correct Window number, use /rollwindow (0,1,2,3) or use /rollall!", new NewNotificationMessageContent[0]));
                    return;
                }
                if (outWindowID < 0)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "That is not a correct Window number, use /rollwindow (0,1,2,3) or use /rollall!", new NewNotificationMessageContent[0]));
                    return;
                }

                int vehicle = API.GetVehiclePedIsIn(player, false);
                bool inVehicleCheck = API.IsPedInAnyVehicle(player, false);
                bool isWindowDown = API.IsVehicleWindowIntact(vehicle, outWindowID);

                if (!inVehicleCheck)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "You must be in a vehicle to roll down the window!", new NewNotificationMessageContent[0]));
                    return;
                }
                if (!isWindowDown)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled up your window!", new NewNotificationMessageContent[0]));
                    API.FixVehicleWindow(vehicle, outWindowID);
                    API.RollUpWindow(vehicle, outWindowID);
                    return;
                }
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled down your window!", new NewNotificationMessageContent[0]));
                API.RollDownWindow(vehicle, outWindowID);
                API.SmashVehicleWindow(vehicle, outWindowID);
            });
        }

        #region Auto Break Lights

        private async Task AutoBreakLights()
        {
            await Delay(400);
            var player = Game.PlayerPed.Handle;
            var inVehicle = API.IsPedInAnyVehicle(player, true);

            if (!inVehicle) return;

            var vehicle = API.GetVehiclePedIsIn(player, false);
            var driver = API.GetPedInVehicleSeat(vehicle, 0);

            if (driver != player) return;

            // entity speed * 2.236936 for mph
            var vehicleSpeed = API.GetEntitySpeed(vehicle);
            var vehicleSpeedMph = vehicleSpeed * 2.236936f;

            if (vehicleSpeedMph > 10f)
            {
                API.SetVehicleBrakeLights(vehicle, false);
                return;
            }

            API.SetVehicleBrakeLights(vehicle, true);
            return;
        }

        #endregion

        #region Gloves Command
        private async void OnGlovesCommand()
        {

            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole == null) return;
            var userAces = await _permissionService.GetUserAces();
            if (currentUserRole.Branch != UserBranch.Police && currentUserRole.Branch != UserBranch.Nhs) return;

            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;
            if (API.IsPedInAnyPlane(player)) return;

            var glovesType1 = 85;
            var glovesType2 = 86;
            var glovesType3 = 88;
            var glovesType4 = 92;
            var afoGloves = 17;
            var ctGloves = 171;

            var wearingGloves = false;
            if (API.GetPedDrawableVariation(player, 3) == glovesType1) wearingGloves = true;
            if (API.GetPedDrawableVariation(player, 3) == glovesType2) wearingGloves = true;
            if (API.GetPedDrawableVariation(player, 3) == glovesType3) wearingGloves = true;
            if (API.GetPedDrawableVariation(player, 3) == glovesType4) wearingGloves = true;
            if (API.GetPedDrawableVariation(player, 3) == afoGloves) wearingGloves = true;
            if (API.GetPedDrawableVariation(player, 3) == ctGloves) wearingGloves = true;

            if (!wearingGloves)
            {
                prevHands = API.GetPedDrawableVariation(player, 3);
            }
            if (wearingGloves)
            {
                if (currentUserRole.Division == UserDivision.Afo && API.GetPedDrawableVariation(player, 3) != afoGloves)
                {
                    if (API.GetPedDrawableVariation(player, 3) == glovesType1)
                    {
                        API.SetPedComponentVariation(player, 3, ctGloves, 0, 0);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Gloves", "success", "You have applied your firearms gloves!", new NewNotificationMessageContent[0]));
                        return;
                    }

                    if (API.GetPedDrawableVariation(player, 3) != ctGloves)
                    {
                        API.SetPedComponentVariation(player, 3, afoGloves, 0, 0);
                        _newNotificationOverlay.SendNotification(new NewNotificationMessage("Gloves", "success", "You have applied your firearms gloves!", new NewNotificationMessageContent[0]));
                        return;
                    }
                }

                API.SetPedComponentVariation(player, 3, prevHands, 0, 0);
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Gloves", "success", "You have taken off your Gloves!", new NewNotificationMessageContent[0]));
                return;
            }

            try
            {
                var currentHandsID = API.GetPedDrawableVariation(player, 3);
                switch (currentHandsID)
                {
                    case 0:
                        API.SetPedComponentVariation(player, 3, glovesType1, 0, 0);
                        break;
                    case 4:
                        API.SetPedComponentVariation(player, 3, glovesType3, 0, 0);
                        break;
                    case 1:
                        API.SetPedComponentVariation(player, 3, glovesType2, 0, 0);
                        break;
                    case 11:
                        API.SetPedComponentVariation(player, 3, glovesType4, 0, 0);
                        break;
                    case 12:
                        API.SetPedComponentVariation(player, 3, glovesType2, 0, 0);
                        break;
                    default:
                        return;
                }

                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Gloves", "success", "You have applied your Gloves!", new NewNotificationMessageContent[0]));
            }
            catch
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Gloves", "error", "Error has occured while trying to put on your gloves, try again!", new NewNotificationMessageContent[0]));
            }
        }
        #endregion


        private async Task CheckWindowSmashDriver()
        {
            await Delay(1000);
            var player = Game.PlayerPed.Handle;
            var vehicle = API.GetVehiclePedIsIn(player, false);

            await Delay(5);
            if (!API.DecorGetBool(player, "DriverWindowSmashed")) return;
            var windowIndex = API.DecorGetInt(player, "DriverWindowSmashedInt");
            API.SmashVehicleWindow(vehicle, windowIndex);
        }

        private async Task CheckWindowSmash()
        {
            await Delay(1000);
            var player = Game.PlayerPed.Handle;

            var vehicles = World.GetAllVehicles();
            var windowIndex = 0;
            foreach (var vehicle in vehicles)
            {
                await Delay(5);
                if (!API.DecorGetBool(vehicle.Handle, "WindowSmashed")) continue;

                windowIndex = API.DecorGetInt(vehicle.Handle, "WindowSmashedInt");
                API.SmashVehicleWindow(vehicle.Handle, windowIndex);
            }
        }


        private void OnRifeRelaxCommand()
        {
            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;

            if (AdminMenu.NoClipActive) return;
            if (currentUserRole.Branch != UserBranch.Police) return;
            if (API.IsPedInAnyVehicle(player, true)) return;
            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;
            if (API.IsPedInAnyPlane(player)) return;

            bool isPedArmed = API.IsPedArmed(player, 4 | 2);
            if (!isPedArmed) return;

            if (RifleRelax)
            {
                API.ExecuteCommand("e c");
                RifleRelax = false;
            }
            else
            {
                API.ExecuteCommand("e riflerelax");
                RifleRelax = true;
            }
        }

        private void OnHandsUpCommand()
        {
            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;

            if (AdminMenu.NoClipActive) return;
            if (currentUserRole.Branch != UserBranch.Civ) return;
            if (API.IsPedInAnyVehicle(player, true)) return;
            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;
            if (API.IsPedInAnyPlane(player)) return;

            if (HandsUp)
            {
                API.ExecuteCommand("e c");
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emote", "error", "You have put your hands down!", new NewNotificationMessageContent[0]));
                HandsUp = false;
            }
            else
            {
                API.ExecuteCommand("e handsup");
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Emote", "error", "You have put your hands up!", new NewNotificationMessageContent[0]));
                HandsUp = true;
            }
        }

        #region Police Reaching Anims
        private async Task ReachingCancel()
        {
            API.RequestAnimDict(animDict);

            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;

            animPlaying = API.IsEntityPlayingAnim(player, animDict, animName, 49);

            if (API.IsPedInAnyVehicle(player, true)) return;
            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;
            if (API.IsPedInAnyPlane(player)) return;
            if (animPlaying != true) return;
            if (currentUserRole.Branch != UserBranch.Police) return;

            if (API.IsControlReleased(0, 37) && !stopClearingTasks) // TAB and LB Keys
            {
                await Delay(0);
                API.ClearPedTasks(player);
                stopClearingTasks = true;
            }
        }

        private async Task WeaponReaching()
        {
            API.RequestAnimDict(animDict);

            var player = Game.PlayerPed.Handle;
            var currentUserRole = _permissionService.CurrentUserRole;

            if (currentUserRole == null) return;

            animPlaying = API.IsEntityPlayingAnim(player, animDict, animName, 49);

            if (API.IsPedInAnyVehicle(player, true)) return;
            if (API.IsPedInAnyHeli(player)) return;
            if (API.IsPedInAnyBoat(player)) return;
            if (API.IsPedInAnyPlane(player)) return;
            if (currentUserRole.Branch != UserBranch.Police) return;

            while (API.IsControlPressed(0, 37) && !animPlaying) // TAB and LB Keys
            {
                stopClearingTasks = false;
                API.TaskPlayAnim(player, animDict, animName, 10000000f, 2f, -1, 49, 0.0f, false, false, false);
                await Delay(100);
            }
        }

        #endregion


        private void OnShuffCommand()
        {
            var player = Game.PlayerPed;

            if (player.CurrentVehicle == null) return;

            if (player.SeatIndex == VehicleSeat.Passenger && player.CurrentVehicle.IsSeatFree(VehicleSeat.Driver))
            {
                player.Task.ShuffleToNextVehicleSeat(player.CurrentVehicle);
            }
        }

        private void OnRollAllCommand()
        {
            var player = Game.PlayerPed.Handle;
            int vehicle = API.GetVehiclePedIsIn(player, false);
            bool inVehicleCheck = API.IsPedInAnyVehicle(player, false);
            bool isWindowDown = API.IsVehicleWindowIntact(vehicle, 0);

            if (!inVehicleCheck)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "You must be in a vehicle to roll down the window!", new NewNotificationMessageContent[0]));
                return;
            }
            if (!isWindowDown)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled up all your windows!", new NewNotificationMessageContent[0]));
                API.FixVehicleWindow(vehicle, 0);
                API.FixVehicleWindow(vehicle, 1);
                API.FixVehicleWindow(vehicle, 2);
                API.FixVehicleWindow(vehicle, 3);

                API.RollUpWindow(vehicle, 0);
                API.RollUpWindow(vehicle, 1);
                API.RollUpWindow(vehicle, 2);
                API.RollUpWindow(vehicle, 3);

                return;
            }
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled down all your windows!", new NewNotificationMessageContent[0]));
            API.RollDownWindow(vehicle, 0);
            API.RollDownWindow(vehicle, 1);
            API.RollDownWindow(vehicle, 2);
            API.RollDownWindow(vehicle, 3);

            API.SmashVehicleWindow(vehicle, 0);
            API.SmashVehicleWindow(vehicle, 1);
            API.SmashVehicleWindow(vehicle, 2);
            API.SmashVehicleWindow(vehicle, 3);

        }

        private void OnRollFrontCommand()
        {
            var player = Game.PlayerPed.Handle;
            int vehicle = API.GetVehiclePedIsIn(player, false);
            bool inVehicleCheck = API.IsPedInAnyVehicle(player, false);
            bool isWindowDown = API.IsVehicleWindowIntact(vehicle, 0);

            if (!inVehicleCheck)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "You must be in a vehicle to roll down the window!", new NewNotificationMessageContent[0]));
                return;
            }
            if (!isWindowDown)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled up your front windows!", new NewNotificationMessageContent[0]));
                API.FixVehicleWindow(vehicle, 0);
                API.FixVehicleWindow(vehicle, 1);

                API.RollUpWindow(vehicle, 0);
                API.RollUpWindow(vehicle, 1);

                return;
            }
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled down your front windows!", new NewNotificationMessageContent[0]));
            API.RollDownWindow(vehicle, 0);
            API.RollDownWindow(vehicle, 1);

            API.SmashVehicleWindow(vehicle, 0);
            API.SmashVehicleWindow(vehicle, 1);

        }

        private void OnRollBackCommand()
        {
            var player = Game.PlayerPed.Handle;
            int vehicle = API.GetVehiclePedIsIn(player, false);
            bool inVehicleCheck = API.IsPedInAnyVehicle(player, false);
            bool isWindowDown = API.IsVehicleWindowIntact(vehicle, 2);

            if (!inVehicleCheck)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "error", "You must be in a vehicle to roll down the window!", new NewNotificationMessageContent[0]));
                return;
            }
            if (!isWindowDown)
            {
                _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled up your back windows!", new NewNotificationMessageContent[0]));
                API.FixVehicleWindow(vehicle, 2);
                API.FixVehicleWindow(vehicle, 3);

                API.RollUpWindow(vehicle, 2);
                API.RollUpWindow(vehicle, 3);

                return;
            }
            _newNotificationOverlay.SendNotification(new NewNotificationMessage("Vehicle Window", "success", "You have rolled down your back windows!", new NewNotificationMessageContent[0]));
            API.RollDownWindow(vehicle, 2);
            API.RollDownWindow(vehicle, 3);

            API.SmashVehicleWindow(vehicle, 2);
            API.SmashVehicleWindow(vehicle, 3);

        }


        #region Clamp Script

        /*
        private async void OnClamp()
        {
            var player = Game.PlayerPed;
            int vehicleToClamp = API.GetClosestVehicle(player.Position.X, player.Position.Y, player.Position.Z, 25, 0, 70);
            var coords = API.GetEntityCoords(player.Handle, false);
            API.RequestModel((uint)API.GetHashKey("prop_clamp"));

            if (API.IsPedInAnyVehicle(player.Handle, true))
            {
                _notifications.Error("Vehicle Clamp", "You can't apply a clamp while in a car, who do you think you are?");
                return;
            }

            if (!API.DecorGetBool(vehicleToClamp, "clampStatus"))
            {
                API.SetNetworkIdExistsOnAllMachines(vehicleToClamp, true);
                clampObject = API.CreateObject(API.GetHashKey("prop_clamp"), coords.X, coords.Y, coords.Z, true, true, true);
                API.SetNetworkIdExistsOnAllMachines(clampObject, true);
                var boneIndex = API.GetEntityBoneIndexByName(vehicleToClamp, "wheel_lf");
                API.SetEntityHeading(clampObject, 0f);
                API.SetEntityRotation(clampObject, 60f, 20f, 10f, 1, true);
                API.AttachEntityToEntity(clampObject, vehicleToClamp, boneIndex, -0.10f, 0.15f, -0.30f, 180f, 200f, 90f, true, true, false, false, 2, true);
                API.SetEntityRotation(clampObject, 60f, 20f, 10f, 1, true);
                API.SetEntityAsMissionEntity(clampObject, true, true);
                API.FreezeEntityPosition(clampObject, true);

                API.FreezeEntityPosition(vehicleToClamp, true);
                _notifications.Success("Vehicle Clamp", "Vehicle has been clamped!");

                API.DecorSetBool(vehicleToClamp, "clampStatus", true);

                //await Delay(500);
                //_notifications.Info("Vehicle Clamp", "Enter vehicle to apply handbrake!");

                //while (!API.IsPedInVehicle(player.Handle, vehicleToClamp, false))
                //{
                //    await Delay(500);
                //}
                //_notifications.Success("Vehicle Clamp", "Handbrake applied, clamp successful!");
                return;

            }

            API.DecorSetBool(vehicleToClamp, "clmapStatus", false);
            API.DeleteEntity(ref clampObject);
            API.DeleteObject(ref clampObject);
            API.FreezeEntityPosition(vehicleToClamp, false);
            _notifications.Error("Vehicle Clamp", "Vehicle clamp has been removed!");
        }

        private async Task VehicleClampedCheck()
        {

            await Delay(100);
            var player = Game.PlayerPed.Handle;
            var vehicleIn = API.GetVehiclePedIsIn(player, false);

            if (!API.IsPedInAnyVehicle(player, true)) return;

            API.LoadAllObjectsNow();

            if (!API.DecorGetBool(vehicleIn, "clampStatus"))
            {
                await Delay(1000);
                API.FreezeEntityPosition(vehicleIn, false);
                API.DeleteEntity(ref clampObject);
                API.DeleteObject(ref clampObject);
                return;
            }

            API.FreezeEntityPosition(vehicleIn, true);
            _notifications.Error("Vehicle Clamp", "This vehicle is clamped. You ain't going anywhere!");
        }

        
        */

        #endregion

    }

}