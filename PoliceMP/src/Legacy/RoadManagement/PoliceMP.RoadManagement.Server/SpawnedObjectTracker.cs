using System;
using System.Collections.Generic;
using CitizenFX.Core;
using PoliceMP.RoadManagement.Shared;

namespace PoliceMP.RoadManagement.Server
{
    public class SpawnedObjectTracker : BaseScript
    {
        private List<SpawnedObject> _spawnedObjects = new List<SpawnedObject>();

        public SpawnedObjectTracker()
        {
            EventHandlers[ServerEvents.ADD_SPAWNED_OBJECT] += new Action<SpawnedObject>(Add);
            EventHandlers[ServerEvents.REMOVE_SPAWNED_OBJECT] += new Action<SpawnedObject>(Remove);
        }

        private void Get(int id)
        {

        }

        private void Add(SpawnedObject spawnedObject)
        {
            _spawnedObjects.Add(spawnedObject);
        }

        private void Remove(SpawnedObject spawnedObject)
        {
            _spawnedObjects.Remove(spawnedObject);
        }
    }
}
