using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace PoliceMP.Housing.Server
{
    class House
    {
        public int HouseID { get; set; }
        public bool Locked { get; set; }

        // List of player object and respective player handle
        public List<Tuple<Player, int>> PlayersInHouse { get; set; }
        public List<int> EntitiesInHouseNetIDs { get; set; }
    }
}
