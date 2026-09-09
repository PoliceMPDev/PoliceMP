using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Options;
using System;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Scripts.VehicleFaultGenerator
{
    public class VehicleFaultGenerator : Script
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ITickManager _ticks;
        private VehicleFaultGeneratorOptions _options;

        public VehicleFaultGenerator(ILegacyClientCommunicationsManager comms, ITickManager ticks)
        {
            _comms = comms;
            _ticks = ticks;

            API.DecorRegister(VehicleDecors.HasGeneratedFaults, (int)DecorType.Bool);
        }

        protected override async Task OnStartAsync()
        {
            _options = await _comms.Request<VehicleFaultGeneratorOptions>(ServerEvents.VehicleFaultGeneratorGetOptions);
            _ticks.On(VehicleFaultGeneratorTick);
        }

        private async Task VehicleFaultGeneratorTick()
        {
            var vehicles = World.GetAllVehicles();
            foreach (var vehicle in vehicles)
            {
                if (!Entity.Exists(vehicle)) continue;

                if (!ShouldGenerateFaultsForVehicle(vehicle))
                {
                    API.SetDriverAbility(vehicle.Driver.Handle, 1);
                    API.SetDriverAggressiveness(vehicle.Driver.Handle, 0);
                    continue;
                }

                vehicle.SetBoolDecor(VehicleDecors.HasGeneratedFaults, true);
                bool hasDriver = vehicle.Driver != null && vehicle.Driver.Exists();

                if (hasDriver && CheckCondition(_options.SpeedingChance))
                    API.TaskVehicleDriveWander(vehicle.Driver.Handle, vehicle.Handle, _options.SpeedingSpeed, _options.SpeedingDrivingStyle);

                if (hasDriver && CheckCondition(_options.RunRedLightChance))
                    API.SetDriveTaskDrivingStyle(vehicle.Driver.Handle, _options.RunRedLightDrivingStyle);

                if (CheckCondition(_options.HeadlightsOffChance))
                    vehicle.AreLightsOn = false;

                if (CheckCondition(_options.PoorBodyHealthChance))
                    vehicle.BodyHealth = AppRandom.Next(_options.PoorBodyMinHealth, _options.PoorBodyMaxHealth);

                if (CheckCondition(_options.PoorEngineHealthChance))
                    vehicle.EngineHealth = AppRandom.Next(_options.PoorEngineMinHealth, _options.PoorEngineMaxHealth);

                if (CheckCondition(_options.PoorPetrolTankHealthChance))
                    vehicle.PetrolTankHealth = AppRandom.Next(_options.PoorPetrolTankMinHealth, _options.PoorPetrolTankMaxHealth);

                if (CheckCondition(_options.BurstTyreChance))
                {
                    int quantity = AppRandom.Next(1, 4);
                    for (var i = 0; i < quantity; i++)
                        API.SetVehicleTyreBurst(vehicle.Handle, i, true, 1000f);
                }

                if (CheckCondition(_options.AlarmActiveChance))
                {
                    vehicle.IsAlarmSet = true;
                    vehicle.AlarmTimeLeft = _options.AlarmTimeLeft;
                    vehicle.Windows[VehicleWindowIndex.FrontLeftWindow].Smash();
                }

                if (hasDriver && CheckCondition(_options.DodgyDriverChance))
                    API.SetDriveTaskDrivingStyle(vehicle.Driver.Handle, _options.DodgyDriverDrivingStyle);

                await Delay(500);
            }

            await Delay(TimeSpan.FromSeconds(3));
        }

        private bool CheckCondition(double chance)
            => chance >= AppRandom.Next(0, 100);

        private bool ShouldGenerateFaultsForVehicle(Vehicle vehicle)
            => !vehicle.HasGeneratedFaults()
               && !vehicle.IsLegal()
               && vehicle.ClassType != VehicleClass.Helicopters
               && vehicle.ClassType != VehicleClass.Planes
               && vehicle.ClassType != VehicleClass.Military
               && CheckCondition(_options.IllegalStuffChance);
    }
}