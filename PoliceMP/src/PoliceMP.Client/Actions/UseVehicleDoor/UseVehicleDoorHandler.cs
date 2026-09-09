using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Actions.UseVehicleDoor
{
    public class UseVehicleDoorHandler : ActionHandler<UseVehicleDoor>
    {
        // This is used for the new boot AFO locker that opens on the bonnet
        private readonly List<string> _afoBoots = new List<string>
        {
            "addpolbmw5arv",
        };

        protected override async Task<bool> Handle(UseVehicleDoor action)
        {
            var timeout = Game.GameTime + 2000;
            if (action.Target.Doors[action.Door].IsOpen)
            {
                action.Target.Doors[action.Door].Close();

                if (action.Door is VehicleDoorIndex.Trunk && VehicleHasAfoBoot(action.Target))
                {
                    action.Target.Doors[VehicleDoorIndex.Hood].Close();
                }
            }
            else
            {
                action.Target.Doors[action.Door].Open();

                if (action.Door is VehicleDoorIndex.Hood && VehicleHasAfoBoot(action.Target))
                {
                    action.Target.Doors[VehicleDoorIndex.Trunk].Open();
                }

                while (!action.Target.Doors[VehicleDoorIndex.Trunk].IsFullyOpen && Game.GameTime < timeout)
                    await Script.Delay(1);

                action.Target.Doors[action.Door].Open(true);
            }

            return true;
        }

        private bool VehicleHasAfoBoot(Vehicle vehicle)
        {
            return _afoBoots.Any(afoBoot => API.GetHashKey(afoBoot) == vehicle.Model.Hash);
        }
    }
}