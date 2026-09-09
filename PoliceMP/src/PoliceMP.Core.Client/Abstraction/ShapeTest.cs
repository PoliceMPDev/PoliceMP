using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Abstraction
{
    [Flags]
    public enum ShapeTestFlags
    {
        IncludeVehicle = 2,
        IncludePed = 4,
        IncludeRagdoll = 8,
        IncludeObject = 16,
        IncludePickup = 32,
        IncludeGlass = 64,
        IncludeRiver = 128,
        IncludeFoliage = 256,
        IncludeMover = 1,
        IncludeAll = 511
    }
}
