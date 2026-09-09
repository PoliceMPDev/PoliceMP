using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Scripts.HideBlips;
using PoliceMP.Client.Scripts.PlayerControllerScript;
using PoliceMP.Client.Services;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using Color = System.Drawing.Color;
using Font = CitizenFX.Core.UI.Font;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Client.Scripts.PlayerTags
{
    public class PlayerTagsScript : Script
    {
        private readonly ILogger<PlayerTagsScript> _log;
        private readonly IPlayerService _players;
        private readonly IHideBlipsScript _hideBlips;
        private readonly IPermissionService _permissionService;
        private readonly IFeatureService _featureService;

        private const string TOGGLE_COMMAND = "playertags.toggle";
        private const string TOGGLE_SELF_COMMAND = "playertags.toggleself";
        private const string SET_SCALE_COMMAND = "playertags.setscale";

        private const float MAX_DISTANCE = 20.0f;
        private const float HEIGHT_ABOVE_ENTITY = 1.1f;
        private readonly SizeF _speechBubbleOffset = new(0.0f, -10.0f);

        private readonly PlayerStateBagProxy<bool> _enabled = new(Game.Player, PlayerStates.PlayerTags.Enabled, defaultValue: true);
        private readonly PlayerStateBagProxy<bool> _selfEnabled = new(Game.Player, PlayerStates.PlayerTags.SelfEnabled, defaultValue: false);
        private readonly PlayerStateBagProxy<float> _scale = new(Game.Player, PlayerStates.PlayerTags.Scale, defaultValue: 1.0f);

        private readonly Color _normalColor = Color.FromArgb(255, 255, 255, 255);
        private readonly Color _hiddenColor = Color.FromArgb(255, 180, 180, 255);

        private bool _allowPlayerBlips = true;

        private UserAces _userAces;


        public PlayerTagsScript(
            ILogger<PlayerTagsScript> log,
            ITickManager ticks,
            ICommandManager commands,
            IPlayerService players,
            IHideBlipsScript hideBlips,
            IPermissionService permissionService,
            IFeatureService featureService)

        {
            _log = log;
            _players = players;
            _hideBlips = hideBlips;
            _permissionService = permissionService;
            _featureService = featureService;

            commands.Register(TOGGLE_COMMAND).WithHandler(HandleToggle);
            commands.Register(TOGGLE_SELF_COMMAND).WithHandler(HandleToggleSelf);
            commands.Register(SET_SCALE_COMMAND).WithHandler(HandleSetScale);
            ticks.On(PlayerTagsTick);
            ticks.On(PlayerBlipsTick);
            ticks.On(CheckAllowBlipsTick);
            _allowPlayerBlips = _featureService.IsFeatureEnabled(FeatureToggle.AllowBlips);
        }

        private async Task CheckAllowBlipsTick()
        {
            await Script.Delay(TimeSpan.FromSeconds(5000).Milliseconds);
            _allowPlayerBlips = _featureService.IsFeatureEnabled(FeatureToggle.AllowBlips);
           
        }

        private void HandleToggle()
        {
            _enabled.Value = !_enabled.Value;
        }

        private void HandleToggleSelf()
        {
            _selfEnabled.Value = !_selfEnabled.Value;
            //_log.Debug($"Playertags Self Enabled: {_selfEnabled.Value}");
        }

        private void HandleSetScale(string scaleString)
        {
            if (!float.TryParse(scaleString, out var scale))
            {
                //TODO: Chat service
                return;
            }

            _scale.Value = Math.Max(scale, 0);
        }

        private async Task PlayerBlipsTick()
    {
        var role = _permissionService.CurrentUserRole;

        if (role == null)
        {
            await Script.Delay(TimeSpan.FromSeconds(70000).Milliseconds);
            return;
        }
        
        if (role.Branch == UserBranch.Civ) return;

        if (!_allowPlayerBlips) return;

        var players = API.GetActivePlayers();
        var cachedPlayers = await _players.FetchAllRecentPlayerInfo();

        for (int i = 0; i < players.Count; i++)
        {
            int handle = players[i];

            var player = new Player(handle);
            if (player.Character == null) continue;
            var ped = player.Character;
            var hidden = _hideBlips.AreBlipsHiddenForPed(player);

            var pl = cachedPlayers.FirstOrDefault(pi => pi.NetworkId == ped.NetworkId);
            if (pl == null) continue;

            var name = $"[{pl.CallSign ?? string.Empty}] {pl.Name}";
            if (hidden)
            {
                if (ped.AttachedBlips.Any())
                {
                    foreach (var blip in ped.AttachedBlips)
                    {
                        blip.Delete();
                    }
                }
                continue;
            }

            if (ped == Game.PlayerPed || ped.Exists() == false) continue;


            var pedBlip = ped?.AttachedBlip;
            var inVehicle = ped.IsInVehicle();
            if (inVehicle && ((ped.CurrentVehicle.Driver.Exists() && ped.CurrentVehicle.Driver != ped) ||
                            (Game.PlayerPed.Exists() && ped.CurrentVehicle == Game.PlayerPed.CurrentVehicle)))
            {
                pedBlip?.Delete();
            }
            else
            {
                pedBlip ??= ped.AttachBlip();
                BlipColor desiredColor;
                BlipSprite desiredSprite;

                    if (API.IsPedInAnyHeli(ped.Handle))
                    {
                        desiredSprite = (BlipSprite)422;

                        _userAces = await _permissionService.GetUserAces();
                        switch (pl.ActiveBranch)
                        {
                            case UserBranch.Police:
                                desiredColor = BlipColor.Blue;
                                break;
                            case UserBranch.Fire:
                                desiredColor = BlipColor.Red;
                                break;
                            case UserBranch.Nhs:
                            case UserBranch.Blood:
                                desiredColor = BlipColor.Green;
                                break;
                            case UserBranch.Highways:
                                desiredColor = BlipColor.TrevorOrange;
                                break;
                            case UserBranch.Control:
                                desiredColor = BlipColor.White;
                                break;
                            default:
                                desiredColor = BlipColor.Blue;
                                break;
                        }
                    }
                    else if (inVehicle && ped.CurrentVehicle.IsSirenActive)
                    {
                        desiredSprite = BlipSprite.PoliceCarDot;
                        desiredColor = BlipColor.White;
                    }
                    else
                    {
                        desiredSprite = inVehicle ? (BlipSprite)57 : BlipSprite.Player;
                        pedBlip.Rotation = (int)ped.Heading;

                        _userAces = await _permissionService.GetUserAces();

                        switch (pl.ActiveBranch)
                        {
                            case UserBranch.Police:
                                desiredColor = BlipColor.Blue;
                                break;
                            case UserBranch.Fire:
                                desiredColor = BlipColor.Red;
                                break;
                            case UserBranch.Nhs:
                                desiredColor = BlipColor.Green;
                                break;
                            case UserBranch.Blood:
                                desiredColor = BlipColor.Green;
                                break;
                            case UserBranch.Highways:
                                desiredColor = BlipColor.TrevorOrange;
                                break;
                            case UserBranch.Control:
                                desiredColor = BlipColor.White;
                                break;
                            default:
                                desiredColor = BlipColor.Blue;
                                break;
                        }
                    }

                    pedBlip.Name = name;
                    pedBlip.Scale = 0.6f;

                    if (pedBlip.Color != desiredColor)
                        pedBlip.Color = desiredColor;

                    if (pedBlip.Sprite != desiredSprite)
                        pedBlip.Sprite = desiredSprite;
                }
            }
        }

        private async Task PlayerTagsTick()
        {
            var enabled = _enabled.Value;
            var modEnabled = Game.Player.State.Get<bool>(PlayerStates.PlayerTags.ModOverride);
            var selfEnabled = _selfEnabled.Value;
            var scale = _scale.Value;

            if (_permissionService.CurrentUserRole == null) return;
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Civ)
                
            {
                await Script.Delay(TimeSpan.FromSeconds(70000).Milliseconds);
                return;
            }

            if (!(enabled || modEnabled))
            {
                //_log.Debug("Disabled");
                return;
            }

            var players = API.GetActivePlayers();
            var cachedPlayers = await _players.FetchAllRecentPlayerInfo();

            for (int i = 0; i < players.Count; i++)
            {
                int handle = players[i];

                var player = new Player(handle);
                if (player.Character == null) continue;
                var ped = player.Character;

                var pl = cachedPlayers.FirstOrDefault(pi => pi.NetworkId == ped.NetworkId);
                if (pl == null) continue;

                if (pl.ActiveBranch == UserBranch.Civ) continue;

                var name = pl.Name;

                if (!string.IsNullOrEmpty(pl.CallSign))
                {
                    name = $"[{pl.CallSign}] {pl.Name}";
                }

                var hidden = _hideBlips.AreBlipsHiddenForPed(player);

                if (!hidden && pl.ActiveBranch == UserBranch.Civ)
                {
                    hidden = true;
                }

                //_log.Debug($"Hidden: {hidden}; ModEnabled: {modEnabled}; {hidden && !modEnabled}");
                if (Game.Player.Handle == handle && !selfEnabled
                    || hidden && !modEnabled
                    || !ped.IsVisible
                    || !ped.IsOnScreen
                    || ped.IsOccluded
                    || API.IsPlayerSwitchInProgress()
                    || Screen.Fading.IsFadedOut)
                {
                    continue;
                }

                var camPos = API.GetGameplayCamCoord();


                var distance = World.GetDistance(camPos, ped.Position);
                if (distance > MAX_DISTANCE && !modEnabled)
                    continue;

                var curVehicle = ped.CurrentVehicle;
                if (curVehicle != null && curVehicle == Game.PlayerPed.CurrentVehicle)
                    continue;

                var pos = curVehicle != null
                    ? ped.Bones[Bone.SKEL_ROOT].Position
                    : ped.Position;

                pos.Z += HEIGHT_ABOVE_ENTITY + (scale * 0.1f);

                DrawTag(player, pos, distance, scale, hidden, name);
            }
        }

        private void DrawTag(Player player, Vector3 position, float distance, float scale, bool hidden, string playerName)
        {
            float screenX = 0.0f;
            float screenY = 0.0f;

            var distanceScale = (1 / distance) * (100.0f / MathUtil.Clamp(API.GetGameplayCamFov(), 0.0f, 100.0f)) * scale;

            if (!API.World3dToScreen2d(position.X, position.Y, position.Z, ref screenX, ref screenY))
                return;

            var screenPos = new PointF(screenX * Screen.Width, screenY * Screen.Height);
            var name = new Text(hidden ? $"{playerName} 🚷" : playerName, screenPos, distanceScale)
            {
                Font = Font.ChaletLondon,
                Color = hidden ? _hiddenColor : _normalColor,
                Shadow = true,
                Outline = true,
                Centered = true
            };

            name.Draw();

            if (API.NetworkIsPlayerTalking(player.Handle))
            {
                var speech = new Text("💬", screenPos, distanceScale)
                {
                    Font = Font.ChaletLondon,
                };

                speech.Draw(new SizeF(
                    name.Width / 2 + _speechBubbleOffset.Width * scale,
                    _speechBubbleOffset.Height * scale));
            }
        }
    }
}
