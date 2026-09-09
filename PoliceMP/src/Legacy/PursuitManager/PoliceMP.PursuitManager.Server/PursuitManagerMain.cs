using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.PursuitManager.Server
{
    public class PursuitManagerMain : BaseScript
    {
        private int pursuitID = 0;
        private List<Pursuit> activePursuits = new List<Pursuit>();

        public void PursitManagerServer() { }


        [EventHandler("PursuitManager:RegisterPursuit")]
        private async void RegisterPursuit([FromSource] Player player, int suspectNetID, string vehicleMake, string vehicleColour, string location, float suspectX, float suspectY, float suspectZ)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetSuspectNetID() == suspectNetID)
                    {
                        string txt = "This pursuit already exists as ID: " + p.GetPursuitID();
                        player.TriggerEvent("PursuitManager:PursuitAlreadyExists", txt);
                        return;
                    }
                }
                
                string dispatchText =
                    $"<span class='text-primary'>{player.Name}</span> has a {vehicleColour} <span class='text-warning'>{vehicleMake}</span>" +
                    $" failing to stop for them at <span class='text-warning'>{location}</span><br>" +
                    $"<code>/joinpursuit {pursuitID}</code>";
                Pursuit pursuit = new Pursuit(pursuitID, suspectNetID, player, vehicleMake, vehicleColour, location, suspectX, suspectY, suspectZ);
                pursuit.AddPursuingPlayer(player);

                activePursuits.Add(pursuit);

                player.TriggerEvent("PursuitManager:PursuitHost", pursuitID, suspectNetID);

                TriggerClientEvent("PursuitManager:DisplayNotification", dispatchText);
                Debug.WriteLine(DateTime.Now + ": " + player.Name + " has registered a new pursuit for netID: " + suspectNetID + " this is pursuit " + pursuitID);
                pursuitID++;
            }
        }


        [EventHandler("PursuitManager:EndPursuit")]
        private async void EndPursuit([FromSource] Player _pl, int _pursuit)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetPursuitID() == _pursuit)
                    {
                        Debug.WriteLine(DateTime.Now + ": " + _pl.Name + " has registered pursuit conclusion for: " + _pursuit);
                        Pursuit toRemove = p;
                        foreach (Player pl in p.GetAllPlayersInPursuit())
                        {
                            pl.TriggerEvent("PursuitManager:PursuitEndedNotification");
                        }
                        activePursuits.Remove(toRemove);
                        return;
                    }
                }
            }
        }

        [EventHandler("PursuitManager:JoinPursuit")]
        private async void JoinPursuit([FromSource] Player _sourcePlayer, int _pursuit)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetPursuitID() == _pursuit)
                    {
                        p.AddPursuingPlayer(_sourcePlayer);
                        _sourcePlayer.TriggerEvent("PursuitManager:JoinedPursuit", _pursuit, p.GetSuspectNetID(), p.X, p.Y, p.Z, p.CanSuspectBeSeen());
                    }
                }
            }
        }

        [EventHandler("PursuitManager:LeavePursuit")]
        private async void LeavePursuit([FromSource] Player _sourcePlayer, int _pursuit)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetPursuitID() == _pursuit)
                    {
                        p.RemovePursuingPlayer(_sourcePlayer);
                    }
                }
            }
        }


        [EventHandler("PursuitManager:UpdateFromOrigin")]
        private async void updateFromOrigin(int _pursuit, string _location, int _speed, float _suspectX, float _suspectY, float _suspectZ)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetPursuitID() == _pursuit)
                    {
                        p.SetLocation(_location);
                        p.SetXYZ(_suspectX, _suspectY, _suspectZ);
                    }
                }
            }
        }


        [EventHandler("PursuitManager:UpdateFromPursuerSight")]
        private async void updatePursuerSeesSuspect([FromSource] Player _sourcePlayer, int _pursuit, bool _canSeeSuspect)
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    if (p.GetPursuitID() == _pursuit)
                    {
                        if (_canSeeSuspect)
                        {
                            p.AddPlayerSight(_sourcePlayer);
                        }
                        else
                        {
                            p.RemovePlayerSight(_sourcePlayer);
                        }
                    }
                }
            }
        }

        [Tick]
        private async Task PursuitUpdater()
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    foreach (Player pl in p.GetAllPlayersInPursuit())
                    {
                        pl.TriggerEvent("PursuitManager:RecievingUpdate", p.GetPursuitID(), p.GetSuspectNetID(), p.X, p.Y, p.Z, p.CanSuspectBeSeen());
                    }
                }
            }

            //Wait a while to prevent spam
            await Delay(5000);
        }

        [Tick]
        private async Task PursuitChecker()
        {
            lock (activePursuits)
            {
                foreach (Pursuit p in activePursuits)
                {
                    List<Player> pursuitPlayers = p.GetAllPlayersInPursuit();

                    if (pursuitPlayers.Count == 0)
                    {
                        Pursuit toRemove = p;
                        activePursuits.Remove(toRemove);
                        return;
                    }
                }
            }

            //Stop spam and checks too quickly
            await Delay(50000);
        }
    }
}
