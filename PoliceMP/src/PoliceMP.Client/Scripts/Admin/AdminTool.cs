using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Admin
{
    public class AdminTool : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<AdminTool> _logger;
        private readonly ICommandManager _commands;
        private readonly INotificationService _notifications;
        private readonly ITickManager _ticks;
        private readonly ICommonFunctionsService _common;

        private bool _ray_permission = false;

        public AdminTool(ILegacyClientCommunicationsManager comms, ILogger<AdminTool> logger,
            INotificationService notifications, ICommandManager commands, ITickManager ticks, ICommonFunctionsService common)
        {
            _comms = comms;
            _logger = logger;
            _notifications = notifications;
            _commands = commands;
            _ticks = ticks;
            _common = common;
        }

        protected override async Task OnStartAsync()
        {
            _ray_permission = await _comms.Request<bool>(ServerEvents.RequestRayPerms);
            _comms.On(ClientEvents.TellPlayerToFling, FlingMyself);
            _ticks.On(AdminToolTick);
            _ticks.Off(ModPopupTick);

            _comms.On(ClientEvents.AdminSendPopupClient, (string playerName) =>
            {
                SendPopup(playerName);
            });
        }

        private async void SendPopup(string playerName)
        {
            var playerID = API.PlayerId();
            var localPlayerName = API.GetPlayerName(playerID);

            if (localPlayerName != playerName) return;

            _ticks.On(ModPopupTick);
            API.ExecuteCommand("[ADMIN WARNING] This player has recieved the Mod popup screen!");

            var loop = 20;
            while (loop > 0)
            {
                API.PlaySoundFrontend(-1, "Beep_Red", "DLC_HEIST_HACKING_SNAKE_SOUNDS", false); await Delay(250);
                loop = loop - 1;
            }
            await Delay(20000);
            _ticks.Off(ModPopupTick);
        }

        private async Task ModPopupTick()
        {
            API.DrawSprite("", "", 0.5f, 0.552f, 0.550f, 0.49752f, 0, 255, 0, 0, 255); //Main Border
            API.DrawSprite("", "", 0.5f, 0.552f, 0.540f, 0.4802f, 0, 107, 0, 0, 255); //Main Box

            Text titleText = new Text($"~h~Moderator Notice", new System.Drawing.PointF(497, 250), 0.9f);
            Text helloText = new Text($"Hello!", new System.Drawing.PointF(605, 325), 0.6f);
            Text messageText = new Text($"A member of the Moderation Team is now trying to speak with you,", new System.Drawing.PointF(315, 380), 0.52f);
            Text message2Text = new Text($"Ensure you have your microphone unmuted if you have one.", new System.Drawing.PointF(345, 405), 0.52f);
            Text message3Text = new Text($"Pay attention to what they are saying in local voice chat,", new System.Drawing.PointF(365, 430), 0.52f);
            Text message4Text = new Text($"and/or text chat to speak with you!", new System.Drawing.PointF(480, 455), 0.52f);
            Text closeText = new Text($"~italic~~HUD_COLOUR_4~this notification will close automatically.", new System.Drawing.PointF(490, 515), 0.4f);

            titleText.Draw();
            helloText.Draw();
            messageText.Draw();
            message2Text.Draw();
            message3Text.Draw();
            message4Text.Draw();
            closeText.Draw();
        }


        private async Task AdminToolTick()
        {
            uint interactionweapon = (uint)API.GetHashKey("weapon_pistol50");
            uint weaponhash = 0;
            API.GetCurrentPedWeapon(Game.PlayerPed.Handle, ref weaponhash, true);

            if (_ray_permission)
            {
                if (!API.HasPedGotWeapon(Game.PlayerPed.Handle, interactionweapon, false) && weaponhash != interactionweapon)
                {
                    Game.PlayerPed.Weapons.Give(WeaponHash.Pistol50, 100, false, true);
                }
            }
            else
            {
                Game.PlayerPed.Weapons.Remove(WeaponHash.Pistol50);
                return;
            }

            if (weaponhash != interactionweapon) { return; }

            if (!API.IsPlayerFreeAiming(Game.Player.Handle)) return;

            int entity = 0;
            if (!API.GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref entity)) return;

            _logger.Debug($"Entity: {entity}");
            if (entity == 0) return;

            if (!API.DoesEntityExist(entity)) return;
            Entity e = Entity.FromHandle(entity);
            _logger.Debug($"Model hash: {e.Model.Hash}");
            
            //Home Key Pressed [Yeet Thing]
            if (API.IsControlPressed(0, 212))
            {
                _logger.Debug("Keypress");
                if (API.IsEntityAPed(entity) && API.IsPedAPlayer(entity))
                {
                    _comms.ToServer(ServerEvents.RequestPlayerToFling, e.NetworkId);
                    return;
                }

                if (!await _common.RequestNetworkEntityControl(e.NetworkId)) return;

                if (API.IsEntityAPed(e.Handle))
                {
                    Ped p = (Ped)Entity.FromHandle(e.Handle);
                    if (p.IsInVehicle())
                    {
                        p.CurrentVehicle.Velocity = new Vector3(0, 0, 100f);
                        return;
                    }
                }
                Vector3 V = e.Velocity;
                V.Z = 100f;
                e.Velocity = V;
            }
        }

        private void FlingMyself()
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                Game.PlayerPed.CurrentVehicle.Velocity = new Vector3(0, 0, 100f);
                return;
            }
            Game.PlayerPed.Velocity = new Vector3(0, 0, 100f);
        }
    }
}