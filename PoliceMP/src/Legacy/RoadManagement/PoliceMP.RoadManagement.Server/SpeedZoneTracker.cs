using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using PoliceMP.RoadManagement.Shared;

namespace PoliceMP.RoadManagement.Server
{
    public class SpeedZoneTracker : BaseScript
    {
        private List<int> _speedZones = new List<int>();

        public SpeedZoneTracker()
        { 
            EventHandlers[ServerEvents.SYNC_SPEED_ZONES] += new Action(Sync);
            EventHandlers[ServerEvents.ADD_SPEED_ZONE] += new Action<int>(Add);
            EventHandlers[ServerEvents.REMOVE_ALL_SPEED_ZONES] += new Action(RemoveAll);
            EventHandlers[ServerEvents.REMOVE_SPEED_ZONE] += new Action<int>(Remove);
        }

        private void RemoveAll()
        {
            _speedZones.Clear();
            Sync();
        }

        private void Sync()
        {
            foreach (var player in Players.ToList())
            {
                player.TriggerEvent(ClientEvents.UPDATE_SPEED_ZONES, _speedZones);
            }
        }

        private void Add(int speedZone)
        {
            _speedZones.Add(speedZone);
            Sync();
        }

        private void Remove(int speedZone)
        {
            _speedZones.Remove(speedZone);
            Sync();
        }
    }
}
