using PoliceMP.Main.Client.Retrievers;
using System.Collections.Generic;
using System.Linq;

namespace PoliceMP.Main.Client.Models
{
    public class Ticket
    {
        public const int MAX_POINTS = 12;
        public const int MAX_FINE = 5000;
        public const int MAX_OFFENCES = 5;

        public Ticket()
        {
            Offences = new List<string>();
        }

        public List<string> Offences { get; set; }

        public int GetPoints()
        {
            var points = 0;
            var allOffences = OffenceRetriever.GetAll();

            foreach (var offence in Offences)
            {
                var dbOffence = allOffences.FirstOrDefault(o => o.Name.Equals(offence));
                if (dbOffence == null) continue;

                points += int.Parse(dbOffence.Points);
            }

            return points > MAX_POINTS ? MAX_POINTS : points;
        }

        public int GetFine()
        {
            var fine = 0;
            var allOffences = OffenceRetriever.GetAll();

            foreach (var offence in Offences)
            {
                var dbOffence = allOffences.FirstOrDefault(o => o.Name.Equals(offence));
                if (dbOffence == null) continue;

                fine += int.Parse(dbOffence.Fine);
            }

            return fine > MAX_FINE ? MAX_FINE : fine;
        }
    }
}