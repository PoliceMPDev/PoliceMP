using System.Collections.Generic;
using PoliceMP.Shared.Models;

namespace PoliceMP.Shared.Options
{
    public class OffenceOptions
    {
        public int MaxOffences { get; set; }
        public List<Offence> Offences { get; set; }
    }
}