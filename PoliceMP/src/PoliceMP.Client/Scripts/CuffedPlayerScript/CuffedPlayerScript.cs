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

namespace PoliceMP.Client.Scripts.CuffedPlayerScript
{
    public class CuffedPlayerScript : Script
    {
        private readonly ICommandManager _commandManager;
        private readonly ITickManager _ticks;
        private readonly IPermissionService _permissionService;
        private readonly INewNotificationOverlay _newNotificationOverlay;

        public CuffedPlayerScript(ICommandManager commandManager, ITickManager ticks, IPermissionService permissionService, INewNotificationOverlay newNotificationOverlay)
        {
            _commandManager = commandManager;
            _newNotificationOverlay = newNotificationOverlay;
            _permissionService = permissionService;
            _ticks = ticks;
        }

        protected override async Task OnStartAsync()
        {
            API.DecorRegister("PoliceMP_CuffPlayer_State", 3);
            _ticks.On(CuffedCheck);
            _ticks.Off(CuffedTick);
        }

        // DECOR INT STATES FOR CUFFED PLAYERS
        // 0 - UNCUFFED, NORMAL
        // 1 - INITIATE CUFF
        // 2 - ACTIVELY CUFFED
        // 3 - INITATE UNCUFF

        private async Task CuffedCheck()
        {
            await Delay(1000);
            var currentUserRole = _permissionService.CurrentUserRole;
            if (currentUserRole == null) return;
            if (currentUserRole.Branch != UserBranch.Civ) return;
            var player = Game.PlayerPed.Handle;

            if (!API.DecorExistOn(player, "PoliceMP_CuffPlayer_State"))
            {
                API.DecorSetInt(player, "PoliceMP_CuffPlayer_State", 0);
                Debug.WriteLine("set local player to PoliceMP_CuffPlayer_State : 0, for the first time");
            }

            var cuffedState = API.DecorGetInt(player, "PoliceMP_CuffPlayer_State");

            switch (cuffedState)
            {
                case 1:
                    API.DecorSetInt(player, "PoliceMP_CuffPlayer_State", 2);
                    Debug.WriteLine("set local player to PoliceMP_CuffPlayer_State : 2");
                    API.FreezeEntityPosition(player, true);
                    await Delay(2000);
                    API.FreezeEntityPosition(player, false);
                    _ticks.On(CuffedTick);
                    break;
                case 3:
                    API.DecorSetInt(player, "PoliceMP_CuffPlayer_State", 0);
                    Debug.WriteLine("set local player to PoliceMP_CuffPlayer_State : 0");
                    API.FreezeEntityPosition(player, true);
                    await Delay(2000);
                    API.FreezeEntityPosition(player, false);
                    _ticks.Off(CuffedTick);
                    break;
            }
        }

        private async Task CuffedTick()
        {
            await Delay(800);
            var player = Game.PlayerPed.Handle;
            const string animName = "idle";
            const string animDict = "anim@move_m@prisoner_cuffed";
            API.RequestAnimDict(animDict);
            if (API.IsEntityPlayingAnim(player, "anim@move_m@prisoner_cuffed", "idle", 49))
            {
                return;
            }
            Debug.WriteLine("playing anim for PoliceMP_CuffPlayer_State");
            API.TaskPlayAnim(player, animDict, animName, 10000000f, 100000000f, -1, 49, 0.0f, false, false, false);
        }
    }
}