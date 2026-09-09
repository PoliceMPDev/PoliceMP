using System;
using System.CodeDom;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Actions.TacklePed;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using Weapon = CitizenFX.Core.Weapon;

namespace PoliceMP.Client.Scripts.Tackle
{
    public class TackleScript : Script
    {
        private readonly ILogger<TackleScript> _log;
        private readonly ITickManager _ticks;
        private readonly IActionManager _actions;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IGameInputManager _input;
        private readonly IPermissionService _perms;
        private readonly IInstructionalButtonsService _instructionalButtons;
        private const int TackleTime = 3000;
        private const string InstructionalButtonName = "TackleInstructionalButton";
        private DateTime _lastTackleTime = DateTime.MinValue;
        private readonly TimeSpan _tackleCooldown = TimeSpan.FromMinutes(3);

        public TackleScript(
            ILogger<TackleScript> log, 
            ITickManager ticks, 
            IActionManager actions, 
            ILegacyClientCommunicationsManager comms, 
            IGameInputManager input, 
            IPermissionService perms, 
            IInstructionalButtonsService instructionalButtons)
        {
            _log = log;
            _ticks = ticks;
            _actions = actions;
            _comms = comms;
            _input = input;
            _perms = perms;
            _instructionalButtons = instructionalButtons;

            API.AddTextEntry(InstructionalButtonName, "Tackle");
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(TackleScriptTick);
            _comms.On<int>(ClientEvents.TacklePed, HandleTacklePed);
            _comms.On<int>(ClientEvents.BeTackledBy, HandleBeTackledBy);
            return Task.FromResult(0);
        }

        private async Task HandleTacklePed(int targetNetworkId)
        {
            var target = Entity.FromNetworkId(targetNetworkId);
            if (target == null)
            {
                _log.Error($"Failed to tackler ped. Could not find entity with network Id {targetNetworkId}!");
                return;
            }

            if (target is not Ped targetPed)
            {
                _log.Error($"Failed to tackler ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new TacklePed(Game.PlayerPed, targetPed));
        }

        private async Task HandleBeTackledBy(int tacklerNetworkId)
        {
            var tackler = Entity.FromNetworkId(tacklerNetworkId);
            if (tackler == null)
            {
                _log.Error($"Failed to tackler ped. Could not find entity with network Id {tacklerNetworkId}!");
                return;
            }

            if (tackler is not Ped tacklerPed)
            {
                _log.Error($"Failed to tackler ped. Entity is not a ped!");
                return;
            }

            await _actions.Execute(new TacklePed(tacklerPed, Game.PlayerPed));
        }

        private async Task SetHelp(bool value)
        {
            if (value)
            {
                if (!_instructionalButtons.IsButtonShown(InstructionalButtonName))
                {
                    await _instructionalButtons.AddInstructionalButton(InstructionalButtonName, Control.Attack);
                }
            }
            else if (_instructionalButtons.IsButtonShown(InstructionalButtonName))
            {
                await _instructionalButtons.RemoveInstructionalButton(InstructionalButtonName);
            }
        }

        private async Task TackleScriptTick()
        {
            bool shouldDisplayHelp = false;
            var peds = World.GetAllPeds();
            API.SetRunSprintMultiplierForPlayer(API.GetPlayerIndex(), 1.01f);

            for (int i = 0; i < peds.Length; i++)
            {
                var ped = peds[i];
                var isPlayer = ped.IsPlayer;
                var distance = World.GetDistance(ped.Position, Game.PlayerPed.Position);


                if (ped != Game.PlayerPed
                    && ped.IsHuman
                    && !ped.IsAttached()
                    && (Game.PlayerPed.IsRunning || Game.PlayerPed.IsSprinting)
                    && ped.IsOnFoot && !ped.IsRagdoll)
                {
                    var pedIsCiv = ped.IsPlayer && ped.State.Get<UserRole>(PlayerStates.CurrentRole)?.Branch == UserBranch.Civ;
                    if (!shouldDisplayHelp && distance < 1.5 && pedIsCiv)
                    {
                        shouldDisplayHelp = true;
                    }

                    if ((!ped.IsPlayer && (ped.GetConfigFlag((int)PedConfigFlags.BumpedByPlayer))
                         || distance < 1.0f))
                    {
                        var shouldTackle = _input.IsBeingHeld(Control.Attack)
                                           && (Game.PlayerPed.Weapons.Current.Hash == WeaponHash.Unarmed || !_input.IsBeingHeld(Control.Aim))
                                           && (!isPlayer || pedIsCiv);

                        if (shouldTackle)
                        {
                            var direction = ped.Position - Game.PlayerPed.Position;
                            direction.Normalize();
                            shouldDisplayHelp = false;
                            await SetHelp(false);

                            if (Vector3.Dot(Game.PlayerPed.Rotation, direction) > 0f)
                            {
                                if (!_perms.GetUserAces().Result.IsAdmin && DateTime.UtcNow - _lastTackleTime < _tackleCooldown)
                                {
                                    return;
                                }

                                if (!_perms.GetUserAces().Result.IsAdmin)
                                {
                                    _lastTackleTime = DateTime.UtcNow;
                                }

                                if (ped.IsPlayer)
                                {
                                    if (await _comms.Request<bool>(ServerEvents.PlayerTacklePlayer, ped.NetworkId))
                                    {
                                        await Script.Delay(3000);
                                    }
                                }
                                else
                                {
                                    if (await ped.TryRequestNetworkEntityControl(timeoutMs: 500))
                                    {
                                        await _actions.Execute(new TacklePed(Game.PlayerPed, ped));
                                    }
                                }
                            }

                            return;
                        }
                    }
                }
            }

            await SetHelp(shouldDisplayHelp);
        }
    }
}
