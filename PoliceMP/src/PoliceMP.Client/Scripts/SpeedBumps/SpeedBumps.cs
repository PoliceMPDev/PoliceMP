using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Scripts.Spawn;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;

namespace PoliceMP.Client.Scripts.SpeedBumps
{
    public class SpeedBumps : Script
    {
        private readonly ILogger<SpeedBumps> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ICommonFunctionsService _common;
        private SpeedBumpOptions _options;

        private List<SpeedHumpLocation> SpeedHumps = new List<SpeedHumpLocation>();

        private string SPEED_BUMP_MODEL = "prop_speedbumps";

        public SpeedBumps(ILogger<SpeedBumps> logger, ITickManager ticks, ILegacyClientCommunicationsManager comms, ICommonFunctionsService common)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _common = common;
        }

        protected override async Task OnStartAsync()
        {
            _options = await _comms.Request<SpeedBumpOptions>(SpeedBumpNetworkConstants.GetOptions);

            foreach (SpeedHumpLocation loc in _options.Locations)
            {
                SpeedHumps.Add(loc);
            }

            if (!API.IsModelInCdimage((uint)API.GetHashKey(SPEED_BUMP_MODEL)))
            {
                _logger.Error($"ERROR! This model, {SPEED_BUMP_MODEL} is not valid! This code will never work.");
                return;
            }
            else
            {
                API.RequestModel((uint)API.GetHashKey(SPEED_BUMP_MODEL));
                while (!API.HasModelLoaded((uint)API.GetHashKey(SPEED_BUMP_MODEL)))
                {
                    _logger.Trace($"Waiting for model {SPEED_BUMP_MODEL} to load...");
                    await Script.Delay(100);
                }
            }

            lock (SpeedHumps)
            {
                foreach (SpeedHumpLocation Loc in SpeedHumps)
                {
                    var prop = API.CreateObject(API.GetHashKey(SPEED_BUMP_MODEL), Loc.Position.X, Loc.Position.Y, Loc.Position.Z - 1.1f, false, false, false);
                    API.FreezeEntityPosition(prop, true);
                    API.SetEntityHeading(prop, Loc.Heading);
                    API.FreezeEntityPosition(prop, true);
                    API.RemoveVehicleShadowEffect(prop);
                    Loc.Handle = prop;
                }
            }

            if (SpeedHumps.Any())
                _ticks.On(SpeedBumpsTick);
        }

        private async Task SpeedBumpsTick()
        {
            var playerVehicle = Game.PlayerPed.CurrentVehicle;

            if (playerVehicle == null)
            {
                await Delay(0);
                return;
            }

            float mph = playerVehicle.Speed * 2.23694f;

            var startingHealth = Game.PlayerPed.Health;
            if (IsEntityNearAnySpeedBump(playerVehicle))
            {
                if (mph > 50)
                    playerVehicle.Velocity = new CitizenFX.Core.Vector3(playerVehicle.Velocity.X * 0.8f, playerVehicle.Velocity.Y * 0.8f, playerVehicle.Velocity.Z + playerVehicle.Speed * 0.02f);

                Game.PlayerPed.Health = startingHealth;
                
                if (mph >= 80)
                {
                    BurstTyre(playerVehicle, 2);

                    if (mph > 95)
                    {
                        API.SetEntityHealth(playerVehicle.Handle, 0);
                    }
                }
            }
        }

        private bool IsEntityNearAnySpeedBump(Vehicle vehicle)
        {
            foreach (SpeedHumpLocation Loc in SpeedHumps)
            {
                if (!API.IsEntityAnObject(Loc.Handle))
                    continue;

                if (World.GetDistance(vehicle.Position, Loc.Position.ToCitizenVector3()) <= 2.5f)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task BurstTyre(Vehicle vehicle, int Quantity)
        {
            if (!await _common.RequestNetworkEntityControl(vehicle.NetworkId)) return;
            for (int i = 0; i <= Quantity; i++)
            {
                API.SetVehicleTyreBurst(vehicle.Handle, i, true, 1000);
            }
        }
    }
}