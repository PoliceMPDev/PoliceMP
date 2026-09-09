/*

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

namespace PoliceMP.Client.Scripts
{
    class WalkingStyles : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly INotificationService _notifications;

        public WalkingStyles(ICommandManager commandManager, INotificationService notifications)
        {
            _commandManager = commandManager;
            _notifications = notifications;
        }

        protected override async Task OnStartAsync()
        {
            _commandManager.Register("walkreset").WithHandler(OnWalkReset);
            _commandManager.Register("walkdrunk").WithHandler(OnWalkDrunk);
            _commandManager.Register("walkdrunk2").WithHandler(OnWalkDrunk2);
            _commandManager.Register("walkbrave").WithHandler(OnWalkBrave);
            _commandManager.Register("walklester").WithHandler(OnWalkLester);
            _commandManager.Register("walklester2").WithHandler(OnWalkLester2);
            _commandManager.Register("walkarrogant").WithHandler(OnWalkArrogant);
        }

        private async Task OnWalkReset()
        {
            MovementHandler.WalkingStyle = "DEFAULT_ACTION";
            _notifications.Success("Walking Style", "You have reset your walking style!");
        }
        private async Task OnWalkDrunk()
        {
            MovementHandler.WalkingStyle = "move_m@drunk@a";
            _notifications.Success("Walking Style", "You have set your walking style to Drunk!");
        }
        private async Task OnWalkDrunk2()
        {
            MovementHandler.WalkingStyle = "move_m@buzzed";
            _notifications.Success("Walking Style", "You have set your walking style to Drunk2!");
        }
        private async Task OnWalkBrave()
        {
            MovementHandler.WalkingStyle = "move_m@brave";
            _notifications.Success("Walking Style", "You have set your walking style to Brave!");
        }
        private async Task OnWalkLester()
        {
            MovementHandler.WalkingStyle = "move_heist_lester";
            _notifications.Success("Walking Style", "You have set your walking style to Lester!");
        }
        private async Task OnWalkLester2()
        {
            MovementHandler.WalkingStyle = "move_lester_caneup";
            _notifications.Success("Walking Style", "You have set your walking style to Lester2!");
        }
        private async Task OnWalkArrogant()
        {
            MovementHandler.WalkingStyle = "move_f@arrogant@a";
            _notifications.Success("Walking Style", "You have set your walking style to Arrogant!");
        }

        // To add a new walking style, follow template below, ensure to add a command under the others to trigger it.


        private async Task OnWalkName()
        {
            MovementHandler.WalkingStyle = "CLIPSET ANIMATION NAME";
            _notifications.Success("Walking Style", "You have set your walking style to NAME!");
        }
    }
}
        /*/

