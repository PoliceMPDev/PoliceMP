using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Scripts.Commands
{
    public class DriftScript : Script
    {
        private readonly ICommandManager _command;
        private readonly IPermissionService _permission;
        private readonly INotificationService _notification;

        private UserAces _aces;
        
        public DriftScript(ICommandManager command, IPermissionService permission, INotificationService notification)
        {
            _command = command;
            _permission = permission;
            _notification = notification;
        }

        protected override Task OnStartAsync()
        {
            _command.Register("drift").WithHandler(ToggleDrift);
            _aces = _permission.GetUserAces().Result;
            return Task.FromResult(0);
        }

        private void ToggleDrift()
        {
            if (!_aces.IsDeveloper && _permission.CurrentUserRole.Branch != UserBranch.Civ)
            {
                _notification.Error("Drift Mode", "You aren't allowed to use this.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle == null || Game.PlayerPed.CurrentVehicle.Driver != Game.PlayerPed)
            {
                _notification.Error("Drift Mode", "You can't use this right now");
                return;
                
            }
            
            
        }
    }
}