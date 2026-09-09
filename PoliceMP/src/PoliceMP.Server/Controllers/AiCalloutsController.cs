using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Microsoft.Extensions.DependencyInjection;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Controllers.AiCallouts;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.NetworkMessages.Callouts.Notifications;
using PoliceMP.Shared.NetworkMessages.Callouts.Queries;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;

namespace PoliceMP.Server.Controllers
{
    /// <summary>
    /// Manages and keeps track of AI Callouts currently active in the server.
    /// </summary>
    public class AiCalloutsController : Controller
    {
        /// Represents a pseudo-random number generator.
        /// /
        private static readonly Random Random = new();

        /// Description:
        /// _callouts is a private Collection<IAiCallout> variable in the AiCalloutsController class. It stores all currently active callouts.
        /// Members:
        /// - private readonly Collection<IAiCallout> _callouts: The collection that stores all currently active callouts.
        /// Usage:
        /// The _callouts variable is used in the AiCalloutsController class to manage the active callouts. It is used in the SpawnCallout() method to add and remove callouts from the collection
        /// . It is also used in the CheckCallouts() method to iterate over the active callouts.
        /// Example:
        /// // Create a new callout
        /// IAiCallout callout = new AiCallout();
        /// // Add the callout to the collection
        /// _callouts.Add(callout);
        /// // Remove the callout from the collection
        /// _callouts.Remove(callout);
        /// /
        private readonly Collection<IAiCallout> _callouts = new();

        /// <summary>
        /// The list of clocked-on players in the game.
        /// </summary>
        private readonly List<Player> _clockedOnPlayers = new();

        /// Represents a legacy server communications manager.
        /// This class is part of the PoliceMP server application and is used for communicating
        /// messages between the server and clients.
        /// @deprecated This class is obsolete. Use the new server communications manager instead.
        /// @see ILegacyServerCommunicationsManager
        /// /
        private readonly IServerCommunicationsManager _comms;

        private readonly ILegacyServerCommunicationsManager _legacyComms;

        private readonly ILogger<AiCalloutsController> _logger;
        private readonly IFeatureService _featureService;
        private readonly IXPService _xpService;

        /// <summary>
        /// The controller responsible for managing and keeping track
        /// of AI Callouts currently active in the server.
        /// </summary>
        private readonly IPermissionService _perms;

        private readonly IServiceProvider _serviceProvider;

        /// Indicates whether AI callouts are currently being spawned.
        /// If true, AI callouts are being spawned; otherwise, they are not.
        /// /
        private bool _isSpawning;

        /// Represents the last AI callout that was sent to players.
        /// /
        private IAiCallout? _lastCallout;

        /// <summary>
        /// When a callout was last spawned
        /// </summary>
        private DateTime? _lastSpawned;

        /// <summary>
        /// When a new callout should be spawned
        /// </summary>
        private DateTime? _nextSpawn;

        /// Represents the unique identifier for the next AI callout.
        /// /
        private int _nextId = 1;

        /// <summary>
        /// Spawn timer, spawning a new callout picks a random number between these two values:
        /// - First number being minimum minutes before a new callout
        /// - Second number being maximum minutes before a new callout
        /// </summary>
        private Tuple<int, int> spawnTimer = new Tuple<int, int>(5, 10);

        /// <summary>
        /// Manages and keeps track of AI Callouts currently active in the server.
        /// </summary>
        public AiCalloutsController(
            IServerCommunicationsManager comms,
            ILegacyServerCommunicationsManager legacyComms,
            IPermissionService perms,
            IServiceProvider serviceProvider,
            ILogger<AiCalloutsController> logger,
            ICommandManager command,
            IFeatureService featureService,
            IXPService xpService
        )
        {
            _comms = comms;
            _legacyComms = legacyComms;
            _perms = perms;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _featureService = featureService;
            _xpService = xpService;

            if (_featureService.IsFeatureEnabled(FeatureToggle.AICallouts))
            {
                // Register commands
                command.Register("acceptcallout").HasGreedyArgs().WithHandler(OnPlayerAcceptCallout);
                command.Register("acceptrecentcallout").WithHandler(OnPlayerAcceptRecentCallout);
                command.Register("calloutinfo").WithHandler(OnCalloutInfo);
                command.Register("detachcallout").WithHandler(OnPlayerDetachCallout);
                command.Register("forcecallout").HasGreedyArgs().WithHandler(ForceCallout);
                command.Register("resetcallouts").WithHandler(ResetCallouts);
                command.Register("calloutclockon").WithHandler(CalloutClockOn);
                command.Register("calloutclockoff").WithHandler(CalloutClockOff);

                // Register event handlers
                _comms.AddRequestHandler<GetCalloutInfoByIdQuery, GetCalloutInfoResponse>(GetCalloutInfoById);
                _comms.AddNotificationHandler<PlayerSpawnedEvent>(OnPlayerSpawned);
                _comms.AddNotificationHandler<PlayerDroppedEvent>(OnPlayerDropped);
            }
        }

        /// <summary>
        /// Sends event to client to show current callout information and attached units
        /// </summary>
        /// <param name="player"></param>
        private void OnCalloutInfo(Player player)
        {
            try {
                if (!_clockedOnPlayers.Contains(player)) return;

                // Find the callout the player is attached to
                IAiCallout attachedCallout = null;
                foreach (IAiCallout callout in _callouts.ToList())
                {
                    if (callout.IsPlayerAttached(player))
                    {
                        // Found the attached callout
                        attachedCallout = callout;
                        break;
                    }
                }

                if (null == attachedCallout)
                {
                    // Player is not attached to a callout, send out a dummy response
                    _comms.PublishToClient(player, new CalloutInfoEvent()
                    {
                        CalloutId = null,
                        Title = null,
                        Subtitle = null,
                        Body = null,
                        AttachedUnits = new List<string>()
                    });
                    return;
                }

                // Build up list of attached units and their callsigns
                List<string> attachedUnits = new();
                foreach (var attendee in attachedCallout.GetAttendees())
                {
                    string attachedUnit = "";

                    var callsign = attendee.player.State.Get(PlayerStates.CallSign);
                    if (null != callsign)
                    {
                        attachedUnit += $"[{callsign}] ";
                    }

                    attachedUnit += attendee.player.Name;

                    if (!attachedUnits.Contains(attachedUnit))
                    {
                        attachedUnits.Add(attachedUnit);
                    }
                }

                // Fire the event back to the client
                _comms.PublishToClient(player, new CalloutInfoEvent()
                {
                    CalloutId = attachedCallout.Id,
                    Title = attachedCallout.Title(),
                    Subtitle = attachedCallout.Subtitle(),
                    Body = attachedCallout.Body(),
                    AttachedUnits = attachedUnits
                });
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in OnCalloutInfo: {ex.Message}");
            }
        }

        private Task OnPlayerDropped(PlayerDroppedEvent @event)
        {
            Player player = Players[@event.ServerHandle];
            CalloutClockOff(player);

            return Task.FromResult(0);
        }

        private Task OnPlayerSpawned(Player player, PlayerSpawnedEvent _)
        {
            _xpService.NotifyInitialXP(player);
            CalloutClockOff(player);

            return Task.FromResult(0);
        }

        public async Task StartFire(Vector3 coords, float size, SmartFireType type)
        {
            if (AiCalloutsConfig.CalloutsControllerDebugMode)
            {
                _logger.Debug(
                    $"starting fire with params coords: {coords}, size: {size}, type: {type.ToString().ToLower()}");
            }
            
            try
            {
                //force main thread, important
                await BaseScript.Delay(0);
                Exports["SmartFires"].CreateFire(coords, size, type.ToString().ToLower());
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"Failed to start fire with params coords: {coords}, size: {size}, type: {type.ToString().ToLower()}",
                    e);
            }
        }

        private async void ResetCallouts([FromSource] Player player)
        {
            var aces = await _perms.GetUserAces(player);
            if (!aces.IsZoflora) return;
            try
            {
                foreach (IAiCallout callout in _callouts.ToList())
                {
                    // Mark the callout as completed so its ready to be cleaned up
                    callout.OnComplete();
                    callout.SetStatus(AiCalloutStatus.Completed);

                    // Remove callout from the list since its no longer needed
                    _callouts.Remove(callout);
                    if (_lastCallout?.Id == callout.Id)
                    {
                        _lastCallout = null;
                    }
                }
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in ResetCallouts: {ex.Message}");
            }
        }

        protected override async Task ControllerTick()
        {
            if (!_featureService.IsFeatureEnabled(FeatureToggle.AICallouts))
            {
                return;
            }

            await CheckCallouts();

            if (null == _nextSpawn)
            {
                // Pick a random minutes value to setup next spawn timer
                int minutes = Random.Next(spawnTimer.Item1, spawnTimer.Item2);
                _nextSpawn = DateTime.UtcNow.AddMinutes(minutes);
            }

            // Check if we are ready to spawn a new callout
            if (false == _isSpawning && (null == _lastSpawned || DateTime.UtcNow > _nextSpawn))
            {
                await SpawnCallout();
                _lastSpawned = DateTime.UtcNow;

                // Pick a random minutes value to setup next spawn timer
                int minutes = Random.Next(spawnTimer.Item1, spawnTimer.Item2);
                _nextSpawn = DateTime.UtcNow.AddMinutes(minutes);
            }
        }

        private async Task<GetCalloutInfoResponse> GetCalloutInfoById(GetCalloutInfoByIdQuery arg)
        {
            var callout = _callouts.FirstOrDefault(c => c.Id == arg.CalloutId);
            if (callout is null)
            {
                return null;
            }

            return new GetCalloutInfoResponse
            {
                CalloutId = callout.Id,
                BlipLocation = callout.CalloutBlip().GetLocation().ToNetworkVector()
                //PlayerServerHandles = null
            };
        }

        private async Task ForceCallout([FromSource] Player player, string calloutNum)
        {
            var aces = await _perms.GetUserAces(player);
            if (!aces.IsZoflora) return;
            try {
                switch (calloutNum)
                {
                    case "1":
                        await SpawnCallout(
                            Type.GetType(
                                "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.AntiSocialCallout"));
                        break;

                    case "2":
                        await SpawnCallout(
                            Type.GetType(
                                "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.DomesticDisputeCallout"));
                        break;

                    case "3":
                        await SpawnCallout(
                            Type.GetType(
                                "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.FightInProgressCallout"));
                        break;

                    case "4":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.NHS.MentalHealthCrisisCallout"));
                        break;

                    case "5":
                        await SpawnCallout(
                            Type.GetType(
                                "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.MissingPersonCallout"));
                        break;

                    case "6":
                        await SpawnCallout(
                            Type.GetType("PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.RobberyCallout"));
                        break;

                    case "7":
                        await SpawnCallout(
                            Type.GetType(
                                "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.SingleDrugSelling"));
                        break;

                    case "8":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires.FourOneFiveMirrorParkFireCallout"));
                        break;

                    case "9":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.FailToStopCallout"));
                        break;

                    case "10":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.PublicOrderOffenceCallout"));
                        break;

                    case "11":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.MentalHealthWorldCallout"));
                        break;

                    case "12":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.AFOSingleCallout"));
                        break;

                    case "13":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires.FireServiceInspectionCallout"));
                        break;
                
                    case "14":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.ArmedFailToStopCallout"));
                        break;
                    
                    case "15":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.BtpFightInProgress"));
                        break;
                    
                    case "16":
                        await SpawnCallout(Type.GetType(
                            "PoliceMP.Server.Controllers.AiCallouts.Callouts.Police.BTPGunCallout"));
                        break;
                    
                    // case "9":
                    //     await SpawnCallout(Type.GetType(
                    //         "PoliceMP.Server.Controllers.AiCallouts.Callouts.Highways.BrokenDownVehicleCallout"));
                    //     break;
                    //
                    // case "10":
                    //     await SpawnCallout(Type.GetType(
                    //         "PoliceMP.Server.Controllers.AiCallouts.Callouts.JointResponse.DerailedTrainCallout"));
                    //     break;
                    //
                    // case "11":
                    //     await SpawnCallout(Type.GetType(
                    //         "PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires.VehicleFireCallout"));
                    //     break;
                    //
                    // case "12":
                    //     await SpawnCallout(Type.GetType(
                    //         "PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires.MultiVehicleRtcCallout"));
                    //     break;
                            
                    default:
                        _logger.Error("Choose a number between (1 - 16) inclusive to force a callout.");
                        break;
                }
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in ForceCallout: {ex.Message}");
            }
        }

        /// <summary>
        /// This method attempts to spawn a new random callout, depending on certain conditions.
        /// It is called every X minutes by Timer.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <param name="calloutClass">Pass in the callout to spawn, leave null to spawn a random callout</param>
        private async Task<bool> SpawnCallout(Type calloutClass = null)
        {
            _isSpawning = true;
            if (AiCalloutsConfig.CalloutsControllerDebugMode)
            {
                _logger.Debug($"Callout Type: {calloutClass}");
                _logger.Debug("Spawning a new callout...");
            }

            // Search for potential players who can accept a callout
            List<Player> playerList = new();
            foreach (Player player in _clockedOnPlayers.ToList())
            {
                UserRole role = await _perms.GetUserRole(player);
                if (role.Branch.Equals(UserBranch.Civ) || role.Branch.Equals(UserBranch.Control))
                {
                    // Exclude Civ and Control since they shouldn't be using AI callouts
                    continue;
                }

                playerList.Add(player);
            }

            if (!playerList.Any())
            {
                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"Not creating a callout since there are no clocked-on eligible players online.");
                }

                _isSpawning = false;
                return false;
            }

            IAiCallout callout = null;
            try
            {
                List<Type> possibleCallouts;
                if (null == calloutClass)
                {
                    // Find all the callout classes that aren't already started
                    possibleCallouts = GetPossibleCallouts();
                }
                else
                {
                    // Provided a callout to spawn
                    possibleCallouts = new List<Type>();
                    possibleCallouts.Add(calloutClass);
                }

                if (!possibleCallouts.Any() && AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"All possible callout classes are already started");
                    return false;
                }

                int attempts = 0;
                do
                {
                    try
                    {
                        // Pick one at random and initialize it
                        var nextIndex = Random.Next(possibleCallouts.Count);
                        callout = (IAiCallout)_serviceProvider.GetRequiredService(possibleCallouts[nextIndex]);
                        callout.Controller = this;
                    }
                    catch (Exception ex)
                    {
                        attempts++;

                        if (attempts > 10)
                        {
                            _logger.Error(ex.Message, ex);
                            return false;
                        }
                    }
                } while (null == callout);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }

            try
            {
                callout.Id = _nextId++;
                callout.OnCreate();
                callout.SetStatus(AiCalloutStatus.Created);

                // Start the 999 call
                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"NEW CALLOUT: {callout.Title()}");
                }

                int playersNotified = 0;
                foreach (Player player in playerList.ToList())
                {
                    UserRole role = await _perms.GetUserRole(player);
                    // Check if the player is eligible for this callout
                    if (!await IsEligibleForCallout(player, callout))
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Not notifying player {player.Name} due to not being the targeted division.");
                        }
                        continue;
                    }

                    // Check if the player is already on a callout
                    bool isPlayerAttached = false;
                    foreach (IAiCallout loopCallout in _callouts.ToList())
                    {
                        if (loopCallout.IsPlayerAttached(player))
                        {
                            isPlayerAttached = true;
                            break;
                        }
                    }

                    if (isPlayerAttached)
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Player {player.Name} is already attached to a call");
                        }

                        continue;
                    }

                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} sent new callout");
                    }

                    // Notify player since its a call they can potentially attend
                    // Client side handles the popup
                    _comms.PublishToClient(player, new CalloutCreatedEvent
                    {
                        CalloutId = callout.Id,
                        Title = callout.Title(),
                        Body = callout.Body(),
                        Subtitle = callout.Subtitle()
                    });
                    _lastCallout = callout;
                    playersNotified++;
                }

                if (0 == playersNotified)
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Callout cancelled due to no eligible players");
                    }

                    _isSpawning = false;
                    return false;
                }

                // Add to the callouts list
                _callouts.Add(callout);
            }
            catch (Exception ex)
            {
                // Something has gone wrong spawning the callout
                _logger.Error(ex.Message, ex);
                _callouts.Remove(callout);
                _isSpawning = false;
                return false;
            }

            _isSpawning = false;

            return true;
        }

        private List<Type> GetPossibleCallouts()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    typeof(IAiCallout).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Where(type =>
                    _callouts.Count(callout => callout.GetType() == type) ==
                    0) // Not the same type as one that's already existing
                .ToList();
        }

        private async Task<bool> IsEligibleForCallout(Player player, IAiCallout callout)
        {
            if (null == callout.TargetedDivisions()) return true;

            UserRole role = await _perms.GetUserRole(player);

            foreach (var targetedDivision in callout.TargetedDivisions())
            {
                if (
                    role.Division == targetedDivision.Division
                    && role.Branch == targetedDivision.Branch
                )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method checks all of our active callouts to see whether they need updating / closing.
        /// It is called every X minutes by Timer.
        /// </summary>
        /// <param name="source">The object that raised the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        private async Task CheckCallouts()
        {
            if (_isSpawning)
            {
                return;
            }

            // Loop through active callouts
            foreach (IAiCallout callout in _callouts.ToList())
            {
                try
                {
                    // Check whether the callout has expired
                    if (callout.HasExpired())
                    {
                        // Mark the callout as completed so its ready to be cleaned up
                        callout.OnComplete();
                        callout.SetStatus(AiCalloutStatus.Completed);

                        // Remove callout from the list since its no longer needed
                        _callouts.Remove(callout);
                        if (_lastCallout?.Id == callout.Id)
                        {
                            _lastCallout = null;
                        }

                        break;
                    }

                    // Check whether the status can be progressed
                    if (callout.GetStatus().Equals(AiCalloutStatus.Created) && callout.CanBeStarted())
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Starting callout {callout.Title()}...");
                        }

                        callout.OnStart();
                        callout.SetStatus(AiCalloutStatus.Started);
                    }
                    else if (callout.GetStatus().Equals(AiCalloutStatus.Started) && callout.CanBeResolved())
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Resolving callout {callout.Title()}...");
                        }

                        IAiCallout? newCallout = callout.OnResolve();
                        callout.SetStatus(AiCalloutStatus.Resolved);
                        callout.DetachAllAttendees();
                        _comms.PublishToClients(new CalloutResolvedEvent
                        {
                            CalloutId = callout.Id
                        });

                        if (null != newCallout)
                        {
                            _isSpawning = true;

                            // Spawn new callout and notify all players
                            newCallout.Id = _nextId++;
                            newCallout.OnCreate();
                            newCallout.SetStatus(AiCalloutStatus.Created);

                            // Start the 999 call
                            if (AiCalloutsConfig.CalloutsControllerDebugMode)
                            {
                                _logger.Debug($"NEW CALLOUT: {newCallout.Title()}");
                            }

                            List<Player> playerList = new();
                            foreach (Player player in _clockedOnPlayers.ToList())
                            {
                                UserRole role = await _perms.GetUserRole(player);
                                if (role.Branch.Equals(UserBranch.Civ) || role.Branch.Equals(UserBranch.Control))
                                {
                                    // Exclude Civ and Control since they shouldn't be using AI callouts
                                    continue;
                                }

                                playerList.Add(player);
                            }

                            int playersNotified = 0;
                            foreach (Player player in playerList.ToList())
                            {
                                UserRole role = await _perms.GetUserRole(player);
                                // Check if the player is eligible for this callout
                                if (!await IsEligibleForCallout(player, newCallout))
                                {
                                    _logger.Debug(
                                        $"Not notifying player {player.Name} due to not being the targeted division.");
                                    continue;
                                }

                                // Check if the player is already on a callout
                                bool isPlayerAttached = false;
                                foreach (IAiCallout loopCallout in _callouts.ToList())
                                {
                                    if (loopCallout.IsPlayerAttached(player))
                                    {
                                        isPlayerAttached = true;
                                        break;
                                    }
                                }

                                if (isPlayerAttached)
                                {
                                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                                    {
                                        _logger.Debug($"Player {player.Name} is already attached to a call");
                                    }

                                    continue;
                                }

                                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                                {
                                    _logger.Debug($"Player {player.Name} sent new callout");
                                }

                                // Notify player since its a call they can potentially attend
                                // Client side handles the popup
                                _comms.PublishToClient(player, new CalloutCreatedEvent
                                {
                                    CalloutId = newCallout.Id,
                                    Title = newCallout.Title(),
                                    Body = newCallout.Body(),
                                    Subtitle = newCallout.Subtitle()
                                });
                                _lastCallout = newCallout;
                                playersNotified++;
                            }

                            if (0 == playersNotified)
                            {
                                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                                {
                                    _logger.Debug($"Callout cancelled due to no eligible players");
                                }

                                _isSpawning = false;
                            }

                            // Add to the callouts list
                            _callouts.Add(newCallout);
                            _isSpawning = false;
                        }
                    }
                    else if (callout.GetStatus().Equals(AiCalloutStatus.Resolved) && callout.CanBeCompleted())
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Completing callout {callout.Title()}...");
                        }

                        callout.OnComplete();
                        callout.SetStatus(AiCalloutStatus.Completed);

                        // @todo Dashboard stats?

                        // Remove callout from the list since its no longer needed
                        _callouts.Remove(callout);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);

                    // Something has gone wrong with the callout, try and clear it and detach all units
                    try
                    {
                        callout.OnComplete();
                        callout.DetachAllAttendees();

                        // Notify all attached units that the callout went wrong
                        _comms.PublishToClients(new CalloutErrorNotificationEvent
                        {
                            CalloutId = callout.Id,
                            Title = callout.Title(),
                            Subtitle = callout.Subtitle(),
                            Body = callout.Body(),
                        });
                    }
                    catch (Exception innerEx)
                    {
                        // Log, keep calm and carry on
                        _logger.Error(innerEx.Message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Allows the player to attach to the most recent callout.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        private async void OnPlayerAcceptRecentCallout(Player player)
        {
            try {
                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"{player.Name} is trying to accept recent callout");
                }

                if (!_clockedOnPlayers.Contains(player))
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} is not clocked on.");
                    }

                    return;
                }

                UserRole role = await _perms.GetUserRole(player);

                if (role.Branch == UserBranch.Civ || role.Branch == UserBranch.Control)
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} cannot accept callouts as {role.Branch}.");
                        CalloutClockOff(player);
                    }

                    return;
                }

                // Check if the player is already on a callout
                if (IsPlayerAttachedToCallout(player)) return;

                // Find the most recent callout that targeted this player
                var possibleCallouts = _callouts.ToList();
                possibleCallouts.Reverse();

                if (possibleCallouts.Count == 0)
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} cannot accept recent callout as it doesn't exist.");
                        return;
                    }
                }

                IAiCallout? possibleCallout;
                do
                {
                    possibleCallout = possibleCallouts.First();
                    possibleCallouts.RemoveAt(0);
                    if (null == possibleCallout)
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Player {player.Name} cannot accept a non-existent callout.");
                        }

                        return;
                    }
                } while (
                    null != possibleCallout.TargetedDivisions()
                    && !possibleCallout.TargetedDivisions().Exists(userRole =>
                        role.Branch == userRole.Branch && role.Division == userRole.Division)
                );

                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"Player {player.Name} accepted callout {possibleCallout.Id}.");
                }

                try
                {
                    // Attach the unit to the callout
                    int index = _callouts.IndexOf(possibleCallout);
                    AttachPlayerToCallout(player, _callouts.ToList()[index]);
                }
                catch (Exception ex)
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug(
                            $"Player {player.Name} failed to attach to callout {possibleCallout.Id}: {ex.Message}.");
                    }

                    return;
                }
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in OnPlayerAcceptRecentCallout: {ex.Message}");
            }
        }

        private bool IsPlayerAttachedToCallout(Player player)
        {
            foreach (IAiCallout callout in _callouts)
            {
                if (callout.IsPlayerAttached(player))
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} is already attached to a callout.");
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Allows a player to attach to a callout by ID, regardless of their division.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="calloutId"></param>
        private async void OnPlayerAcceptCallout(Player player, string calloutId)
        {
            try
            {
                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"{player.Name} is trying to accept callout {calloutId}");
                }

                if (!_clockedOnPlayers.Contains(player))
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} is not clocked on.");
                    }

                    return;
                }

                UserRole role = await _perms.GetUserRole(player);

                if (role.Branch == UserBranch.Civ || role.Branch == UserBranch.Control)
                {
                    if (AiCalloutsConfig.CalloutsControllerDebugMode)
                    {
                        _logger.Debug($"Player {player.Name} cannot accept callouts as {role.Branch}.");
                        CalloutClockOff(player);
                    }

                    return;
                }

                // Check if the player is already on a callout
                foreach (IAiCallout callout in _callouts)
                {
                    if (callout.IsPlayerAttached(player))
                    {
                        if (AiCalloutsConfig.CalloutsControllerDebugMode)
                        {
                            _logger.Debug($"Player {player.Name} is already attached to a callout.");
                        }

                        return;
                    }
                }

                // Find callout with the given ID and attach the player
                foreach (IAiCallout callout in _callouts)
                {
                    if (callout.Id == int.Parse(calloutId))
                    {
                        // Found it!
                        AttachPlayerToCallout(player, callout);
                        return;
                    }
                }

                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"Callout {calloutId} does not exist.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred in OnPlayerAcceptCallout: {ex.Message}");
            }
        }

        private void OnPlayerDetachCallout(Player player)
        {
            try
            {
                if (!_clockedOnPlayers.Contains(player))
                {
                    return;
                }

                // Find which callout the player is attached to
                IAiCallout detachedCallout = null;
                foreach (IAiCallout callout in _callouts)
                {
                    if (callout.IsPlayerAttached(player))
                    {
                        callout.DetachPlayer(player);
                        detachedCallout = callout;
                        break;
                    }
                }

                if (null == detachedCallout)
                {
                    return;
                }

                // Tell all clients that the player detached from the callout
                _comms.PublishToClients(new PlayerDetachedFromCalloutNotification
                {
                    CalloutId = detachedCallout.Id,
                    CalloutAttendingUnits = detachedCallout.GetAttendees().Select(attendee =>
                {
                    var cs = attendee.player.State.Get(PlayerStates.CallSign);
                    return cs != null ? $"[{cs}] {attendee.player.Name}" : attendee.player.Name;
                }).ToList(),PlayerServerHandle = int.Parse(player.Handle),
                    PlayerName = player.Name,
                    PlayerCallsign = player.State.Get(PlayerStates.CallSign)
                });

                // Check if the callout has any attached players
                if (!detachedCallout.HasAttendees())
                {
                    // Mark the callout as resolved, ignoring any follow-up callout
                    detachedCallout.OnResolve();
                    detachedCallout.SetStatus(AiCalloutStatus.Resolved);
                }
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in OnPlayerDetachCallout: {ex.Message}");
            }
        }

        private void CalloutClockOn(Player player)
        {
            try
            {
                if (_clockedOnPlayers.Contains(player)) return;

                _clockedOnPlayers.Add(player);

                _comms.PublishToClient(player, new UserClockedOnNotificationEvent());
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in CalloutClockOn: {ex.Message}");
            }
        }

        private void CalloutClockOff(Player player)
        {
            try
            {
                if (!_clockedOnPlayers.Contains(player)) return;

                OnPlayerDetachCallout(player);
                _clockedOnPlayers.Remove(player);

                _comms.PublishToClient(player, new UserClockedOffNotificationEvent());
            } catch (Exception ex)
            {
                _logger.Error($"Exception occurred in CalloutClockOff: {ex.Message}");
            }
        }

        private async void AttachPlayerToCallout(Player player, IAiCallout callout, bool syncToCallsign = true)
        {
            if (callout.IsPlayerAttached(player)) return;

            UserRole role = await _perms.GetUserRole(player);

            try
            {
                callout.AddAttendee(player, role);
                _comms.PublishToClients(new PlayerAttachedToCalloutEvent
                {
                    CalloutId = callout.Id,
                    CalloutTitle = callout.Title(),
                    CalloutSubtitle = callout.Subtitle(),
                    CalloutBody = callout.Body(),
                    CalloutAttendingUnits = callout.GetAttendees().Select(attendee =>
                    {
                        var cs = attendee.player.State.Get(PlayerStates.CallSign);
                        return cs != null ? $"[{cs}] {attendee.player.Name}" : attendee.player.Name;
                    }).ToList(),
                    PlayerServerHandle = int.Parse(player.Handle),
                    PlayerName = player.Name,
                    PlayerCallsign = player.State.Get(PlayerStates.CallSign),
                    CalloutLocation = callout.CalloutBlip().GetLocation().ToNetworkVector(),
                    CalloutIcon = callout.CalloutBlip().GetIcon(),
                    CalloutBlipRadius = callout.CalloutBlip().GetRadius(),
                });
            }
            catch (Exception ex)
            {
                if (AiCalloutsConfig.CalloutsControllerDebugMode)
                {
                    _logger.Debug($"Player {player.Name} failed to attach to callout {callout.Id}: {ex.Message}.");
                }

                return;
            }

            if (syncToCallsign)
            {
                // Check for players that share a callsign and attach them too
                var callsign = player.State.Get(PlayerStates.CallSign);
                if (null != callsign)
                {
                    foreach (Player clockedOnPlayer in _clockedOnPlayers.ToList())
                    {
                        var clockedOnPlayerCallsign = clockedOnPlayer.State.Get(PlayerStates.CallSign);
                        if (clockedOnPlayer.Handle != player.Handle && clockedOnPlayerCallsign == callsign)
                        {
                            // My mate needs attaching
                            AttachPlayerToCallout(clockedOnPlayer, callout, false);
                        }
                    }
                }
            }
        }
    }
}