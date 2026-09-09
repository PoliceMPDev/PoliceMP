using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class RTC : Callout
    {
        public RTC(int id) : base(id)
        {
            Title = "RTC";
            Grade = 1;
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
        }

        public override void Setup()
        {
            var firstSpawn = Location +
                    new Vector3(0f, PoliceMpRandom.Next(-4, -3), 0f);
            var secondSpawn = Location +
                new Vector3(0f, PoliceMpRandom.Next(3, 4), 0f);

            for (var i = 0; i < 2; i++)
            {
                Vector3 spawnPos;
                if (i == 0) spawnPos = firstSpawn;
                else spawnPos = secondSpawn;

                var firstName = Util.Faker.Name.FirstName();
                var lastName = Util.Faker.Name.LastName();
                var isDead = PoliceMpRandom.Next(100) <= 30;

                AddEntity(new CalloutPed()
                {
                    Key = $"Ped{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                    HasBlip = true,
                    SpawnLocation = spawnPos + new Vector3(4f, 4f, 0f),
                    GenerateInfoManual = true,
                    FirstName = firstName,
                    LastName = lastName,
                    Flags = new Dictionary<string, bool>() { { "IsDead", isDead } }
                });

                AddEntity(new CalloutVehicle()
                {
                    Key = $"Vehicle{i}",
                    Hash = ServerFunctions.GetRandomVehicleHash(),
                    SpawnLocation = spawnPos,
                    HasBlip = true,
                    Colour = ServerFunctions.GetRandomVehicleColor(),
                    PetrolTankHealth = PoliceMpRandom.Next(50, 1000),
                    EngineHealth = PoliceMpRandom.Next(50, 1000),
                    BodyHealth = PoliceMpRandom.Next(50, 1000),
                    GenerateInfoManual = true,
                    Owner = $"{firstName} {lastName}",
                    Driver = isDead && PoliceMpRandom.Next(100) <= 50 ? $"Ped{i}" : string.Empty
                });
            }
        }

        protected override void OnEntitiesReady()
        {
            base.OnEntitiesReady();
            bool firstDone = false;
            var host = GetHost();
            foreach (var entity in _entities.Values.ToList())
            {
                if (entity is CalloutVehicle vehicle)
                {
                    ClientEventAPI.SetVehiclePhysicalDamage(host.Player, vehicle.NetworkId);
                    var rotation = 0f;
                    if (firstDone)
                    {
                        rotation = 180f;
                    }
                    else firstDone = true;

                    ClientEventAPI.SetVehicleRotation(host.Player, vehicle.NetworkId, rotation);
                }
                else if (entity is CalloutPed ped)
                {
                    if (ped.Flags.ContainsKey("IsDead") && ped.Flags["IsDead"])
                    {
                        ClientEventAPI.ApplyPedDamage(host.Player, ped.NetworkId, 2000);
                    }
                }
            }
        }
    }
}
