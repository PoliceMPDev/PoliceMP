using System;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using PoliceMP.Callouts.Client.Models;
using PoliceMP.Callouts.Shared.Events;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Callouts.Client
{
    public class ClientCalloutManager : BaseScript
    {
        private ActiveCallout _activeCallout = null;
        private int _lastCalloutId = -1;

        public ClientCalloutManager()
        {
            API.DecorRegister("CalloutId", 3);
        }

        [Tick]
        private async Task OnTick()
        {
            if (_activeCallout == null) return;

            await Delay(1000);

            if (_activeCallout == null) return;

            // Check if the player has arrived
            if (!_activeCallout.Arrived &&
                LocalPlayer.Character.Position.DistanceToSquared(_activeCallout.Location) <= 3000f)
            {
                OnPlayerArrived();
            }

            if (!_activeCallout.WithinRange &&
                LocalPlayer.Character.Position.DistanceToSquared(_activeCallout.Location) <= 20000f)
            {
                OnPlayerWithinRange();
            }
        }

        private void OnPlayerArrived()
        {
            _activeCallout.Arrived = true;
            ServerEventAPI.ArrivedAtCallout(_activeCallout.ID);
            ShowNotification("You have arrived at your destination");
        }

        private void OnPlayerWithinRange()
        {
            _activeCallout.WithinRange = true;
            ServerEventAPI.WithinRangeOfCallout(_activeCallout.ID);
        }

        [EventHandler(ClientEvents.RECEIVE_CALLOUT_NOTIFICATION)]
        private void ReceiveCalloutNotification(int id, string title, string description, int grade, Vector3 location, bool isBackup)
        {
            var playerPos = Game.PlayerPed.Position;
            var street = ClientFunctions.GetStreetName(location);
            var distanceMetres = API.CalculateTravelDistanceBetweenPoints(location.X, location.Y, location.Z,
                playerPos.X, playerPos.Y, playerPos.Z);
            var distanceMiles = distanceMetres / 1609.344f;

            _lastCalloutId = id;

            if (isBackup)
            {
                ShowNotification($"<div style='display: flex; justify-content: center; align-items: center; padding: 5px; border-radius: 3px; background: rgba(255, 255, 255, 0.05); color: white;'>" +
                                 $"<div style='flex: 1; padding: 0px 5px; text-align: left;'>CAD #{id}</div>" + 
                                 $"<div style='flex-shrink: 1; border-radius: 3px; background: {(grade == 1 ? "rgba(255, 0, 0, 0.9)" : "rgba(2, 119, 189, 0.9)")}; color: white; text-align: center; font-family: Inconsolata, monospace; padding: 0px 5px; font-size: 1rem'>Grade {grade}</div></div>" + 
                                 $"<div style='margin: 5px 0px; border-radius: 3px; background: rgba(255, 255, 255, 0.05); color: white; line-height: 1.1; padding: 10px;'>" +
                                 $"{title} at {street}<br/><br/>" +
                                 $"{description}<br/><br/>" +
                                 $"You are {distanceMiles:0.00} miles away." +
                                 $"</div>" +
                                 $"<div style='padding: 0 5px; font-size: 0.9rem; margin-top: 10px;'>Use <code>/joincallout {id}</code> to attend</div>");
            }
            else
            {
                ShowNotification($"<span class='text-warning'>{title}</span> was reported at <span class='text-warning'>{street}</span>.<br>" +
                                 $"Grade: <span class='text-warning'>{grade}</span><br>" +
                                 $"Distance: <span class='text-warning'>{distanceMiles:0.00} miles</span><br>" +
                                 $"Use the <code>/joincallout {id}</code> command to join the callout.");
            }
        }

        [EventHandler(ClientEvents.JOIN_CALLOUT)]
        private void JoinCallout(int calloutId, string title, string description, Vector3 location, bool isBackup)
        {
            if (_activeCallout != null) return;

            _activeCallout = new ActiveCallout(calloutId, location, title, description);

            var street = ClientFunctions.GetStreetName(location);
            var word = title.ToUpper().StartsWith("A") ? "an" : "a";

            if(isBackup)
            {
                ShowNotification($"You have accepted the dispatch call for <span class='text-warning'>{title}</span> at <span class='text-warning'>{street}</span>." +
                 $" The destination is marked on your map. " +
                 $"Use the <code>/leavecallout</code> command when you are done.");

            } else
            {
                ShowNotification($"You have accepted the dispatch call to investigate {word} <span class='text-warning'>{title}</span> at <span class='text-warning'>{street}</span>." +
                 $" The destination is marked on your map. " +
                 $"Use the <code>/leavecallout</code> command when you are done.");
            }

        }

        [EventHandler(ClientEvents.LEAVE_CALLOUT)]
        private void LeaveCallout(int calloutId)
        {
            if (_activeCallout == null || _activeCallout.ID != calloutId) return;

            ShowNotification("You have left the callout.");

            _activeCallout = null;
        }

        [EventHandler(ClientEvents.CALLOUT_ENDED)]
        private void CalloutEnded(int calloutId)
        {
            // Hacky as fuck
            foreach (var vehicle in World.GetAllVehicles())
            {
                SetEntityAsNoLongerNeeded(calloutId, vehicle);
            }

            foreach (var ped in World.GetAllPeds())
            {
                SetEntityAsNoLongerNeeded(calloutId, ped);
            }

            foreach (var prop in World.GetAllProps())
            {
                SetEntityAsNoLongerNeeded(calloutId, prop);
            }

            if (_activeCallout == null || _activeCallout.ID != calloutId) return;

            ShowNotification("<span class='text-danger'>The callout has been abandoned.</span>");

            BlipHandler.RemoveAllBlips();

            _activeCallout = null;
        }

        private void SetEntityAsNoLongerNeeded(int calloutId, Entity entity)
        {
            if (!API.NetworkHasControlOfNetworkId(entity.NetworkId)) return;
            
            if (!API.DecorExistOn(entity.Handle, "CalloutId"))
            {
                Debug.WriteLine("Legacy Callouts: Tried to delete non callout entity.");
                return;
            }

            if (API.DecorGetInt(entity.Handle, "CalloutId") != calloutId)
            {
                Debug.WriteLine("Legacy Callouts: Tried to delete entity with different callout ID.");
                return;
            }

            if (API.IsEntityAVehicle(entity.Handle))
            {
                if (API.IsVehiclePreviouslyOwnedByPlayer(entity.Handle))
                {
                    Debug.WriteLine("Legacy Callouts: Tried to delete vehicle belonging to a player.");
                    return;
                }
            }
                
            Debug.WriteLine($"Marking entity {entity.Handle} as no longer needed for callout {calloutId}");
            entity.MarkAsNoLongerNeeded();
            entity.IsPersistent = false;
            API.SetNetworkIdAlwaysExistsForPlayer(entity.NetworkId, Game.Player.Handle, false);
            API.SetNetworkIdCanMigrate(entity.Handle, true);
        }

        [Command("joincallout")]
        private void Cmd_JoinCallout(string[] args)
        {
            if (args.Length != 1)
            {
                ClientFunctions.SendErrorMessage("USAGE: /joincallout [calloutId]");
                return;
            }

            if (!int.TryParse(args[0], out int calloutId))
            {
                ClientFunctions.SendErrorMessage("Parameter 'calloutId' must be a number.");
                return;
            }

            ServerEventAPI.RequestJoinCallout(calloutId);
        }
        
        [Command("joinrecentcallout")]
        private void Cmd_JoinRecentCallout()
        {
            if (_lastCalloutId != -1)
            {
                ServerEventAPI.RequestJoinCallout(_lastCalloutId);
            }
        }

        [Command("leavecallout")]
        private void Cmd_LeaveCallout()
        {
            ServerEventAPI.RequestLeaveCallout();
        }

        [Command("pos")]
        private void Cmd_Pos(string[] args)
        {
            if (LocalPlayer.Name != "mickmelon")
                return;

            if (args.Length != 1) return;

            ClientFunctions.SendChatMessage($"{LocalPlayer.Character.Position.ToString()}");
            ClientFunctions.SendChatMessage($"{LocalPlayer.Character.Rotation.ToString()}");

            ServerEventAPI.SavePosition(args[0], LocalPlayer.Character.Position, LocalPlayer.Character.Heading);

            LocalPlayer.Character.Task.ClearAllImmediately();
        }

        private void ShowNotification(string message)
            => ClientFunctions.ShowToast("Dispatch", message, "info");
    }
}
