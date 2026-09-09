using System.Collections.Generic;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Shared.Models
{
    public class SpawnLocation
    {
        public string Name { get; set; }
        public PmpVector3 Position { get; set; }
        
        public UserBranch Branch { get; set; }
        
        public UserDivision? Division { get; set; }

        public SpawnLocation()
        {
        }

        public SpawnLocation(string name, PmpVector3 position, UserBranch branch, UserDivision? division = null)
        {
            Name = name;
            Position = position;
            Branch = branch;
            Division = division;
        }
    }
}