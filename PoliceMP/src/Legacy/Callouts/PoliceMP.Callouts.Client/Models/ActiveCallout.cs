using CitizenFX.Core;

namespace PoliceMP.Callouts.Client.Models
{
    public class ActiveCallout
    {
        public int ID { get; private set; }
        public Vector3 Location { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool Arrived { get; set; }
        public bool WithinRange { get; set; }
        public bool EntitiesSpawnedOnClient { get; set; }

        public ActiveCallout(int id, Vector3 location, string title, string description)
        {
            ID = id;
            Location = location;
            Title = title;
            Description = description;
            Arrived = false;
            WithinRange = false;
            EntitiesSpawnedOnClient = false;
        }
    }
}
