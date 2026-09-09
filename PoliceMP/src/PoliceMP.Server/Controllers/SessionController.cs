using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.Extensions.Options;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Constants;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Shared.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Timers;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Server.Services;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using Debug = System.Diagnostics.Debug;

namespace PoliceMP.Server.Controllers
{
    public class SessionController : Controller
    {
        private readonly ILogger<SessionController> _logger;

        //private readonly IUserService _userService;
        //private readonly ISessionService _sessionService;
        private readonly PlayerList _players;

        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly ServerOptions _serverOptions;

        //private readonly IActivityService _activityService;
        private readonly IPermissionService _perms;

        private readonly IBucketService _buckets;
        private readonly IFeatureService _featureService;
        private Timer MinuteTimer = new Timer(60000);

        public SessionController(IFiveEventManager fiveEvents,
            ILogger<SessionController> logger,
            //IUserService userService,
            //ISessionService sessionService,
            PlayerList players,
            ILegacyServerCommunicationsManager comms,
            INotificationService notifications,
            IOptions<ServerOptions> serverOptions,
            //IActivityService activityService,
            IPermissionService perms,
            IBucketService buckets,
            IFeatureService featureService)
        {
            _logger = logger;
            //_userService = userService;
            //_sessionService = sessionService;
            _players = players;
            _comms = comms;
            _notifications = notifications;
            _serverOptions = serverOptions.Value;
            //_activityService = activityService;
            _perms = perms;
            _buckets = buckets;
            _featureService = featureService;

            fiveEvents.On<Player, string, dynamic, dynamic>(FiveEvents.PlayerConnecting, OnPlayerConnecting);
            fiveEvents.On<Player, string>(FiveEvents.PlayerDropped, OnPlayerDropped);
            fiveEvents.On<Player, string>(FiveEvents.PlayerJoining, OnPlayerJoining);

            //MinuteTimer.Start();
            //MinuteTimer.Elapsed += MinuteTimerOnElapsed;
        }

        private Task OnPlayerJoining([FromSource] Player player,
            string oldId)
        {
            if (_featureService.IsFeatureEnabled(FeatureToggle.IsEarlyAccess))
            {
                // Early Access Server
                var isAdmin = API.IsPlayerAceAllowed(player.Handle, "Police.adminAuth");
                var isDev = API.IsPlayerAceAllowed(player.Handle, "Police.developer");
                var isDonatorPro = API.IsPlayerAceAllowed(player.Handle, "Don.pro");
                var isDigitalTeam = API.IsPlayerAceAllowed(player.Handle, "Digital.Team");
                var isQaTeam = API.IsPlayerAceAllowed(player.Handle, "group.qa");
                var allowedAccess = isAdmin || isDev || isDonatorPro || isDigitalTeam || isQaTeam;
                if (!allowedAccess)
                {
                    player.Drop($"This is only for Early Access members!");
                }
            }
            return Task.CompletedTask;
        }

        /*
        private async Task MinuteTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            MinuteTimer.Stop();

            foreach (var player in _players.ToList())
            {
                try
                {
                    if (player == null) continue;

                    var steamId = player.Identifiers[Identifiers.Steam];

                    var isModerator = API.IsPlayerAceAllowed(player.Handle, "Police.modAuth");

                    var userActivity = await _activityService.GetBySteamIdAsync(steamId) ?? await _activityService.CreateAsync(steamId, player.Name, isModerator);

                    if (userActivity == null) continue;

                    await _activityService.IncrementPlayTime(steamId);
                }
                catch (Exception exception)
                {
                    _logger.Error(exception.ToString());
                    continue;
                }
            }

            MinuteTimer.Start();
        }
        */

        public override async Task Started()
        {
            /*
            await _sessionService.EndAllAsync("Server restarted.");

            _logger.Trace($"Starting sessions for {_players.Count()} online players.");

            foreach (var player in _players)
            {
                var steamId = player.Identifiers[Identifiers.Steam];

                var user = await _userService.GetBySteamIdAsync(steamId) ?? await _userService.CreateAsync(steamId);
                if (user == null)
                {
                    _logger.Trace($"Failed to create user for player \"{player.Name}\"");
                    continue;
                }

                var session = await _sessionService.StartAsync(player, user);
                if (session == null)
                {
                    _logger.Error($"Failed to create session for player \"{player.Name}\" (UserID: {user.UserId}");
                    continue;
                }
                var isModerator = API.IsPlayerAceAllowed(player.Handle, "Police.modAuth");
                var userActivity = await _activityService.GetBySteamIdAsync(steamId) ??
                                   await _activityService.CreateAsync(steamId, player.Name, isModerator);

                if (userActivity == null)
                {
                    _logger.Error($"Failed to create User Activity for player {player.Name} (UserId: {user.UserId})");
                    continue;
                }

                if (userActivity.SteamName != player.Name)
                {
                    await _activityService.UpdateSteamUserName(steamId, player.Name);
                    _logger.Trace($"New Steam Name detected for player ID {steamId}");
                }

                if (userActivity.IsMod != isModerator)
                {
                    await _activityService.UpdateModStatus(steamId, isModerator);
                }

                await _activityService.UpdateLastLogin(steamId);

                _logger.Trace($"Created Session \"{session.SessionId}\" for User \"{user.UserId}\"");
            }*/
        }

        private async Task OnPlayerDropped([FromSource] Player player, string reason)
        {
            _logger.Trace($"Player \"{player.Name}\" dropped: {reason}");
            /*
            if (!await _sessionService.EndAsync(player, reason))
            {
                _logger.Error($"Failed to end session for Player \"{player.Name}\"");
            }*/
        }

        private async Task OnPlayerConnecting([FromSource] Player player,
            string playerName,
            dynamic setKickReason,
            dynamic deferrals)
        {
            deferrals.defer();

            await Delay(0);

            string steamId = player.Identifiers[Identifiers.Steam];

            if (string.IsNullOrEmpty(steamId))
            {
                _logger.Trace($"Player \"{playerName}\" connecting from IP \"{player.Identifiers[Identifiers.IpAddress]}\" failed to connect: No SteamID found");
                deferrals.done("You must have Steam running to play PoliceMP, even if you do not have GTA V on Steam.\n\n " +
                               $"If you need support, then please join our Discord at: {_serverOptions.Discord}");
                return;
            }

            _logger.Trace($"Player \"{playerName}\" connecting from IP \"{player.Identifiers[Identifiers.IpAddress]}\" (SteamID: {steamId})");

            await Delay(0);

            string discordId = player.Identifiers[Identifiers.Discord];

            if (string.IsNullOrEmpty(discordId))
            {
                _logger.Debug($"Player \"{playerName}\" connecting from IP \"{player.Identifiers[Identifiers.IpAddress]}\" failed to connect: No DiscordID found");
                deferrals.done("You must have Discord running to play PoliceMP.\n\n " +
                    $"If you need support, then please join our Discord at: {_serverOptions.Discord}");
                return;
            }

            deferrals.update("Please wait initialise your session...");

            /*
            var user = await _userService.GetBySteamIdAsync(steamId) ?? await _userService.CreateAsync(steamId);
            if (user == null)
            {
                _logger.Trace($"Failed to create user for player \"{playerName}\"");
                deferrals.done("Failed to initialise User data.");
                return;
            }

            _logger.Trace($"Player \"{player.Name}\" is UserId \"{user.UserId}\"");

            await Delay(0);

            var session = await _sessionService.StartAsync(player, user);
            if (session == null)
            {
                _logger.Error($"Failed to create session for player \"{playerName}\" (UserID: {user.UserId}");
                deferrals.done("Failed to create Session data.");
                return;
            }

            var isModerator = API.IsPlayerAceAllowed(player.Handle, "Police.modAuth");

            var userActivity = await _activityService.GetBySteamIdAsync(steamId) ??
                               await _activityService.CreateAsync(steamId, player.Name, isModerator);

            if (userActivity == null)
            {
                _logger.Error($"Failed to create User Activity for player {player.Name} (UserId: {user.UserId})");
                return;
            }

            if (userActivity.SteamName != player.Name)
            {
                await _activityService.UpdateSteamUserName(steamId, player.Name);
                _logger.Trace($"New Steam Name detected for player ID {steamId}");
            }
            if (userActivity.IsMod != isModerator)
            {
                await _activityService.UpdateModStatus(steamId, isModerator);
            }

            await _activityService.UpdateLastLogin(steamId);

            _logger.Trace($"Created Session \"{session.SessionId}\" for User \"{user.UserId}\"");

            await Delay(0);*/

            var chatResourceState = API.GetResourceState("chat");
            if (chatResourceState != null && chatResourceState != "started")
            {
                var chatStopwatch = new Stopwatch();
                chatStopwatch.Start();
                deferrals.update("Waiting on Chat System to Build.");
                while (API.GetResourceState("chat") != "started")
                {
                    if (chatStopwatch.Elapsed.TotalMinutes > 1)
                    {
                        deferrals.update("Chat Resource has taken longer than 1 minute to build. Re-connect");
                        return;
                    }
                    await Delay(1000);
                }
            }

            deferrals.done();
        }
    }
}