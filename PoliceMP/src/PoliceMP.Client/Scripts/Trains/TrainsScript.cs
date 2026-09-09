using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;

namespace PoliceMP.Client.Scripts.Trains
{
    public class TrainsScript : Script
    {
        private readonly ILogger<TrainsScript> _logger;
        private readonly ITickManager _ticks;
        private readonly IClientCommunicationsManager _comms;
        private readonly ICommonFunctionsService _common;
        private readonly ICommandManager _commands;
        private readonly IPermissionService _userAces;
        private readonly ISoundService _soundService;

        private readonly Random _random = new Random();

        private const string DecorHoldTrain = "PoliceMPDecor:HoldTrainAtStation";
        private const int SecondsToHoldTrain = 50_000;
        private const int SecondsToAllowEscape = 150_000;

        #region Lists

        private readonly string[] _trains = { "metrotrain", "freight", "freightcar", "freightgrain", "freightcont1", "freightcont2", "freighttrailer", "tankercar" };
        
        private readonly List<Vector3> _metroStops = new()
        {
            new Vector3(-537.81665039062f, -1265.0319824219f, 25.905330657959f),
            new Vector3(-1088.627f, -2709.362f, -7.137033f),
            new Vector3(1081.309f, 2725.259f, 7.137033f),
            new Vector3(-889.2755f, -2311.825f, -11.45941f),
            new Vector3(-876.7512f, -2323.808f, -11.45609f),
            new Vector3(-545.3138f, -1280.548f, 27.09238f),
            new Vector3(-536.8082f, -1286.096f, 27.08238f),
            new Vector3(270.2029f, -1210.818f, 39.25398f),
            new Vector3(265.3616f, -1198.051f, 39.23406f),
            new Vector3(-286.3837f, -318.877f, 10.33625f),
            new Vector3(-302.6719f, -322.995f, 10.33629f),
            new Vector3(-826.3845f, -134.7151f, 20.22362f),
            new Vector3(-816.7159f, -147.4567f, 20.2231f),
            new Vector3(-1351.282f, -481.2916f, 15.318f),
            new Vector3(-1341.085f, -467.674f, 15.31838f),
            new Vector3(-496.0209f, -681.0325f, 12.08264f),
            new Vector3(-495.8456f, -665.4668f, 12.08244f),
            new Vector3(-218.2868f, -1031.54f, 30.51112f),
            new Vector3(-209.6845f, -1037.544f, 30.50939f),
            new Vector3(112.3714f, -1729.233f, 30.24097f),
            new Vector3(120.0308f, -1723.956f, 30.31433f)
        };

        #endregion
        
        public TrainsScript(ILogger<TrainsScript> logger, ITickManager ticks, IClientCommunicationsManager comms,
            ICommandManager commands, IPermissionService userAces, ISoundService soundService, ICommonFunctionsService common)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _commands = commands;
            _userAces = userAces;
            _soundService = soundService;
            _common = common;

            RegisterDecor(DecorHoldTrain, DecorType.Bool);
        }
        
        protected override async Task OnStartAsync()
        {
            API.SwitchTrainTrack(0, true);
            API.SwitchTrainTrack(3, true);
            API.N_0x21973bbf8d17edfa(0, 120000);
            API.N_0x21973bbf8d17edfa(3, 120000);
            API.SetRandomTrains(true);
            API.SetTrainsForceDoorsOpen(false);
            _ticks.On(RandomMetro);
        }

        private async Task RandomMetro()
        {
            API.SetDisableRandomTrainsThisFrame(false);
            var vehicles = World.GetAllVehicles();
            var trainHashes = _trains.Select(API.GetHashKey).ToArray();
            
            foreach (var vehicle in vehicles)
            {
                if (!trainHashes.Contains(vehicle.Model.Hash)) continue;
                
                if(!API.NetworkHasControlOfNetworkId(vehicle.NetworkId)) continue;
                
                var driver = vehicle.Driver;

                if (driver == null) continue;

                if (API.DecorExistOn(vehicle.Handle, DecorHoldTrain))
                {
                    var shouldHold = API.DecorGetBool(vehicle.Handle, DecorHoldTrain);

                    if (shouldHold)
                    {
                        vehicle.Speed = 0f;
                        vehicle.IsHandbrakeForcedOn = true;
                        DoorToggle(vehicle, 1);
                    }
                    else
                    {
                        vehicle.IsHandbrakeForcedOn = false;
                        DoorToggle(vehicle, 0);
                        if (vehicle.Speed < 25)
                        {
                            vehicle.Speed++;
                        }
                    }
                }
                else
                {
                    API.DecorSetBool(vehicle.Handle, DecorHoldTrain, false);
                }

                var trainPosition = API.GetOffsetFromEntityInWorldCoords(vehicle.Handle, 0f, 15f, 0f);
                API.DrawMarker(20, trainPosition.X, trainPosition.Y, trainPosition.Z, 0f,0f,0f,0f,0f,0f, 1f, 1f, 1f, 255, 255, 255, 100, false, false, 2, false, null, null, true);
                
                foreach (var _ in _metroStops.Select(stop => API.GetDistanceBetweenCoords(stop.X, stop.Y, stop.Z,
                    trainPosition.X, trainPosition.Y, trainPosition.Z, true)).Where(distance => distance <= 2.5f))
                {
                    API.DecorSetBool(vehicle.Handle, DecorHoldTrain, true);
                    await Delay(TimeSpan.FromSeconds(SecondsToHoldTrain)); // time at station
                    API.DecorSetBool(vehicle.Handle, DecorHoldTrain, false);
                    await Delay(TimeSpan.FromSeconds(SecondsToAllowEscape)); // time to get away
                }

                await Delay(0);
            }

            await Delay(10);
        }

        private static void DoorToggle(PoolObject train, int ratio)
        {
            var doors = API.GetTrainDoorCount(train.Handle);
            for (var i = 0; i < doors-1; i++)
            {
                API.SetTrainDoorOpenRatio(train.Handle, i, ratio);
            }
            
            var carriage = API.GetTrainCarriage(train.Handle, 1);
            doors = API.GetTrainDoorCount(carriage);
            for (var i = 0; i < doors-1; i++)
            {
                API.SetTrainDoorOpenRatio(carriage, i, ratio);
            }

            for (var i = 0; i < 6; i++)
            {
                if (ratio == 0)
                {
                    API.SetVehicleDoorShut(train.Handle, i, true);
                    API.SetVehicleDoorShut(carriage, i, true);
                    API.SetVehicleDoorsLocked(train.Handle, 6);
                    API.SetVehicleDoorsLocked(carriage, 6);
                }
                else
                {
                    API.SetVehicleDoorOpen(train.Handle, i, false, true);
                    API.SetVehicleDoorOpen(carriage, i, false, true);
                    API.SetVehicleDoorsLocked(train.Handle, 1);
                    API.SetVehicleDoorsLocked(carriage, 1);
                }
            }
        }
    }
}