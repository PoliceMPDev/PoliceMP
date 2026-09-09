using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;

namespace PoliceMP.PursuitManager.Server
{
    class Pursuit : BaseScript
    {
        private int pursuitID; //This should be the pos of this pursuit in the list of pursuits      
        private int suspectNetID;
        private Player originatingPlayer;
        private string vehicleMake;
        private string vehicleColour;
        private string location;

        //These are used if the suspect cannot be seen
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        private List<Player> pursingPlayers = new List<Player>();
        private List<Player> playersThatCanSeeSuspect = new List<Player>();


        public Pursuit(int _pursuitID, int _suspectNetID, Player _originatingPlayer, string _vehicleMake, string _vehicleColour, string _location, float _suspectX, float _suspectY, float _suspectZ)
        {
            pursuitID = _pursuitID;
            suspectNetID = _suspectNetID;
            originatingPlayer = _originatingPlayer;
            vehicleMake = _vehicleMake;
            vehicleColour = _vehicleColour;
            location = _location;

            lock (pursingPlayers)
            {
                pursingPlayers.Add(_originatingPlayer);
            }

            lock (playersThatCanSeeSuspect)
            {
                playersThatCanSeeSuspect.Add(_originatingPlayer);
            }
        }

        public int GetPursuitID() { return pursuitID; }
        public int GetSuspectNetID() { return suspectNetID; }

        public void SetLocation(string _location) { location = _location; }
        public string GetLocation() { return location; }

        public void SetXYZ(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool CanSuspectBeSeen()
        {
            lock (playersThatCanSeeSuspect)
            {
                if (playersThatCanSeeSuspect.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void AddPursuingPlayer(Player _pl)
        {
            lock (pursingPlayers)
            {
                if (pursingPlayers.Contains(_pl)) { return; }
                pursingPlayers.Add(_pl);
            }
        }

        public void RemovePursuingPlayer(Player _pl)
        {
            lock (pursingPlayers)
            {
                if (!pursingPlayers.Contains(_pl)) { return; }
                pursingPlayers.Remove(_pl);
            }
        }

        public void AddPlayerSight(Player _pl)
        {
            lock (playersThatCanSeeSuspect)
            {
                if (playersThatCanSeeSuspect.Contains(_pl)) { return; }
                playersThatCanSeeSuspect.Add(_pl);
            }
        }


        public void RemovePlayerSight(Player _pl)
        {
            lock (playersThatCanSeeSuspect)
            {
                if (!playersThatCanSeeSuspect.Contains(_pl)) { return; }
                playersThatCanSeeSuspect.Remove(_pl);
            }
        }


        public List<Player> GetAllPlayersInPursuit()
        {
            return pursingPlayers;
        }
    }
}
