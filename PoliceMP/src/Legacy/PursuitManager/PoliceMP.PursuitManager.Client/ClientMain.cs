using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.PursuitManager.Client
{
    public class ClientMain : BaseScript
    {
        private bool inPursuit = false;
        private bool pursuitHost = false;
        private int pursuitID;
        private int suspectNetID;
        public float suspectX, suspectY, suspectZ;
        private bool suspectVisible;

        private Blip SuspectBlip;
        private Blip PreciseBlip;

        public ClientMain() { }

        [EventHandler("PursuitManager:PursuitHost")]
        private void RecievingPursuit(int _pursuitID, int _suspectNetID)
        {
            pursuitID = _pursuitID;
            suspectNetID = _suspectNetID;

            int entityID = API.NetworkGetEntityFromNetworkId(suspectNetID);
            Ped suspect = (Ped)Entity.FromNetworkId(suspectNetID);
            if (suspect.CurrentVehicle.Driver == suspect)
            {
                API.SetVehicleHasBeenOwnedByPlayer(suspect.CurrentVehicle.Handle, true);
            }

            API.SetEntityAsMissionEntity(entityID, true, true);
            API.SetDriveTaskDrivingStyle(entityID, 524860);
            API.SetDriverAbility(entityID, 1f);
            API.SetDriverRacingModifier(entityID, 1f);
            API.SetDriverAggressiveness(entityID, 1f);

            API.NetworkRequestControlOfNetworkId(suspectNetID);
            API.SetNetworkIdExistsOnAllMachines(suspectNetID, true);
            API.SetNetworkIdCanMigrate(suspectNetID, false);

            foreach (Player p in Players)
            {
                if (API.NetworkIsPlayerActive(p.Handle))
                {
                    API.SetNetworkIdSyncToPlayer(suspectNetID, p.Handle, true);
                }
            }

            inPursuit = true;
            pursuitHost = true;
            suspectVisible = true;
        }

        [EventHandler("PursuitManager:DisplayNotification")]
        private void PutsuitStarted(string _dispatchText)
        {
            ShowNotification(_dispatchText);
        }

        [Command("joinpursuit")]
        private void JoinPursuit(string[] args)
        {
            if (inPursuit)
            {
                SendChatMessage("You are already in a pursuit!");
                return;
            }

            if (args.Length != 1) { return; }

            int _pursuit;
            try
            {
                _pursuit = System.Convert.ToInt32(args[0]);
                TriggerServerEvent("PursuitManager:JoinPursuit", _pursuit);
            }
            catch (Exception)
            {
                SendChatMessage("Inforrect Pursuit Command");
            }
        }

        [Command("leavepursuit")]
        private void LeavePursuit()
        {
            RemoveBlips();

            pursuitHost = false;
            inPursuit = false;

            if (!inPursuit)
            {
                SendChatMessage("You are not in an active pursuit!");
                return;
            }

            TriggerServerEvent("PursuitManager:LeavePursuit", pursuitID);
        }

        [Command("endpursuit")]
        private void EndPursuit()
        {
            if (!pursuitHost)
            {
                SendChatMessage("You are not the leader of an active pursuit!");
                return;
            }

            RemoveBlips();

            inPursuit = false;
            pursuitHost = false;

            TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
        }

        [EventHandler("PursuitManager:PursuitAlreadyExists")]
        private void PursuitExists(string _txt)
        {
            ShowNotification(_txt);
        }


        [EventHandler("PursuitManager:JoinedPursuit")]
        private void JoinedPursuit(int _pursuitID, int _suspectNetID, float _x, float _y, float _z, bool _suspectVisible)
        {
            ShowNotification("You are now part of the pursuit");
            inPursuit = true;
            pursuitID = _pursuitID;
            suspectNetID = _suspectNetID;
            suspectX = _x;
            suspectY = _y;
            suspectZ = _z;
            suspectVisible = _suspectVisible;
        }

        [EventHandler("PursuitManager:RecievingUpdate")]
        private void RecievingPursuit(int _pursuitID, int _suspectNetID, float _x, float _y, float _z, bool _suspectVisible)
        {
            pursuitID = _pursuitID;
            suspectNetID = _suspectNetID;
            suspectX = _x;
            suspectY = _y;
            suspectZ = _z;
            suspectVisible = _suspectVisible;
        }

        //THIS SHITE THREW AN ERROR ONE SUSPECT WAS GATTED
        [EventHandler("PursuitManager:PursuitEndedNotification")]
        private void PursuitEnded()
        {
            RemoveBlips();

            try
            {
                int entityID = API.NetworkGetEntityFromNetworkId(suspectNetID);
                API.SetEntityAsMissionEntity(entityID, false, false);

                Ped suspect = (Ped)Entity.FromNetworkId(suspectNetID);
                if (suspect.CurrentVehicle.Driver == suspect)
                {
                    API.SetVehicleHasBeenOwnedByPlayer(suspect.CurrentVehicle.Handle, false);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Issue with ending pursuit: " + ex);
            }

            ShowNotification("The suspect has been stopped, pursuit ended!");
            inPursuit = false;
            pursuitHost = false;
        }

        [Tick]
        private Task PursuitKeeper()
        {
            if (!pursuitHost) { return Task.FromResult(0); }

            if (!API.NetworkDoesEntityExistWithNetworkId(suspectNetID))
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                return Task.FromResult(0);
            }

            Ped suspect = (Ped)Entity.FromNetworkId(suspectNetID);

            if (suspect == null)
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                return Task.FromResult(0);
            }

            if (suspect.IsDead)
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                API.SetEntityAsNoLongerNeeded(ref suspectNetID);
                return Task.FromResult(0);
            }

            if (API.IsPedBeingStunned(suspect.Handle, 0))
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                return Task.FromResult(0);
            }

            if (API.IsPedUsingScenario(suspect.Handle, "WORLD_HUMAN_SUNBATHE"))
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                return Task.FromResult(0);
            }

            if (suspect.IsCuffed)
            {
                TriggerServerEvent("PursuitManager:EndPursuit", pursuitID);
                return Task.FromResult(0);
            }

            TriggerServerEvent("PursuitManager:UpdateFromOrigin", pursuitID, "", 0, suspect.Position.X, suspect.Position.Y, suspect.Position.Z);
            //TODO: Get street name for updates
            //TODO: Get Speed for updates
            //int _pursuit, string _location, int _speed, float _suspectX, float _suspectY, float _suspectZ         ^PARAMS FOR THAT SHITE
            return Task.FromResult(0);
        }

        [Tick]
        private async Task UpdatePursuit()
        {
            if (!inPursuit)
            {
                return;
            }
            else
            {
                //ShowNotification("Pursuit " + pursuitID + " continuing.");
                //Debug.WriteLine(pursuitID + "TICK UPDATE OF SUSPECT SIGHT: " + suspectVisible);
                if (suspectVisible)
                {
                    try
                    {
                        if (API.NetworkDoesEntityExistWithNetworkId(suspectNetID))
                        {
                            Ped suspect = (Ped)Entity.FromNetworkId(suspectNetID);
                            if (suspect == null)
                            {
                                BlipAreaCreate();
                            }
                            else
                            {
                                RemoveBlips();
                                PreciseBlip = suspect.AttachBlip();
                                PreciseBlip.Name = "Suspect";
                                PreciseBlip.Color = BlipColor.Red;
                                PreciseBlip.ShowRoute = true;
                                suspect.Task.FleeFrom(Game.PlayerPed);
                            }
                        }
                        else
                        {
                            BlipAreaCreate();
                        }
                    }
                    catch
                    {
                        BlipAreaCreate();
                    }
                }
                else
                {
                    BlipAreaCreate();
                }

                TriggerServerEvent("PursuitManager:UpdateFromPursuerSight", pursuitID, CanSeeSuspect());

                await Delay(5000);
            }
        }

        private void BlipAreaCreate()
        {
            RemoveBlips();

            var BlipID = API.AddBlipForRadius(suspectX, suspectY, suspectZ, 300f);
            SuspectBlip = new Blip(BlipID)
            {
                Color = BlipColor.Red,
                Name = "Suspect Last Seen",
                ShowRoute = true,
                Alpha = 128
            };
        }

        private bool CanSeeSuspect()
        {
            try
            {
                if (!API.NetworkDoesEntityExistWithNetworkId(suspectNetID)) { return false; }

                Ped suspect = (Ped)Entity.FromNetworkId(suspectNetID);
                Entity player = Entity.FromHandle(Game.PlayerPed.Handle);
                if (suspect == null) { return false; }
                if (player == null) { return false; }

                //suspect.IsNearEntity(player, new Vector3(100f, 100f, 100f)) || 
                if (API.HasEntityClearLosToEntity(player.Handle, suspect.Handle, 17))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void ShowNotification(string message)
            => ClientFunctions.ShowToast("Pursuit Manager", message, "info");

        public void SendChatMessage(
            string message,
            string title = "PoliceMP | Pursuit Manager",
            int red = 138,
            int green = 8,
            int blue = 8,
            bool multiline = true)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { red, green, blue },
                multiline,
                args = new[] { title, message }
            });
        }

        private void RemoveBlips()
        {
            if (PreciseBlip != null)
            {
                if (PreciseBlip.Exists())
                {
                    PreciseBlip.Delete();
                }
            }

            if (SuspectBlip != null)
            {
                if (SuspectBlip.Exists())
                {
                    SuspectBlip.Delete();
                }
            }
        }
    }
}
