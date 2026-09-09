using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Abstraction;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers
{
    public class DistantBlipsController : Controller
    {
        private readonly ILogger<DistantBlipsController> _log;
        private readonly IPlayerListAccessor _playerListAccessor;
        private readonly IPermissionService _perms;
        private readonly StateBagProxy<IList<DistantBlip>> _distantBlipState;
        private const int MaxSeatIndex = 6;

        private const int SpritePlayerDot = 57;
        private const int SpriteSiren = 42;
        private const int SpriteInvalid = 303;
        private const int SpriteHelicopter = 43;

        private readonly Timer _timer = new Timer(5000)
        {
            AutoReset = true,
            Enabled = true
        };

        public DistantBlipsController(
            ILogger<DistantBlipsController> log,
            IPlayerListAccessor playerListAccessor,
            IPermissionService perms,
            IGlobalStateAccessor globalState)
        {
            _log = log;
            _playerListAccessor = playerListAccessor;
            _perms = perms;

            _distantBlipState = globalState.GlobalState.CreateProxy<IList<DistantBlip>>(GlobalStates.DistantBlips, true, Array.Empty<DistantBlip>());

            _timer.Elapsed += UpdatePlayerBlips;
        }

        private async void UpdatePlayerBlips(object sender, ElapsedEventArgs e)
        {
            try
            {
                var players = _playerListAccessor.Players.ToList();
                var distantBlips = new List<DistantBlip>(players.Count);

                // Use Task.WhenAll to parallelize asynchronous operations
                var tasks = players.Select(async player =>
                {
                    var pedHandle = API.GetPlayerPed(player.Handle);
                    var ped = new Ped(pedHandle);
                    var state = GetBlipStateForPed(pedHandle);
                    var hidden = player.State.Get<bool>(PlayerStates.HideBlipState);
                    var callsign = player.State.Get<string>(PlayerStates.CallSign) ?? string.Empty;

                    if (state != DistantBlipState.None && !hidden)
                    {
                        var p = ped.Position;
                        distantBlips.Add(new DistantBlip()
                        {
                            PlayerServerHandle = player.Handle,
                            PlayerPedNetworkId = player.Character?.NetworkId ?? 0,
                            BlipName = $"[{callsign}] {player.Name}",
                            Position = new PmpVector3(p.X, p.Y, p.Z),
                            Scale = GetScaleForState(state),
                            Color = state == DistantBlipState.VehicleFlashingBlues ? PmpBlipColor.White : await GetColorForPlayer(player),
                            Sprite = GetBlipSpriteForPlayer(state)
                        });
                    }
                });

                // Wait for all tasks to complete
                await Task.WhenAll(tasks);

                _distantBlipState.Value = distantBlips;
            }
            catch (Exception ex)
            {
                _log.Error("Error occuren creating distant blips", ex);
            }
        }

        private DistantBlipState GetBlipStateForPed(int pedHandle)
        {
            var pedVehicle = API.GetVehiclePedIsIn(pedHandle, false);
            if (pedVehicle == 0)
            {
                return DistantBlipState.OnFoot;
            }
            else
            {
                // Is this ped at the lowest seat index?
            for (int i = -1; i < MaxSeatIndex; i++)
            {
                var pedInSeat = API.GetPedInVehicleSeat(pedVehicle, i);
                if (pedInSeat != 0 && pedInSeat != pedHandle)
                {
                    return DistantBlipState.None;
                }

                if (pedInSeat == pedHandle)
                {

                    var vehicleClass = API.GetVehicleType(pedVehicle);
                    if (vehicleClass == "heli")
                    {
                        return DistantBlipState.VehicleHelicopter;
                    }

                    return API.IsVehicleSirenOn(pedVehicle)
                        ? DistantBlipState.VehicleFlashingBlues
                        : DistantBlipState.VehicleNormal;
                    }
                }
            }

            return DistantBlipState.None;
        }

        private float GetScaleForState(DistantBlipState state)
        {
            switch (state)
            {
                case DistantBlipState.VehicleNormal:
                case DistantBlipState.VehicleFlashingBlues:
                case DistantBlipState.VehicleHelicopter:
                    return 0.6f;

                case DistantBlipState.OnFoot:
                default:
                    return 0.4f;

            }
        }

        private async Task<PmpBlipColor> GetColorForPlayer(Player player)
        {
            var role = await _perms.GetUserRole(player);
            switch (role.Branch)
            {
                case UserBranch.Police:
                    return PmpBlipColor.Blue;
                case UserBranch.Fire:
                    return PmpBlipColor.Red;
                case UserBranch.Nhs:
                    return PmpBlipColor.ForestGreen;
                case UserBranch.Highways:
                    return PmpBlipColor.Orange2;
                case UserBranch.Control:
                default:
                    return PmpBlipColor.White;
            }
        }

        private int GetBlipSpriteForPlayer(DistantBlipState state)
        {
            switch (state)
            {
                case DistantBlipState.OnFoot:
                    return SpritePlayerDot;

                case DistantBlipState.VehicleNormal:
                    return SpritePlayerDot;

                case DistantBlipState.VehicleHelicopter:
                    return SpriteHelicopter;

                case DistantBlipState.VehicleFlashingBlues:
                    return SpriteSiren;

                default:
                    return SpriteInvalid;
            }
        }
    }
}
