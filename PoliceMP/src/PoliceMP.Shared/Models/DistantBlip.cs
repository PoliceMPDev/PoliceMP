using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Shared.Models
{
    public class DistantBlip
    {
        public string PlayerServerHandle { get; set; }
        public int PlayerPedNetworkId { get; set; }
        public string BlipName { get; set; }
        public PmpVector3 Position { get; set; }
        public PmpBlipColor Color { get; set; }
        public float Scale { get; set; }
        public int Sprite { get; set; }
    }
}
