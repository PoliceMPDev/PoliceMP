using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Shared.Models
{
    public class Ticket
    {
        public List<Offence> Offences { get; set; } = new List<Offence>();
        public int TotalPoints => Offences.Sum(offence => offence.Points);
        public int TotalFine => Offences.Sum(offence => offence.Fine);

        public bool HasAnyPointsOrFine => TotalPoints > 0 || TotalFine > 0;
    }
}