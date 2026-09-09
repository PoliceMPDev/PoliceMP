using System.Collections.Generic;

namespace PoliceMP.Core.Shared.Commands
{
    public class Command
    {
        public string Name { get; set; }
        public bool Restricted { get; set; }
        public bool HasGreedyArgs { get; set; }
    }
}
