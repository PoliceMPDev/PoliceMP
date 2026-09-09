using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Garage.Server
{
    public class Main : BaseScript
    {
        private static GarageConfig config;

        private static long gTimer;

        private static Dictionary<string, Guid> Reservations = new Dictionary<string, Guid>();
        private static Dictionary<string, int> PlayerVehicles = new Dictionary<string, int>();

        private static List<Guid> ReservedGarage = new List<Guid>();
        private static List<Guid> ReservedOutside = new List<Guid>();

        public Main()
        {
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            EventHandlers["CarGarage:Request"] += new Action<string, string, string>(OnRequest);
            EventHandlers["CarGarage:Release"] += new Action<string, string>(OnRelease);

            Debug.WriteLine("Before deserialize");
            // Use the file that the Client's resources refferences, so that the data is synced up. To change check Client Solution Resources folder.
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<GarageConfig>(File.ReadAllText("./resources/[PoliceResources]/PoliceMP.Garage/Resources/garage-config.json"));
            Debug.WriteLine(config.ToString());
            Tick += OnTick;
        }

        private Task OnTick()
        {
            // ~10 second loop timer
            if (API.GetGameTimer() - gTimer >= 10000)
            {
                gTimer = API.GetGameTimer();
                Loop();
            }
            return null;
        }

        private void Loop()
        {
            PlayerList pl = Players;

            int nPlayers = pl.Count();

            if (nPlayers == 0)
            {
                Reservations = new Dictionary<string, Guid>();
                ReservedGarage = new List<Guid>();
                ReservedOutside = new List<Guid>();
            }
            else
            {
                Dictionary<string, Guid> copyOfReservations = new Dictionary<string, Guid>(Reservations);
                foreach (KeyValuePair<string, Guid> entry in copyOfReservations)
                {
                    if (!pl.Contains(GetPlayerFromList(entry.Key)))
                    {
                        if (Reservations.Contains(new KeyValuePair<string, Guid>(entry.Key, entry.Value)))
                        {
                            Reservations.Remove(entry.Key);
                        }
                        if (ReservedGarage.Contains(entry.Value))
                        {
                            ReservedGarage.Remove(entry.Value);
                        }
                        if (ReservedOutside.Contains(entry.Value))
                        {
                            ReservedOutside.Remove(entry.Value);
                        }
                    }
                }
            }
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            try
            {
                if (Reservations.ContainsKey(player.Handle))
                {
                    OnRelease(player.Handle, Reservations[player.Handle].ToString());
                    Reservations.Remove(player.Handle);
                }
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine("Player disconnected, ensuring no reservation left over...");
            }
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            Reservations.Add(player.Handle, new Guid());
        }

        private static void OnRelease(string playerId, string guidStr)
        {
            Debug.WriteLine($"Spawn released.");
            Guid guid = new Guid(guidStr);
            try
            {
                if (Reservations[playerId] == guid)
                {
                    Reservations[playerId] = new Guid();
                }
            }
            catch (KeyNotFoundException)
            {
                Reservations.Add(playerId, new Guid());
            }
            if (ReservedGarage.Contains(guid))
            {
                ReservedGarage.Remove(guid);
            }
            if (ReservedOutside.Contains(guid))
            {
                ReservedOutside.Remove(guid);
            }
        }

        private Player GetPlayerFromList(string playerId)
        {
            PlayerList pl = Players;
            foreach (Player plyr in pl)
            {
                if (plyr.Handle == playerId)
                {
                    return plyr;
                }
            }
            return null;
        }

        private void OnRequest(string playerId, string type, string department = null)
        {
            Player player = GetPlayerFromList(playerId);
            if (department == null)
            {
                Debug.WriteLine($"{playerId} / {player.Name} requested a {type} spawn.");
            }
            else
            {
                Debug.WriteLine($"{playerId} / {player.Name} requested an {type} ({department}) spawn.");
            }

            if (type == "garage")
            {
                Randomize(config.GarageSpawns);
                foreach (GarageSpawn gs in config.GarageSpawns)
                {
                    if (!ReservedGarage.Contains(gs.Guid))
                    {
                        player.TriggerEvent("CarGarage:Assign", gs.Guid.ToString());
                        ReservedGarage.Add(gs.Guid);
                        Reservations[player.Handle] = gs.Guid;
                        return;
                    }
                }
            }
            if (type == "outside")
            {
                foreach (OutsideSpawn os in config.OutsideSpawns)
                {
                    if (!ReservedOutside.Contains(os.Guid))
                    {
                        player.TriggerEvent("CarGarage:Assign", os.Guid.ToString());
                        ReservedOutside.Add(os.Guid);
                        Reservations[player.Handle] = os.Guid;
                        return;
                    }
                }
            }
        }

        public void Randomize<T>(T[] items)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = PoliceMpRandom.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }

        public class Reservation
        {
            public Reservation() { }
            public Reservation(FullDestination position, Guid guid, Player player)
            {
                Guid = guid;
                Player = player;
            }
            public override string ToString() => Position.ToString() + "  " + Guid.ToString() + "   " + Player.Name;
            public FullDestination Position { get; set; }
            public Guid Guid { get; set; }
            public Player Player { get; set; }
        }
        public class GarageConfig
        {
            public GarageEntry[] GarageEntry { get; set; }
            public GarageSpawn[] GarageSpawns { get; set; }
            public OutsideSpawn[] OutsideSpawns { get; set; }
            /*public SpawnableVehicle[] Cars { get; set; }*/
        }
        public class FullDestination
        {
            public override string ToString() => X.ToString() + "  " + Y.ToString() + "   " + Z.ToString() + "   " + Heading;
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float Heading { get; set; }
        }
        public class GarageEntry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public FullDestination MarkerLocation { get; set; }
            public FullDestination OutsideLocation { get; set; }
        }
        public class GarageSpawn
        {
            public Guid Guid { get; set; }
            public FullDestination PlayerLocation { get; set; }
        }
        public class OutsideSpawn
        {
            public Guid Guid { get; set; }
            public FullDestination Location { get; set; }
        }
        public class SpawnableVehicle
        {
            public string Model { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
        }
    }
}
