using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants.Decors;

namespace PoliceMP.Client.Scripts.CalmVehicles
{
    public interface ICalmVehicles
    {
        void On(Vehicle vehicle);
        void Off(Vehicle vehicle);
    }

    public class CalmVehiclesScript : Script, ICalmVehicles
    {
        private readonly ILogger<CalmVehiclesScript> _logger;
        private readonly ITickManager _ticks;

        public CalmVehiclesScript(ILogger<CalmVehiclesScript> logger, ITickManager ticks)
        {
            _logger = logger;
            _ticks = ticks;

            API.DecorRegister(VehicleDecors.CalmVehicleDisabled, (int) DecorType.Bool);
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(CalmVehicleTick);

            return Task.FromResult(0);
        }

        private async Task CalmVehicleTick()
        {
            API.SetAggressiveHorns(false);

            var vehicles = World.GetAllVehicles();
            for (int i = 0; i < vehicles.Length; i++)
            {
                var vehicle = vehicles[i];
                if (vehicle == null
                    || !vehicle.Exists()
                    || !vehicle.HasDriver()
                    || !API.NetworkHasControlOfEntity(vehicle.Handle)
                    || API.IsEntityAMissionEntity(vehicle.Handle)
                    || vehicle.GetBoolDecor(VehicleDecors.CalmVehicleDisabled)
                    || vehicle.Driver?.GetBoolDecor(VehicleDecors.CalmVehicleDisabled) == true)
                {
                    await Delay(1);
                    continue;
                }

                if (vehicle.Driver is null)
                {
                    continue;
                }

                API.SetPlayerAngry(vehicle.Driver.Handle, false);
                API.SetDriverAggressiveness(vehicle.Driver.Handle, 0f);
                vehicle.Driver.SetConfigFlag((int)PedConfigFlags.CanBeAgitated, false);
                await Delay(1);
            }
        }

        public void Off(Vehicle vehicle)
        {
            vehicle.SetBoolDecor(VehicleDecors.CalmVehicleDisabled, true);
        }

        public void On(Vehicle vehicle)
        {
            vehicle.SetBoolDecor(VehicleDecors.CalmVehicleDisabled, false);
        }
    }
}
