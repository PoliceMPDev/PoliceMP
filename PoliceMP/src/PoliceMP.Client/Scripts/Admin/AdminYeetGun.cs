using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.Admin
{
    public class AdminYeetGun : Script
    {
        private readonly ILogger<AdminYeetGun> _log;
        private readonly IGameInputManager _gameInput;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _perms;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private UserAces _aces;

        public AdminYeetGun(ILogger<AdminYeetGun> log, IGameInputManager gameInput, ITickManager ticks, IPermissionService perms, ILegacyClientCommunicationsManager comms, INotificationService notifications)
        {
            _log = log;
            _gameInput = gameInput;
            _ticks = ticks;
            _perms = perms;
            _comms = comms;
            _notifications = notifications;
        }

        bool IsAllowedToUseGun()
            => _aces?.IsAdmin == true 
            || _aces?.IsModerator == true 
            || _aces?.IsDeveloper == true
            || _aces?.IsTierTwo == true;

        protected override async Task OnStartAsync()
        {
            _aces = await _perms.GetUserAces();

            if(IsAllowedToUseGun())
            {
                _ticks.On(AdminYeetGunTick);
            }
        }


        private async Task AdminYeetGunTick()
        {
            var entity = Game.Player.GetTargetedEntity();

            if (entity == null)
                return;

            if (entity is Ped ped && ped.CurrentVehicle != null)
                entity = ped.CurrentVehicle;

            var isNetworked = API.NetworkGetEntityIsNetworked(entity.Handle);

            if (isNetworked && 
                (entity is Ped or Vehicle && _gameInput.IsJustPressed(Control.FrontendDelete)) 
                || (entity is Prop && _gameInput.IsJustBeingHeld(Control.FrontendDelete)))
            {
                if (!IsAllowedToUseGun())
                {
                    _log.Error("You are not authorized to use this feature!");
                    return;
                }

                if (!API.NetworkGetEntityIsNetworked(entity.Handle))
                {
                    _notifications.Warning("Not Networked", $"{entity.GetType().Name} {entity.Handle} was not networked! This might not be deleted on other people's clients!");
                    entity.Delete();
                    return;
                }

                if (API.NetworkGetEntityIsLocal(entity.Handle) ||
                    await entity.TryRequestNetworkEntityControl(true, 500))
                {
                    _comms.ToServer(ServerEvents.AdminYeetEntityLog, entity.NetworkId);
                    entity.Delete();
                    return;
                }

                _comms.ToServer(ServerEvents.AdminYeetEntity, entity.NetworkId);
            }

            if (_gameInput.IsJustPressed(Control.FrontendSocialClub) || _gameInput.IsJustPressed(Control.FrontendSocialClubSecondary))
            {
                if (!IsAllowedToUseGun())
                {
                    _log.Error("You are not authorized to use this feature!");
                    return;
                }

                if (isNetworked)
                {
                    var player = await _comms.Request<string>(ServerEvents.AdminGetNetworkFirstOwner, entity.NetworkId);
                    if (player != null)
                    {
                        _notifications.Info("Owner Identified", $"{entity.GetType().Name} {entity.NetworkId} was created by {player}.");
                    }
                    else
                    {
                        _notifications.Error("Failed", $"The creator of {entity.GetType().Name} {entity.NetworkId} is no longer on the server!");
                    }
                }
                else
                {
                    _notifications.Error("Failed", $"{entity.GetType().Name} {entity.Handle} is not networked.");
                }
                
            }
        }
    }
}
