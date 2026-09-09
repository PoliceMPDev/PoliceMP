using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Callouts.Server.Models;
using PoliceMP.Callouts.Server.Models.DTO;
using PoliceMP.Callouts.Server.Requests;
using PoliceMP.Callouts.Shared.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using PoliceMP.Callouts.Server.Enums;
using PoliceMP.Callouts.Server.Models.CalloutTypes;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Main.Core.Server.Extensions;
using PoliceMP.Main.Core.Shared;
using PoliceMP.Main.Core.Shared.Constants;
using PoliceMP.Shared.Models;
using Debug = CitizenFX.Core.Debug;
using PoliceMP.Core.Shared.Communications;

namespace PoliceMP.Callouts.Server
{
    public class CalloutManager : BaseScript
    {
        /// <summary>
        /// All the currently active callouts.
        /// </summary>
        private readonly List<Callout> _activeCallouts = new List<Callout>();

        /// <summary>
        /// The last assigned callout Id. Incremented every time
        /// a new callout is spawned.
        /// </summary>
        private int _lastCalloutId = 0;

        private Timer _timer;

        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            if (!IsPlayerOnAnyCallout(player))
                return;

            var callout = GetCalloutPlayerIsOn(player);
            callout.RemovePlayer(player);
        }

        [EventHandler(ServerEvents.REQUEST_CALLOUT_DATA)]
        private void OnRequestCalloutData([FromSource] Player player, string requestGuid)
        {
            // Create DTOs
            var calloutDtos = new List<CalloutDTO>();
            foreach (var callout in _activeCallouts)
            {
                var calloutDto = new CalloutDTO()
                {
                    Id = callout.Id,
                    Time = callout.TimeCreated,
                    Title = callout.Title,
                    LocationVector = callout.Location,
                    FirstPlayerArrived = callout.FirstPlayerArrived,
                    Grade = callout.Grade,
                    Rank = callout.Rank,
                    TimeOfArrival = callout.TimeOfArrival,
                    Players = new List<CalloutPlayerDTO>()
                };
                foreach (var calloutPlayer in callout.Players)
                {
                    var calloutPlayerDto = new CalloutPlayerDTO()
                    {
                        Name = calloutPlayer.Player.Name,
                        HasArrived = calloutPlayer.HasArrived,
                        TimeOfArrival = calloutPlayer.TimeOfArrival
                    };

                    calloutDto.Players.Add(calloutPlayerDto);
                }

                calloutDtos.Add(calloutDto);
            }

            player.TriggerEvent("PoliceMP:ReceiveRequestResult",
                requestGuid,
                JsonConvert.SerializeObject(calloutDtos));
        }

        [EventHandler("onResourceStart")]
        private void OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;
            Debug.WriteLine($"Loading {API.GetCurrentResourceName()}");
            _timer = new Timer();
            _timer.Elapsed += OnTimerElapsed;
            _timer.Interval = 60000;
            _timer.Enabled = true;
            _timer.Start();
            
        }
        
        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await CreateCallout();

            double interval = PoliceMpRandom.Next(30000, 300000);

            int playerCount = Players.Count();

            if (playerCount < 10)
            {
                // 5 - 10 minutes
                interval = PoliceMpRandom.Next(300000, 600000);
            }

            if (playerCount >= 10 && playerCount < 20)
            {
                // 4 - 8 minutes
                interval = PoliceMpRandom.Next(240000, 480000);
            }

            if (playerCount >= 20 && playerCount < 30)
            {
                // 4 - 6 minutes
                interval = PoliceMpRandom.Next(240000, 360000);
            }

            if (playerCount > 40)
            {
                // 2 - 5 minutes
                interval = PoliceMpRandom.Next(120000, 300000);
            }
            _timer.Interval = interval;
            _timer.Start();
        }

        /// <summary>
        /// Handles the event when a player requests to join a callout.
        /// </summary>
        /// <param name="player">The player requesting to join</param>
        /// <param name="calloutId">The Id of the callout they are requesting to join</param>
        [EventHandler(ServerEvents.REQUEST_JOIN_CALLOUT)]
        private void OnPlayerRequestJoinCallout([FromSource] Player player, int calloutId)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null)
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to join non-existant callout {calloutId}");
                ClientEventAPI.SendChatMessage(player, $"Callout {calloutId} doesn't exist.");
                return;
            }

            if (IsPlayerOnAnyCallout(player))
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to join callout {calloutId} but was already on a callout");
                ClientEventAPI.SendChatMessage(player, $"You are already on a callout.");
                return;
            }

            callout.AddPlayer(player);
            ClientEventAPI.JoinCallout(player, callout.Id, callout.Title, callout.Description, callout.Location, callout is BackupRequestCallout);
        }

        /// <summary>
        /// Handles the event when a player requests to leave a callout.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="calloutId"></param>
        [EventHandler(ServerEvents.REQUEST_LEAVE_CALLOUT)]
        private void OnPlayerRequestLeaveCallout([FromSource] Player player)
        {
            var callout = GetCalloutPlayerIsOn(player);
            if (callout == null)
            {
                ClientEventAPI.SendChatMessage(player, "You are not on any callout.", true);
                return;
            }

            callout.RemovePlayer(player);
            ClientEventAPI.SendChatMessage(player, "You have left the callout successfully.");
            ClientEventAPI.LeaveCallout(player, callout.Id);
        }

        /// <summary>
        /// Handles the event when a callout has ended
        /// </summary>
        /// <param name="calloutId">The id of the callout that ended</param>
        [EventHandler(ServerEvents.CALLOUT_ENDED)]
        private void OnCalloutEnded(int calloutId)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null) return;
            _activeCallouts.Remove(callout);

            foreach (var player in Players)
            {
                ClientEventAPI.CalloutEnded(player, calloutId);
            }
        }

        /// <summary>
        /// Handles the event when a client spawns a callout ped
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="calloutId">Id of the callout</param>
        /// <param name="pedId">Callout ped id</param>
        /// <param name="networkId">Ped network id</param>
        [EventHandler(ServerEvents.CLIENT_SPAWNED_PED)]
        private void OnClientSpawnedPed([FromSource] Player player, int calloutId, string pedKey, int networkId)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null)
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send CLIENT_SPAWNED_PED event for non-existant callout {calloutId}");
                return;
            }

            if (!callout.HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send CLIENT_SPAWNED_PED event for callout they are not on ({callout.Id})");
                return;
            }

            callout.SetEntityNetworkId(pedKey, networkId);

            Debug.WriteLine($"CALLOUTS: Player {player.Name} set ped {pedKey} networkId to {networkId} on callout {calloutId}");
        }

        /// <summary>
        /// Handles the event when a client spawns a callout vehicle
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="calloutId">Id of the callout</param>
        /// <param name="vehicleId">Callout vehicle id</param>
        /// <param name="networkId">Vehicle network id</param>
        [EventHandler(ServerEvents.CLIENT_SPAWNED_VEHICLE)]
        private void OnClientSpawnedVehicle([FromSource] Player player, int calloutId, string vehicleKey, int networkId, string plate)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null)
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send CLIENT_SPAWNED_VEHICLE event for non-existant callout {calloutId}");
                return;
            }

            if (!callout.HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send CLIENT_SPAWNED_VEHICLE event for callout they are not on ({callout.Id})");
                return;
            }

            callout.SetEntityNetworkId(vehicleKey, networkId, plate: plate);

            Debug.WriteLine($"CALLOUTS: Player {player.Name} set vehicle {vehicleKey} networkId to {networkId} on callout {calloutId}");
        }

        /// <summary>
        /// Handles the event when a player arrives at a callout
        /// </summary>
        /// <param name="player"></param>
        /// <param name="calloutId"></param>
        [EventHandler(ServerEvents.ARRIVED_AT_CALLOUT)]
        private void OnPlayerArrivedAtCallout([FromSource] Player player, int calloutId)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null)
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send ARRIVED_AT_CALLOUT event for non-existant callout {calloutId}");
                return;
            }

            if (!callout.HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send ARRIVED_AT_CALLOUT event for callout they are not on ({callout.Id})");
                return;
            }

            callout.OnPlayerArrived(player);
            Debug.WriteLine($"CALLOUTS: Player {player.Name} has arrived at callout {callout.Id}");
        }

        [EventHandler(ServerEvents.WITHIN_RANGE_OF_CALLOUT)]
        private void OnPlayerWithinRangeOfCallout([FromSource] Player player, int calloutId)
        {
            var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);
            if (callout == null)
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send WITHIN_RANGE_OF_CALLOUT event for non-existant callout {calloutId}");
                return;
            }

            if (!callout.HasPlayer(player))
            {
                Debug.WriteLine($"CALLOUTS: Player {player.Name} attempted to send WITHIN_RANGE_OF_CALLOUT event for callout they are not on ({callout.Id})");
                return;
            }

            callout.OnPlayerWithinRange(player);
            Debug.WriteLine($"CALLOUTS: Player {player.Name} is now within range of callout {callout.Id}");
        }

        [EventHandler(ServerEvents.SAVE_POSITION)]
        private async void OnPlayerSavePosition([FromSource] Player player, string name, Vector3 position, float heading)
        {
            Debug.WriteLine($"wrtiing to file");
            using (var writer = new StreamWriter("pos.txt", append: true))
            {
                await writer.WriteLineAsync($"new Vector3({position.X}f, {position.Y}f, {position.Z}f); // {name}");
            }
        }
        
        [EventHandler(ServerEvents.CALLOUT_BACKUP_REQUEST)]
        private void OnBackupRequest(string backupRequest)
        {
            if (backupRequest == null)
            {
                Debug.WriteLine("Backup request null or no cords. Cancelling...");
                return;
            }
            
            var _backupRequest = JsonConvert.DeserializeObject<BackupRequest>(backupRequest);
            
            Debug.WriteLine($"GotBackup, {JsonConvert.ToString(backupRequest)}");

            _lastCalloutId++;

            Callout callout = new BackupRequestCallout(_lastCalloutId, _backupRequest);
            
            callout.Setup();

            _activeCallouts.Add(callout);

            Debug.WriteLine(
                "Sending Backup Callout Notification: id: \"{0}\" title: \"{1}\" description: \"{2}\" grade: \"{3}\" location: \"{4}\"",
                callout.Id, callout.Title, callout.Description, callout.Grade, callout.Location);
            
            foreach (var p in Players)
            {
                ClientEventAPI.ReceiveCalloutNotification(p, callout.Id, callout.Title, callout.Description, callout.Grade, callout.Location, true);
            }
        }

        [Tick]
        private Task UpdateCallouts()
        {
            // Check the timer on the callout
            // Check callouts that haven't been accepted
            foreach (var callout in _activeCallouts.ToList())
            {
                callout.Update();
            }

            return Task.FromResult(0);
        }

        [Command("spawncallout")]
        private async void Cmd_StartRandomCallout(int source, List<object> args, string rawCommand)
        {
            // make this command password protected for now
            if (args.Count < 1) return;
            var pw = args[0].ToString();
            if (!pw.Equals("This_Is_A_Safe_Password")) return;

            await CreateCallout();
        }

        [Command("spawnspecificcallout")]
        private async void Cmd_StartSpecificCallout(int source, List<object> args, string rawCommand)
        {
            // make this command password protected for now
            if (args.Count < 1) return;
            var pw = args[0].ToString();
            if (!pw.Equals("This_Is_A_Safe_Password")) return;

            var calloutName = args[1].ToString();
            await CreateCallout(calloutName);
        }

        private int _createCalloutAttempts = 0;

        private async Task CreateCallout(string calloutName = "")
        {
            Debug.WriteLine($"Requesting Callout: {calloutName}");
            
            if (_createCalloutAttempts >= 3)
            {
                Debug.WriteLine("Too many failed attempts to create callout. Cancelling...");
                _createCalloutAttempts = 0;
                return;
            }
/*
            // dont create callout if no players on
            if (!Players.Any())
            {
                return;
            }
*/
            _lastCalloutId++;
            Callout callout;
            var attempts = 0;
            do
            {
                if (calloutName == "")
                {
                    callout = CalloutFactory.Random(_lastCalloutId);

                    if (callout == null) return;
                }
                else
                {
                    callout = CalloutFactory.GetSpecificCallout(calloutName, _lastCalloutId);
                }


                if (attempts >= 5)
                    return;
                attempts++;
                await Delay(1);
            }
            while (_activeCallouts.FirstOrDefault(c => c.Location.DistanceToSquared(callout.Location) <= 30000f) != null);

            Debug.WriteLine($"Callout ID: {callout.Id} Generated. Name: {callout.GetType().Name}");
            
            switch (callout.LocationSetting)
            {
                case LocationSetting.GetNextPositionOnStreet:
                {
                    var position = await ClientRequester.Request<Vector3>(GetRandomPlayer(),
                        ClientEvents.GET_NEXT_POSITION_ON_STREET, Locations.GetRandomCallout().ToCitizenVector3());
                    callout.Location = position;
                    break;
                }
                case LocationSetting.GetNextPositionOnSidewalk:
                {
                    var position = await ClientRequester.Request<Vector3>(GetRandomPlayer(),
                        ClientEvents.GET_NEXT_POSITION_ON_SIDEWALK, Locations.GetRandomCallout().ToCitizenVector3());
                    callout.Location = position;
                    break;
                }
            }

            if (_activeCallouts.FirstOrDefault(c => c.Location.DistanceToSquared(callout.Location) <= 30000f) != null)
            {
                Debug.WriteLine("Couldn't spawn callout with NextPositionOnStreet because already calout spawned there.. trying again");
                _createCalloutAttempts++;
                await CreateCallout();
                return;
            }

            callout.Setup();

            _activeCallouts.Add(callout);

            Debug.WriteLine(
                "Sending Callout Notification: id: \"{0}\" title: \"{1}\" description: \"{2}\" grade: \"{3}\" location: \"{4}\"",
                callout.Id, callout.Title, callout.Description, callout.Grade, callout.Location);
            foreach (var p in Players)
            {
                ClientEventAPI.ReceiveCalloutNotification(p, callout.Id, callout.Title, callout.Description, callout.Grade, callout.Location, false);
            }
        }

        private Player GetRandomPlayer()
        {
            var player = Players.OrderBy(p => Guid.NewGuid()).FirstOrDefault();
            return player;
        }

        [Command("testpos")]
        private async void Cmd_TestPos(int source, List<object> args, string rawCommand)
        {
            var player = Players.FirstOrDefault(p => p.Handle == source.ToString());
            var result = await ClientRequester.Request<Vector3>(player, ClientEvents.GET_NEXT_POSITION_ON_STREET, new Vector3(2062.25f, -881.75f, 78.15625f));
            Debug.WriteLine($"The result is {result.ToString()}");
        }

        [Command("activecallouts")]
        private void Cmd_ShowActiveCallouts(int source, List<object> args, string rawCommand)
        {
            var player = Players.FirstOrDefault(p => p.Handle == source.ToString());
            if (player == null)
            {
                foreach (var callout in _activeCallouts)
                {
                    Debug.WriteLine($"{callout.Id}: {callout.Title}");
                }

                return;
            }

            ClientEventAPI.SendChatMessage(player, $"[ACTIVE CALLOUTS]");
            foreach (var callout in _activeCallouts)
            {
                ClientEventAPI.SendChatMessage(player, $"{callout.Id}: {callout.Title}");
            }
        }

        [Command("endcallout", Restricted = true)]
        private void Cmd_EndCallout(int source, List<object> args, string rawCommand)
        {
            try
            {
                var player = Players.FirstOrDefault(p => p.Handle == source.ToString());

                if (int.TryParse(args[0] as string, out int calloutId))
                {
                    var callout = _activeCallouts.FirstOrDefault(c => c.Id == calloutId);

                    if (callout != null)
                    {
                        callout?.End(CalloutResult.NA);
                        Debug.WriteLine($"{player?.Name} requested end callout for {calloutId}");
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Check whether the specified player is on any callout.
        /// </summary>
        /// <param name="player">The player to check</param>
        /// <returns>Whether the player is on any callout</returns>
        private bool IsPlayerOnAnyCallout(Player player)
        {
            foreach (var callout in _activeCallouts)
                if (callout.HasPlayer(player))
                    return true;

            return false;
        }

        /// <summary>
        /// Gets the callout that the player is on.
        /// </summary>
        /// <param name="player">The player</param>
        /// <returns>The callout or null if none found</returns>
        private Callout GetCalloutPlayerIsOn(Player player)
        {
            foreach (var callout in _activeCallouts)
                if (callout.HasPlayer(player))
                    return callout;

            return null;
        }
    }
}
