using CitizenFX.Core;
using CitizenFX.Core.UI;
using System;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Client;
using static CitizenFX.Core.Native.API;

// TODO
// - Get some different / extra messages for the people to say (we can ask the players for suggestions too)
// - Make peds spawned passive / not attack people even if they hit them w/ a car
// - Make player's mouth move when using radio?
// - Investigate pulling peds during Cororner drive over. I couldn't figure it out how w/ the required ref references used in funcs.
// - If you are far off a road, they will seem to endlessly drive around until you warp them
// - Sometimes if a ped is disturbed during their walk they will never move again, might need to check on a timer?

namespace PoliceMP.Summon.Client
{
    public class PoliceMPSummon : BaseScript
    {
        private static int gTimer;
        public static uint civHash;

        public PoliceMPSummon()
        {
            // EMS
            RegisterCommand("ems", new Action(SummonEMS), false);
            RegisterCommand("emswarp", new Action(EmergencyServices.Warp), false);

            // Fire
            RegisterCommand("fire", new Action(FireDepartment.Summon), false);
            RegisterCommand("firewarp", new Action(FireDepartment.Warp), false);

            // Taxi
            EventHandlers["PoliceMPSummon:Taxi:AssignTaxi"] += new Action<int>(Taxi.AssignPedToTaxi);
            RegisterCommand("taxiwarp", new Action(Taxi.Warp), false);

            // Tow Truck
            RegisterCommand("towtruck", new Action<int>(TowTruck.Summon), false);
            RegisterCommand("towtruckwarp", new Action(TowTruck.Warp), false);

            // Prisoner Transport
            EventHandlers["PoliceMPSummon:Transport:AssignTransport"] += new Action<int>(PrisonerTransport.AssignPedToTransport);
            RegisterCommand("transportwarp", new Action(PrisonerTransport.Warp), false);

            Tick += OnTick;
        }

        private Task OnTick()
        {
            // Handle warping hotkey
            if (IsControlJustPressed(1, 168))
            {
                if (EmergencyServices.eventSpawned)
                {
                    EmergencyServices.Warp();
                }
                if (FireDepartment.eventSpawned)
                {
                    FireDepartment.Warp();
                }
                if (Taxi.eventSpawned)
                {
                    Taxi.Warp();
                }
                if (TowTruck.eventSpawned)
                {
                    TowTruck.Warp();
                }
                if (PrisonerTransport.eventSpawned)
                {
                    PrisonerTransport.Warp();
                }
            }

            // ~1 second loop timer
            if (GetGameTimer() - gTimer >= 1000)
            {
                gTimer = GetGameTimer();
                EnableDispatchService(3, false); // Fire
                EnableDispatchService(5, false); // EMS

                Coroner.Loop();
                EmergencyServices.Loop();
                FireDepartment.Loop();
                Taxi.Loop();
                TowTruck.Loop();
                PrisonerTransport.Loop();
            }

            return Task.FromResult(0);
        }

        private void SummonCoroner()
        {
            if (!EmergencyServices.eventSpawned || (EmergencyServices.eventSpawned && EmergencyServices.eventSceneOver))
            {
                Coroner.Summon();
            }
            else
            {
                ShowNotification("You cannot call the coroner while EMS is on the way or on scene, please wait until they leave.");
            }
        }

        private void SummonEMS()
        {
            if (!Coroner.eventSpawned || (Coroner.eventSpawned && Coroner.eventSceneOver))
            {
                EmergencyServices.Summon();
            }
            else
            {
                ShowNotification("You cannot call EMS while the Coroner is on the way or on scene, please wait until they leave.");
            }
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Summon", message, "error");
    }
}
