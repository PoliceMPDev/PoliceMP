using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using MenuAPI;
using PoliceMP.Client.Scripts.PlayerControllerScript;
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

namespace PoliceMP.Client.Scripts.ServicePoints
{
    public class ServicePoints : Script
    {
        private readonly ILogger<ServicePoints> _logger;
        private readonly ITickManager _ticks;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IPlayerController _playercontroller;
        private readonly INotificationService _notificationService;
        
        private readonly List<uint> _fuelPumps = new List<uint>
        {
            1339433404,
            1694452750,
            1933174915,
            2287735495,
            3825272565,
            4130089803,
            3832150195,
            1767019582,
        };

        private bool _doingSomething = false;
        public ServicePoints(ILogger<ServicePoints> logger, ITickManager ticks, ILegacyClientCommunicationsManager comms, IPlayerController playercontroller, INotificationService notificationService)
        {
            _logger = logger;
            _ticks = ticks;
            _comms = comms;
            _playercontroller = playercontroller;
            _notificationService = notificationService;
        }

        protected override async Task OnStartAsync()
        {

            var otherFuelPumps = new List<string>
            {
                "petrol_pump"
            };

            foreach (var otherFuelPump in otherFuelPumps)
            {
                _fuelPumps.Add((uint)API.GetHashKey(otherFuelPump));
            }
            
            _ticks.On(FuelEntry);
        }

        private async Task FuelEntry()
        {
            if(_doingSomething) return;
            if(Game.PlayerPed.CurrentVehicle == null) return;
            if (Game.PlayerPed.CurrentVehicle.Driver == null) return;

            var currentVehicle = Game.PlayerPed.CurrentVehicle;

            var isDriver = currentVehicle.Driver == Game.PlayerPed;
            if (!isDriver) return;
            
            var vehiclePosition = currentVehicle.Position;

            var nearestObject = 0;

            foreach (var fuelPump in _fuelPumps)
            {
                if (nearestObject != 0) break;
                nearestObject = API.GetClosestObjectOfType(vehiclePosition.X, vehiclePosition.Y, vehiclePosition.Z, 5f,
                    fuelPump, false, true, true);
            }

            var isNearPump = nearestObject > 0;

            if (!isNearPump) return;

            if (currentVehicle.IsEngineRunning)
            {
                Screen.ShowSubtitle("Turn off your engine!");
                return;
            }
            
            Screen.ShowSubtitle($"Press E to fuel vehicle");

            if (API.IsControlJustReleased(0, (int) Control.VehicleHorn))
            {
                currentVehicle.FuelLevel = 100f;
            }
        }
    }
}