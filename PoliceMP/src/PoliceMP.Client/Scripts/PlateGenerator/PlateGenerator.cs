using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants.Decors;
using System;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using CitizenFX.Core.NaturalMotion;
using System.ComponentModel;

namespace PoliceMP.Client.Scripts.PlateGenerator
{
    public class PlateGenerator : Script
    {
        private readonly ITickManager _ticks;
        private readonly INotificationService _notifications;
        private readonly ILogger<PlateGenerator> _logger;
        private ICommandManager _commandManager;

        public PlateGenerator(ITickManager ticks, INotificationService notifications, ILogger<PlateGenerator> logger, ICommandManager commandManager)
        {
            _ticks = ticks;
            _notifications = notifications;
            _logger = logger;
            _commandManager = commandManager;

            API.DecorRegister(VehicleDecors.HasBritishPlate, (int)DecorType.Bool);

            _commandManager.Register("checkplate").WithHandler(PlateCheck);
            
        }

        private async Task PlateCheck()
        {
            var CurrentCar = Game.PlayerPed.LastVehicle.Handle;
            var platetext = API.GetVehicleNumberPlateText(CurrentCar);
            _notifications.Success("Police Garage Log", $"Your Booked Out Car Is:{platetext}");
        }

        protected override Task OnStartAsync()
        {
            _ticks.On(PlateGeneratorTick);
            return Task.FromResult(0);
        }

        private async Task PlateGeneratorTick()
        {
            var vehicles = World.GetAllVehicles();
            foreach (var vehicle in vehicles)
            {
                if (!Entity.Exists(vehicle)) continue;
                if (vehicle.HasBritishPlate()) continue;

                vehicle.SetPlateText(GenerateBritishPlate());
                vehicle.SetBoolDecor(VehicleDecors.HasBritishPlate, true);

                await Delay(10);
            }

            await Delay(TimeSpan.FromSeconds(3));
        }

        private string GenerateBritishPlate()
        {
            int[] yearStart = { 0, 1, 5, 6 };

            var builder = new StringBuilder();
            builder.Append(GetLetter());
            builder.Append(GetLetter());

            var yearOne = yearStart[AppRandom.Next(yearStart.Length - 1)];
            var yearTwo = AppRandom.Next(0, 9);
            
            while (yearOne == 0 && yearTwo == 0)
            {
                // Prevent generating plates 00 = Seen by Unsociable
                yearOne = yearStart[AppRandom.Next(yearStart.Length - 1)];
                yearTwo = AppRandom.Next(0, 9);
            }
            
            builder.Append($"{yearOne}{yearTwo}");
            builder.Append("_");
            builder.Append(GetLetter());
            builder.Append(GetLetter());
            builder.Append(GetLetter());
            return builder.ToString();
        }

        private string GetLetter()
        {
            int num = AppRandom.Next(0, 26);
            char letter = (char)('a' + num);
            return letter.ToString().ToUpper();
        }
    }
}