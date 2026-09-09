using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.EntityFrameworkCore.Internal;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Models;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Models;
using Debug = System.Diagnostics.Debug;

namespace PoliceMP.Server.Controllers
{
    public class PlayerInfoController : Controller
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly PlayerList _playerList;
        private readonly ICommandManager _command;
        private readonly ILogger<PlayerInfoController> _logger;
        private readonly IBucketService _buckets;
        private readonly IPermissionService _permissionService;

        private readonly Timer _timer = new Timer(5000)
        {
            AutoReset = true,
            Enabled = true
        };

        private List<PlayerInfo> _cachedPlayerInfo = new List<PlayerInfo>();

        public PlayerInfoController(ILegacyServerCommunicationsManager comms, PlayerList playerList, ICommandManager command, ILogger<PlayerInfoController> logger, IBucketService buckets, IPermissionService permissionService)
        {
            _comms = comms;
            _playerList = playerList;
            _command = command;
            _logger = logger;
            _buckets = buckets;
            _permissionService = permissionService;

            _timer.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Updates the cached player list every 10 seconds to stop spamming it everytime it is requested. When requested they are sent the cache.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool sendToOdd = true;
        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (!_playerList.Any()) return;

                // Update the cached players list, only on the odd - this is so it only does once every 10 secs.
                if (sendToOdd)
                {
                    await UpdateCachedPlayers();
                }
                // Filter players based on the current flag (odd or even IDs)
                var filteredPlayers = _cachedPlayerInfo
                    .Where(player => sendToOdd ? player.ServerHandle % 2 != 0 : player.ServerHandle % 2 == 0)
                    .ToList();

                // Send the filtered players list to clients
                foreach (var player in filteredPlayers)
                {
                    // Skip if this player no longer exists in the current player list
                    bool stillExists = _playerList.Any(p =>
                        p != null &&
                        !string.IsNullOrEmpty(p.Handle) &&
                        int.TryParse(p.Handle, out var handleInt) &&
                        handleInt == player.ServerHandle);

                    if (!stillExists)
                        continue;

                    Player eventTarget = _playerList
                        .Where(p => p != null && !string.IsNullOrEmpty(p.Handle))
                        .FirstOrDefault(p =>
                        {
                            if (int.TryParse(p.Handle, out var handleInt))
                                return handleInt == player.ServerHandle;
                            return false;
                        });

                    if (eventTarget != null)
                    {
                        _comms.ToClient(eventTarget, ServerEvents.SendAllPlayersToClient, _cachedPlayerInfo);
                    }
                    else
                    {
                        _logger.Debug($"Could not find player with ServerHandle {player.ServerHandle}");
                    }
                }
                
                // Toggle the flag for the next timer elapse
                sendToOdd = !sendToOdd;
            }
            catch (Exception ex)
            {
                _logger.Error("Player info controller", ex);
            }
        }


        public async Task UpdateCachedPlayers()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var playerList = new ConcurrentBag<PlayerInfo>();

                var tasks = _playerList.Select(async targetPlayer =>
                {
                    
                    if (targetPlayer == null) return;
                    if (targetPlayer.Character == null) return;

                    try
                    {
                        var userRole = await _permissionService.GetUserRole(targetPlayer);
                        if (!int.TryParse(targetPlayer.Handle, out int targetHandle)) return;


                        var radioChannel = -1;
                        var radioChannelValue = targetPlayer.State["radioChannel"];  // Directly access the state

                        // Ensure radioChannelValue is not null and attempt to parse it
                        if (radioChannelValue != null)
                        {
                            if (radioChannelValue is string radioChannelStr)
                            {
                                // Parse if it's a string
                                Int32.TryParse(radioChannelStr, out radioChannel);
                            }
                            else if (radioChannelValue is int channelInt)
                            {
                                // Directly assign if it's already an int
                                radioChannel = channelInt;
                            }
                        }

                        var playerInfo = new PlayerInfo
                        {
                            Name = targetPlayer.Name,
                            Index = _playerList.IndexOf(targetPlayer),
                            RoutingBucket = _buckets.GetPlayerBucket(targetPlayer),
                            VehicleNetworkId = 0,
                            CallSign = (string)targetPlayer.State.Get(PlayerStates.CallSign),
                            RadioChannel = radioChannel,
                            ActiveBranch = userRole.Branch,
                            ActiveDivision = userRole.Division,
                            ServerHandle = targetHandle
                        };

                        var ped = targetPlayer.Character;
                        if (ped != null)
                        {
                            playerInfo.NetworkId = ped.NetworkId;
                            playerInfo.Position = new PmpVector3(ped.Position.X, ped.Position.Y, ped.Position.Z);
                            playerInfo.Rotation = new PmpVector3(ped.Rotation.X, ped.Rotation.Y, ped.Rotation.Z);
                        }

                        playerList.Add(playerInfo);
                    }
                    catch (Exception e)
                    {
                        _logger.Debug($"Player list issue with: {targetPlayer.Name} {e}");
                    }
                });

                await Task.WhenAll(tasks);

                lock (_cachedPlayerInfo)
                {
                    _cachedPlayerInfo = playerList.ToList();
                }
            }
            catch (Exception e)
            {
                _logger.Debug(e.ToString());
            }
            finally
            {
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 4000)
                {
                    _logger.Debug($"Warning: UpdateCachedPlayers took {stopwatch.ElapsedMilliseconds} ms to execute.");
                }
            }
        }


        public override Task Started()
        {
            _comms.OnRequest<int, PlayerInfo>(ServerEvents.RequestPlayerInfo, FetchPlayerInfoFromNetworkId);
            _comms.OnRequest(ServerEvents.FetchAllPlayersFromServer, FetchAllPlayersFromServer);
            _comms.On<string>(ServerEvents.OnReceiveCallSign, (player, callsign) =>
            {
                player.State.Set(PlayerStates.CallSign, callsign, true);
            });
            return Task.FromResult(0);
        }

        private async Task<List<PlayerInfo>> FetchAllPlayersFromServer(Player player)
        {
            return _cachedPlayerInfo;
        }

        private async Task<PlayerInfo> FetchPlayerInfoFromNetworkId(Player player, int networkId)
        {
            var targetPlayer = _playerList.FirstOrDefault(x => x.Character?.NetworkId == networkId);

            if (targetPlayer == null || targetPlayer.Character == null) return null;

            var ped = targetPlayer.Character;

            var userRole = await _permissionService.GetUserRole(player);
           
            var playerInfo = new PlayerInfo
            {
                Name = targetPlayer.Name,
                Index = _playerList.IndexOf(targetPlayer),
                RoutingBucket = _buckets.GetPlayerBucket(targetPlayer),
                NetworkId = ped.NetworkId,
                Position = new PmpVector3(ped.Position.X, ped.Position.Y, ped.Position.Z),
                Rotation = new PmpVector3(ped.Rotation.X, ped.Rotation.Y, ped.Rotation.Z),
                VehicleNetworkId = 0,
                CallSign = targetPlayer.State.Get(PlayerStates.CallSign),
                ActiveBranch = userRole.Branch,
                ActiveDivision = userRole.Division,
            };

            var tryParse = int.TryParse(targetPlayer.Handle, out int targetHandle);

            if (tryParse)
            {
                playerInfo.ServerHandle = targetHandle;
            }

            var pedVehicleId = API.GetVehiclePedIsIn(ped.Handle, false);

            if (pedVehicleId == 0) return playerInfo;

            var pedVehicle = (Vehicle)Entity.FromHandle(pedVehicleId);
            if (pedVehicle != null)
            {
                playerInfo.VehicleNetworkId = pedVehicle.NetworkId;
            }

            return playerInfo;
        }
    }
}