using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Server;
using PoliceMP.Core.Server.Commands;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Core.Server.Interfaces.Factories;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Data;
using PoliceMP.Server.Controllers.AiCallouts;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.Highways;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.NHS;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.Police;
using PoliceMP.Server.Factories;
using PoliceMP.Server.Options;
using PoliceMP.Server.Options.Interfaces;
using PoliceMP.Server.Services;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Options;
// using PoliceMP.Server.Controllers.AiCallouts.Callouts.JointResponse;
using Debug = CitizenFX.Core.Debug;

namespace PoliceMP.Server
{
    public interface IPlayerListAccessor
    {
        PlayerList Players { get; }
    }

    public interface IGlobalStateAccessor
    {
        StateBag GlobalState { get; }
    }

    public class Server : BaseScript, IPlayerListAccessor, IGlobalStateAccessor
    {
        private ILogger<Server> _logger;
        private IServerRpcManager _serverRpc;

        private IServiceProvider _serviceProvider;


        public Server()
        {
            EventHandlers["onServerResourceStart"] += new Func<string, Task>(OnResourceStart);
            EventHandlers[RpcConstants.RpcMessage] += new Func<Player, string, Task>(OnRpcMessage);
        }


        public new StateBag GlobalState => base.GlobalState;
        public new PlayerList Players => base.Players;

        public async Task OnResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName)
                return;

            var serviceCollection = new ServiceCollection();
            await SetupServices(serviceCollection);
        }

        public async Task SetupServices(IServiceCollection services)
        {
            Debug.WriteLine("Fetching Configurations");
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, "server/Options"))
                .AddJsonFile("Server.conf.json", false, true)
                .AddJsonFile("Spawn.conf.json", false, true)
                .AddJsonFile("SpeedBumps.conf.json", false, true)
                .AddJsonFile("Auth.conf.json", false, true)
                .AddJsonFile("DiscordRichPresence.conf.json", false, true)
                .AddJsonFile("VehicleFaultGenerator.conf.json", false, true)
                .AddJsonFile("PlayerController.conf.json", false, true)
                .AddJsonFile("World.conf.json", false, true)
                .AddJsonFile("PedInfo.conf.json", false, true)
                .AddJsonFile("VehicleInfo.conf.json", false, true)
                .AddJsonFile("Questions.conf.json", false, true)
                .AddJsonFile("Action.conf.json", false, true)
                .AddJsonFile("Speech.conf.json", false, true)
                .AddJsonFile("Items.conf.json", false, true)
                .AddJsonFile("Offences.conf.json", false, true)
                .AddJsonFile("DogOptions.conf.json", false, true)
                .AddEnvironmentVariables()
                .Build();
            Debug.WriteLine("Configurations Built");

            services.AddScoped(typeof(ILogger<>), typeof(Logger<>));
            services.AddMemoryCache();
            services.AddEventControllers();

            // Disabled for now
            // services.AddPoliceMPWebServices();

            Debug.WriteLine("Fetching Services");

            #region Configuration

            services.AddSingleton<IConfiguration>(config);
            services.Configure<ServerOptions>(options => config.GetSection("Server").Bind(options));
            services.Configure<SpawnOptions>(options => config.GetSection("Spawn").Bind(options));
            services.Configure<SpeedBumpOptions>(options => config.GetSection("SpeedBump").Bind(options));
            services.Configure<AuthOptions>(options => config.GetSection("Auth").Bind(options));
            services.Configure<DiscordRichPresenceOptions>(options =>
                config.GetSection("DiscordRichPresence").Bind(options));
            services.Configure<VehicleFaultGeneratorOptions>(options =>
                config.GetSection("VehicleFaultGenerator").Bind(options));
            services.Configure<PlayerControllerOptions>(options => config.GetSection("PlayerController").Bind(options));
            services.Configure<WorldOptions>(options => config.GetSection("World").Bind(options));
            services.Configure<PedInfoOptions>(options => config.GetSection("PedInfo").Bind(options));
            services.Configure<VehicleInfoOptions>(options => config.GetSection("VehicleInfo").Bind(options));
            services.Configure<QuestionOptions>(options => config.GetSection("Questions").Bind(options));
            services.Configure<ActionOptions>(options => config.GetSection("Action").Bind(options));
            services.Configure<SpeechOptions>(options => config.GetSection("Speech").Bind(options));
            services.Configure<ItemOptions>(options => config.GetSection("Items").Bind(options));
            services.Configure<OffenceOptions>(options => config.GetSection("Offences").Bind(options));
            services.Configure<DogOptions>(options => config.GetSection("DogOptions").Bind(options));

            #endregion Configuration

            #region Core

            services.AddSingleton<IPlayerListAccessor>(this);
            services.AddSingleton<IGlobalStateAccessor>(this);
            services.AddSingleton<ICommandManager, CommandManager>();
            services.AddSingleton<PlayerList>(Players); // TODO: REMOVE THIS!

            services.AddSingleton<IServerEventManager, ServerEventManager>();
            services.AddSingleton<IServerRpcManager, ServerRpcManager>();
            services.AddSingleton<ILegacyServerCommunicationsManager, LegacyServerCommunicationsManager>();
            services.AddSingleton<IFiveEventManager>(new FiveEventManager(EventHandlers));
            services.AddSingleton<IOptionsManager, OptionsManager>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IServerCommunicationsManager, ServerCommunicationsManager>();
            services.AddTransient<IBehaviorService, BehaviorService>();

            services.AddDbContext<GtaDbContext>(options =>
            {
                options.UseMySql(config.GetSection("ConnectionStrings").GetValue<string>("Database"));
            });

            #endregion Core

            #region Services

            //services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IFeatureService, FeatureService>();
            //services.AddSingleton<ISessionService, SessionService>();
            services.AddScoped<INotificationService, NotificationService>();
            //services.AddScoped<IActivityService, ActivityService>();
            services.AddSingleton<IPedInfoService, PedInfoService>();
            services.AddSingleton<IVehicleInfoService, VehicleInfoService>();
            services.AddSingleton<IPedInfoFactory, BogusPedInfoFactory>();
            services.AddSingleton<IVehicleInfoFactory, BogusVehicleInfoFactory>();
            services.AddSingleton<IQuestionService, QuestionService>();
            services.AddScoped<IBucketService, BucketService>();
            services.AddSingleton<ISonoranService, SonoranService>();
            services.AddSingleton<ISoundService, SoundService>();
            services.AddSingleton<AiCalloutsPeds>();
            services.AddSingleton<IXPService, JsonXPService>();
            
            // Callouts
            services.AddTransient<AntiSocialCallout>();
            services.AddTransient<DomesticDisputeCallout>();
            services.AddTransient<FightInProgressCallout>();
            services.AddTransient<BtpFightInProgress>();
            services.AddTransient<BTPGunCallout>();
            services.AddTransient<MentalHealthCrisisCallout>();
            services.AddTransient<MissingPersonCallout>();
            services.AddTransient<RobberyCallout>();
            services.AddTransient<SingleDrugSelling>();
            services.AddTransient<FourOneFiveMirrorParkFireCallout>();
            //services.AddTransient<BrokenDownVehicleCallout>();
            services.AddTransient<FailToStopCallout>();
            //services.AddTransient<VehicleFireCallout>();
            //services.AddTransient<MultiVehicleRtcCallout>();
            services.AddTransient<PublicOrderOffenceCallout>();
            services.AddTransient<MentalHealthWorldCallout>();
            services.AddTransient<AFOSingleCallout>();
            services.AddTransient<FireServiceInspectionCallout>();
            services.AddTransient<ArmedFailToStopCallout>();
            // services.AddTransient<DerailedTrainCallout>();
            services.AddSingleton<UserDivisionPlaytimeLogger>();

            #endregion Services

            _serviceProvider = services.BuildServiceProvider();

            Debug.WriteLine("Service Provider Built");
            
            _serviceProvider.GetRequiredService<UserDivisionPlaytimeLogger>();
            
            // Disabled for now
            // await Task.Run(() => WebServer.BuildAndRun(_serviceProvider, API.GetResourcePath("PoliceMP")));

            var done = false;
            var scope = _serviceProvider.CreateScope();

            if (done) return;
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<Server>>();
            _serverRpc = scope.ServiceProvider.GetRequiredService<IServerRpcManager>();

            var controllersNonUnique = scope.ServiceProvider.GetServices<Controller>().ToList();
            var controllers = controllersNonUnique.Distinct().ToList();

            var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();

            _logger.Trace("Starting Database Connection");

            /*
            if (!await context.Database.CanConnectAsync())
            {
                _logger.Error("Failed to connect to the database! " +
                              "Check the connection string and ensure that the database server is up.");
                _logger.Error("PoliceMP failed to start.");
                return;
            }
            */

            _logger.Trace("Connected to the database successfully.");

            _logger.Debug($"Starting {controllers.Count} controllers.");

            await Task.WhenAll(controllers.Select(c => c.Started()));

            foreach (var controller in controllers.Distinct())
            {
                var tickMethod = controller.GetType()
                    .GetMethod("ControllerTick", BindingFlags.Instance | BindingFlags.NonPublic);
                if (tickMethod == null || tickMethod.DeclaringType == typeof(Controller))
                {
                    _logger.Debug($"No tick required for {controller.GetType().Name}");
                    continue;
                }

                _logger.Debug($"Adding tick handler for script {controller.GetType().Name}");
                await Delay(0);

                Tick += async () =>
                {
                    try
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        await controller.TickAsync();
                        sw.Stop();
                        switch (sw.ElapsedMilliseconds)
                        {
                            case >= 100 and < 250:
                                _logger.Warn($"{controller.GetType().Name} WARN TICK {sw.ElapsedMilliseconds}");
                                break;
                            case >= 250:
                                _logger.Error($"{controller.GetType().Name} BAD TICK {sw.ElapsedMilliseconds}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(
                            $"TICK FOR CONTROLLER {controller.GetType().Name} SHOULD OF CRASHED WITH EXCEPTION {ex.ToString()}");
                    }
                };
            }

            done = true;

            _logger.Debug("PoliceMP started successfully.");
        }

        public async Task OnRpcMessage([FromSource] Player player, string json)
        {
            try
            {
                _logger.Trace($"OnRpcMessage: {json}");
                await _serverRpc.HandleMessage(player, json);
            }
            catch (Exception ex)
            {
                _logger.Error("An RPC Exception occurred", ex);
            }
        }
    }
}