using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Server.Controllers
{
    public class PlayerVersusCivActionsController : Controller
    {
        private readonly ILogger<PlayerVersusCivActionsController> _log;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly IPermissionService _perms;

        public PlayerVersusCivActionsController(ILogger<PlayerVersusCivActionsController> log, ILegacyServerCommunicationsManager comms, IPermissionService perms)
        {
            _log = log;
            _comms = comms;
            _perms = perms;

            _comms.OnRequest<Player, int, bool>(ServerEvents.PlayerTacklePlayer, HandlePlayerTacklePlayer);
            _comms.OnRequest<Player, int, bool>(ServerEvents.PlayerArrestPlayer, HandlePlayerArrestPlayer);
            _comms.OnRequest<Player, int, bool>(ServerEvents.PlayerCPRPlayer, HandlePlayerCPRPlayer);
            _comms.OnRequest<Player, int, bool>(ServerEvents.PlayerDefibPlayer, HandlePlayerDefibPlayer);
        }

        private async Task<bool> HandlePlayerTacklePlayer(Player tackler, int victimNetworkId)
        {
            var victimPed = Entity.FromNetworkId(victimNetworkId);
            var victimPlayer = Players[API.NetworkGetEntityOwner(victimPed.Handle)];
            if (victimPlayer == null)
            {
                _log.Error($"Cannot find victim with Id {API.NetworkGetEntityOwner(victimNetworkId)}");
                return false;
            }

            var victimRole = await _perms.GetUserRole(victimPlayer);

            if (victimRole.Branch == UserBranch.Civ)
            {
                var tacklerPed = Entity.FromHandle(API.GetPlayerPed(tackler.Handle));

                if (Vector3.Distance(tacklerPed.Position, victimPed.Position) > 10f)
                {
                    _log.Debug($"{tackler.Name} tried to tackle {victimPlayer.Name} but is too far away!");
                    return false;
                }

                _log.Debug($"{tackler.Name} is tacking {victimPlayer.Name}!");
                _comms.ToClient(tackler, ClientEvents.TacklePed, victimNetworkId);
                _comms.ToClient(victimPlayer, ClientEvents.BeTackledBy, tacklerPed.NetworkId);
            }

            return true;
        }

        
        private async Task<bool> HandlePlayerArrestPlayer(Player arrester, int victimNetworkId)
        {
            var victimPed = Entity.FromNetworkId(victimNetworkId);
            var victimPlayer = Players[API.NetworkGetEntityOwner(victimPed.Handle)];
            if (victimPlayer == null)
            {
                _log.Error($"Cannot find victim with Id {API.NetworkGetEntityOwner(victimNetworkId)}");
                return false;
            }
            
            var victimRole = await _perms.GetUserRole(victimPlayer);

            if (victimRole.Branch == UserBranch.Civ)
            {
                var arresterPed = Entity.FromHandle(API.GetPlayerPed(arrester.Handle));

                if (Vector3.Distance(arresterPed.Position, victimPed.Position) > 10f)
                {
                    _log.Debug($"{arrester.Name} tried to arrest {victimPlayer.Name} but is too far away!");
                    return false;
                }

                _log.Debug($"{arrester.Name} is arresting {victimPlayer.Name}!");
                _comms.ToClient(arrester, ClientEvents.ArrestPlayerPed, victimNetworkId);
                _comms.ToClient(victimPlayer, ClientEvents.BeArrestedPlayerPed, arresterPed.NetworkId);
            }
            
            return true;
        }
        
        private async Task<bool> HandlePlayerCPRPlayer(Player subject, int victimNetworkId)
        {
            var victimPed = Entity.FromNetworkId(victimNetworkId);
            var victimPlayer = Players[API.NetworkGetEntityOwner(victimPed.Handle)];
            if (victimPlayer == null)
            {
                _log.Error($"Cannot find victim with Id {API.NetworkGetEntityOwner(victimNetworkId)}");
                return false;
            }
            

            var medicPed = Entity.FromHandle(API.GetPlayerPed(subject.Handle));

            if (Vector3.Distance(medicPed.Position, victimPed.Position) > 10f)
            {
                _log.Debug($"{subject.Name} tried to CPR {victimPlayer.Name} but is too far away!");
                return false;
            }

            _log.Debug($"{subject.Name} is CPRing {victimPlayer.Name}!");
            _comms.ToClient(subject, ClientEvents.CPRPlayerPed, victimNetworkId);
            _comms.ToClient(victimPlayer, ClientEvents.BeCPRByPlayerPed, medicPed.NetworkId);

            
            return true;
        }
        
        private async Task<bool> HandlePlayerDefibPlayer(Player subject, int victimNetworkId)
        {
            var victimPed = Entity.FromNetworkId(victimNetworkId);
            var victimPlayer = Players[API.NetworkGetEntityOwner(victimPed.Handle)];
            if (victimPlayer == null)
            {
                _log.Error($"Cannot find victim with Id {API.NetworkGetEntityOwner(victimNetworkId)}");
                return false;
            }
            

            var medicPed = Entity.FromHandle(API.GetPlayerPed(subject.Handle));

            if (Vector3.Distance(medicPed.Position, victimPed.Position) > 10f)
            {
                _log.Debug($"{subject.Name} tried to Defib {victimPlayer.Name} but is too far away!");
                return false;
            }

            _log.Debug($"{subject.Name} is Defibing {victimPlayer.Name}!");
            _comms.ToClient(subject, ClientEvents.DefibPlayerPed, victimNetworkId);
            _comms.ToClient(victimPlayer, ClientEvents.BeDefibByPlayerPed, medicPed.NetworkId);
            
            return true;
        }
    }
}
