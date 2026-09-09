using System;
using System.Collections.Generic;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Extensions;
using CitizenFX.Core.UI;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared.Commands;
using System.Text;

namespace PoliceMP.Client.Scripts
{
    public enum MovementStates
    {
        Normal,
        Injured,
        VeryInjured,
        Crouched,
        Cuffed,
    }

    public class MovementHandler : Script
    {
        private readonly ITickManager _ticks;
        private readonly IGameInputManager _input;
        private readonly ILogger<MovementHandler> _logger;
        private readonly INotificationService _notifications;
        private readonly ICommandManager _commandManager;
        public static MovementStates _currentState = MovementStates.Normal;

        public static string WalkingStyle;
        public static string currentStyle = "Normal";

        public MovementHandler(ITickManager ticks, IGameInputManager input, ILogger<MovementHandler> logger, ICommandManager commandManager, INotificationService notifications)
        {
            _ticks = ticks;
            _input = input;
            _logger = logger;
            _commandManager = commandManager;
            _notifications = notifications;
        }


        protected override async Task OnStartAsync()
        {
            _ticks.On(MovementActions);
            _ticks.On(MovementStateDecider);
        }


        private async Task MovementActions()
        {
            var player = Game.PlayerPed.Handle;

            API.RequestClipSet("move_m@drunk@a");
            API.RequestClipSet("move_m@buzzed");
            API.RequestClipSet("move_m@brave");
            API.RequestClipSet("move_heist_lester");
            API.RequestClipSet("move_lester_caneup");
            API.RequestClipSet("move_f@arrogant@a");

            /*if (API.IsEntityPlayingAnim(player, "anim@move_m@prisoner_cuffed", "idle", 49)
                && _currentState != MovementStates.Cuffed)
            {
                Game.PlayerPed.Task.ClearAll();
            }*/

            if (Game.PlayerPed.IsAiming)
            {
                await Delay(1000);
            }

            switch (_currentState)
            {
                case MovementStates.Normal:
                    {
                        API.ResetPedWeaponMovementClipset(player);
                        API.ResetPedMovementClipset(player, 1f);
                        API.ResetPedStrafeClipset(player);
                        API.SetPedMovementClipset(player, WalkingStyle, 1f);
                        API.SetPedUsingActionMode(player, false, -1, WalkingStyle);
                        API.SetPedMoveRateOverride(player, 1f);
                    }
                    break;
                case MovementStates.VeryInjured:
                    {
                        API.RequestAnimSet("move_injured_generic");
                        API.SetPedMovementClipset(player, "move_injured_generic", 1f);
                        API.SetPedMoveRateOverride(player, 0.6f);
                    }
                    break;
                case MovementStates.Injured:
                    {
                        API.RequestAnimSet("move_injured_generic");
                        API.SetPedMovementClipset(player, "move_injured_generic", 1f);
                    }
                    break;
                case MovementStates.Crouched:
                    {
                        API.RequestAnimSet("move_ped_crouched");
                        API.RequestAnimSet("move_ped_crouched_strafing");
                        API.SetPedMovementClipset(player, "move_ped_crouched", 0.45f);
                        API.SetPedStrafeClipset(player, "move_ped_crouched_strafing");
                        break;
                    }
                case MovementStates.Cuffed:
                    const string animName = "idle";
                    const string animDict = "anim@move_m@prisoner_cuffed";
                    API.RequestAnimDict(animDict);
                    if (API.IsEntityPlayingAnim(player, "anim@move_m@prisoner_cuffed", "idle", 49))
                    {
                        return;
                    }
                    API.TaskPlayAnim(player, animDict, animName, 10000000f, 100000000f, -1, 49, 0.0f, false, false, false);
                    await Delay(800);
                    break;
                default:
                    API.ResetPedMovementClipset(player, 1f);
                    break;
            }
        }

        private async Task MovementStateDecider()
        {
            API.DisableControlAction(0, 36, true);
            API.DisableControlAction(1, 36, true);
            API.DisableControlAction(2, 36, true);

            var player = Game.PlayerPed;

            /*
            if (Game.PlayerPed.IsCuffed)
            {
                _currentState = MovementStates.Cuffed;
                return;
            }
            */

            switch (player.Health)
            {
                case < 45:
                    _currentState = MovementStates.VeryInjured;
                    return;
                case < 75:
                    _currentState = MovementStates.Injured;
                    return;
            }

            if (_input.IsPressed(Control.Duck))
            {
                if (API.IsPedInAnyVehicle(player.Handle, true)) return;
                if (API.IsPedInAnyBoat(player.Handle)) return;
                if (API.IsPedInAnyPlane(player.Handle)) return;
                if (API.IsPedInAnyHeli(player.Handle)) return;

                if (_currentState == MovementStates.Crouched)
                {
                    _currentState = MovementStates.Normal;
                    await Delay(10);
                    return;
                }
                _currentState = MovementStates.Crouched;
                await Delay(10);
                return;
            }

            if (player.Health > 75 && _currentState != MovementStates.Crouched)
            {
                _currentState = MovementStates.Normal;
                return;
            }

            await Delay(0);
        }
    }
}