using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Computer.Client.Handlers;
using PoliceMP.Computer.Client.Models;
using PoliceMP.Computer.Client.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Computer.Client.Controllers
{
    public class CalloutController : Controller
    {
        /// <summary>
        /// Creates a new CalloutController.
        /// </summary>
        public CalloutController(ILogger logger, NuiHandler nui, TickHandler ticks, CommunicationsHandler comms, RpcHandler rpc) : base(logger, nui, ticks, comms, rpc)
        {
            _nui.On("leaveCallout", OnLeaveCalloutPressed);
            _nui.On("joinCallout", OnJoinCalloutPressed);
            _nui.On("requestCalloutData", OnRequestCalloutData);
            _nui.On("requestSingleCalloutData", OnRequestSingleCalloutData);
            _nui.On("requestUnitsData", OnRequestUnitsData);

            _comms.On("ComputerOpened", OnComputerOpened);
        }

        /// <summary>
        /// Called when a callout has ended.
        /// </summary>
        /// <param name="calloutId">Id of the callout that ended.</param>
        public void CalloutEnded(int calloutId)
        {
            Emit(new
            {
                cmd = "calloutEnded",
                calloutId
            });
        }
        /// <summary>
        /// Called when the player has joined a callout.
        /// </summary>
        /// <param name="calloutId">Id of the callout they joined.</param>
        public void JoinedCallout(int calloutId)
        {
            Emit(new
            {
                cmd = "joinedCallout",
                calloutId
            });
        }

        /// <summary>
        /// Gets a callout by the Id.
        /// </summary>
        /// <param name="id">Id of the callout to get.</param>
        /// <returns>The callout or null if none found.</returns>
        public async Task<CalloutData> GetCallout(int id)
        {
            var callouts = await GetCallouts();
            return callouts.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Gets all the callouts.
        /// </summary>
        /// <returns>All the callouts.</returns>
        public async Task<List<CalloutData>> GetCallouts()
        {
            var callouts = await _rpc.Request<List<CalloutData>>("Callouts:RequestCalloutData");

            foreach (var callout in callouts)
            {
                foreach (var player in callout.Players)
                {
                    if (player.Name == Game.Player.Name)
                        callout.LocalPlayerIsOn = true;
                }

                if (callout.Players.FirstOrDefault(p => p.Name == Game.Player.Name) != null)
                    callout.LocalPlayerIsOn = true;
                callout.Location = World.GetStreetName(callout.LocationVector);
                callout.LatLong = $"{callout.LocationVector.X}, {callout.LocationVector.Z}";
            }

            return callouts;
        }

        /// <summary>
        /// Gets all the ranks for all players.
        /// </summary>
        /// <returns>All players' ranks.</returns>
        public async Task<Dictionary<string, List<string>>> GetPlayersRanks()
        {
            var ranks = await _rpc.Request<Dictionary<string, List<string>>>("PoliceMPComputer:RequestAllPlayersRanks");
            return ranks;
        }

        /// <summary>
        /// Sends the latest callout data to the UI.
        /// </summary>
        public async Task UpdateCalloutData()
        {
            var callouts = await GetCallouts();
            var ranks = await GetPlayersRanks();
            var calloutPlayerIsOn = callouts.FirstOrDefault(c => c.Players.Any(p => p.Name == Game.Player.Name));

            // Add ranks
            foreach (var callout in callouts)
            {
                foreach (var player in callout.Players)
                {
                    if (player.Name == Game.Player.Name)
                        callout.LocalPlayerIsOn = true;

                    var playersRanks = ranks[player.Name];
                    // If they've got no ranks, set them to cadet.
                    if (playersRanks == null || playersRanks.Count() < 1)
                    {
                        player.Ranks = new string[] { "Cadet" };
                    }
                    // Otherwise set their ranks.
                    else
                    {
                        player.Ranks = playersRanks.ToArray();
                    }
                }
            }

            // Send to NUI
            Emit(new
            {
                cmd = "updateCallouts",
                callouts,
                calloutPlayerIsOn
            });
        }

        /// <summary>
        /// Called when the computer has been toggled open.
        /// </summary>
        private async void OnComputerOpened()
        {
            await UpdateCalloutData();
        }

        /// <summary>
        /// Called when the UI requests a single callout. It tries to find it via
        /// the passed ID and returns it if successful, otherwise returns error.
        /// </summary>
        private async void OnRequestSingleCalloutData(IDictionary<string, object> data, CallbackDelegate callback)
        {
            if (!int.TryParse(data["id"].ToString(), out int id))
            {
                _logger.Error("Could not parse ID in OnRequestSingleCalloutData");
                callback(JsonConvert.SerializeObject("error"));
                return;
            }

            var callout = await GetCallout(id);
            if (callout == null)
            {
                _logger.Error($"Could not get callout in OnRequestSingleCalloutData from Id: {id}");
                callback(JsonConvert.SerializeObject("error"));
                return;
            }

            // Add ranks to players on call
            var ranks = await GetPlayersRanks();
            foreach (var player in callout.Players)
            {
                var playersRanks = ranks[player.Name];
                if (playersRanks == null || playersRanks.Count < 1)
                    player.Ranks = new string[] { "Cadet" };
                else
                    player.Ranks = playersRanks.ToArray();
            }

            callback(JsonConvert.SerializeObject(callout));
        }

        /// <summary>
        /// Called when the UI requests units data. Returns the units data if successful,
        /// otherwise returns error.
        /// </summary>
        private async void OnRequestUnitsData(IDictionary<string, object> data, CallbackDelegate callback)
        {
            var units = new List<Unit>();
            var ranks = await GetPlayersRanks();
            var callouts = await GetCallouts();

            List<PlayerInfo> playerList = await _rpc.Request<List<PlayerInfo>>("PoliceMPComputer:RequestAllPlayerInfo");
            
            _logger.Log($"Found {playerList.Count} players");
            
            foreach (var player in playerList)
            {
                var unit = new Unit()
                {
                    Id = player.ServerId,
                    Name = player.Name,
                    Location = World.GetStreetName(new Vector3(player.PositionX, player.PositionY, player.PositionZ ))
                };

                var playersRanks = ranks[player.Name];
                // If they've got no ranks, set them to cadet.
                if (playersRanks == null || playersRanks.Count() < 1)
                {
                    unit.Ranks = new string[] { "Cadet" };
                }
                // Otherwise set their ranks.
                else
                {
                    unit.Ranks = playersRanks.ToArray();
                }

                // Status
                var callout = callouts.FirstOrDefault(c => c.Players.Any(p => p.Name == player.Name));
                if (callout == null)
                    unit.Status = "<code class='bg-gray fg-white'>On Patrol</code>";
                else
                    unit.Status = $"<code class='bg-cyan fg-white'>On Call {callout.Id}</code>";

                units.Add(unit);
            }

            callback(JsonConvert.SerializeObject(units.ToArray()));
        }

        /// <summary>
        /// Called when the UI requests all callouts data. Returns the data if successful,
        /// otherwise returns error.
        /// </summary>
        private async void OnRequestCalloutData(IDictionary<string, object> data, CallbackDelegate callback)
        {
            _logger.Log("OnRequestCalloutData");
            var callouts = await GetCallouts();
            callback(JsonConvert.SerializeObject(callouts));
        }

        /// <summary>
        /// Called when the "Join Callout" button has been pressed.
        /// </summary>
        private async void OnJoinCalloutPressed(IDictionary<string, object> data, CallbackDelegate callback)
        {
            if (!int.TryParse(data["id"].ToString(), out int id))
            {
                _logger.Error("Could not parse ID in OnJoinCalloutPressed");
                return;
            }

            BaseScript.TriggerServerEvent("Callouts:RequestJoinCallout", id);

            // Check if they were added or not.
            var callout = await GetCallout(id);
            if (callout == null)
            {
                callback(JsonConvert.SerializeObject("error"));
                return;
            }

            // They were accepted on the callout
            if (callout.Players.Any(p => p.Name == Game.Player.Name))
            {
                callback(JsonConvert.SerializeObject("success"));
                return;
            }

            callback(JsonConvert.SerializeObject("error"));
        }

        /// <summary>
        /// Called when the "Leave Callout" button has been pressed.
        /// </summary>
        private void OnLeaveCalloutPressed(IDictionary<string, object> data, CallbackDelegate callback)
        {
            if (!int.TryParse(data["id"].ToString(), out int id))
            {
                _logger.Error("Could not parse ID in OnLeaveCalloutPressed");
                return;
            }

            BaseScript.TriggerServerEvent("Callouts:RequestLeaveCallout");

            callback();
        }
    }
}
