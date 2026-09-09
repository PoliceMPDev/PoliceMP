using CitizenFX.Core;
using System.Linq;
using PoliceMP.Main.Core.Server;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;

namespace PoliceMP.Callouts.Server.Models.CalloutTypes
{
    public class StreetRace : Callout
    {
        private Vector3 _raceToPosition;

        public StreetRace(int id) : base(id)
        {
            Title = "Street Race";
            Grade = 1;
            LocationSetting = LocationSetting.GetNextPositionOnStreet;
            _raceToPosition = Locations.GetRandomCallout().ToCitizenVector3();
        }

        public override void Setup()
        {
            base.Setup();

            for (var i = 0; i < PoliceMpRandom.Next(2, 6); i++)
            {
                AddEntity(new CalloutPed()
                {
                    Key = $"Driver{i}",
                    Hash = (PedHash)(uint)PedModels.GetRandomHuman(),
                    HasBlip = true,
                    SpawnLocation = Location,
                });
                AddEntity(new CalloutVehicle()
                {
                    Key = $"Vehicle{i}",
                    Hash = ServerFunctions.GetRandomVehicleHash(),
                    SpawnLocation = Location + new Vector3(3f * i, 3f * i, 0f),
                    HasBlip = false,
                    Colour = ServerFunctions.GetRandomVehicleColor(),
                    Driver = $"Driver{i}",
                });
            }
        }

        protected override void OnEntitiesReady()
        {
            base.OnEntitiesReady();
            var entities = _entities.Values.ToList();
            foreach (var entity in entities)
            {
                if (entity is CalloutPed ped)
                {
                    var vehicle = entities.OfType<CalloutVehicle>().Where(v => v.Driver == entity.Key).FirstOrDefault();
                    ClientEventAPI.SetPedDriveToFast(GetHost().Player, ped.NetworkId, vehicle.NetworkId, _raceToPosition);
                }
            }
        }
    }
}
