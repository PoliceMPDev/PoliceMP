using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Behaviors.Jump;

namespace PoliceMP.Client.Behaviors.AiCallouts
{
    public class StealACarImplementation : PedBehavior<StealACarBehaviour>
    {
        private readonly ILogger<StealACarBehaviour> _logger;
        private readonly IBehaviorService _behaviors;
        private readonly IPlayerListAccessor _playerListAccessor;

        private const float HostMigrateThreshold = 10f;


        public StealACarImplementation(ITickManager ticks, ILogger<StealACarBehaviour> logger,
            IBehaviorService behaviors, IPlayerListAccessor playerListAccessor) : base(ticks)
        {
            _logger = logger;
            _behaviors = behaviors;
            _playerListAccessor = playerListAccessor;
        }

        public override void Initialize()
        {
            Blackboard.Set(bb => bb.StealingState, StealingState.Idle);
        }

        protected override void Start()
        {
            ThePed.Task.HandsUp(5000);
            _behaviors.AddPedBehavior<JumpBehavior>(ThePed);
        }

        protected override async Task Think()
        {
            var position = ThePed.Position;

            Ped entityFound = null;
            foreach (var player in _playerListAccessor.Players)
            {
                if (World.GetDistance(player.Character.Position, position) > 10f) continue;
                if (player.Character == null) continue;

                entityFound = player.Character;
                _logger.Debug("Entity found in area: " + player.Name);
            }

            if (entityFound != null && Blackboard.Get(bb => bb.StealingState) == StealingState.Idle)
            {
                // API.TaskCombatPed(ThePed.Handle, entityFound.Handle, 0, 16);
                Blackboard.Set(bb => bb.StealingState, StealingState.Stealing);
                API.SetPedFleeAttributes(ThePed.Handle, 38, true);

                // API.SetPedFleeAttributes(ThePed.Handle, 38, true);
            }
        }

        protected override async Task ThinkRemote()
        {
            var currentHost = API.NetworkGetEntityOwner(ThePed.Handle);
            var hostServerId = Blackboard.Get(bb => bb.HostServerId);
            var nextAbleToMigrate = Blackboard.Get(bb => bb.NextAbleToMigrate);
            var serverTime = ServerInfo.GetServerTime();

            if (nextAbleToMigrate > serverTime.TotalSeconds) return;

            if (hostServerId != Game.Player.ServerId && nextAbleToMigrate < serverTime.TotalSeconds)
            {
                var hostPed = Entity.FromHandle(API.GetPlayerPed(currentHost));
                var hostDistance = World.GetDistance(ThePed.Position, hostPed?.Position ?? Vector3.Zero);

                if (hostDistance > HostMigrateThreshold)
                {
                    var distance = World.GetDistance(ThePed.Position, Game.PlayerPed.Position);
                    if (distance < hostDistance)
                    {
                        if (ThePed.CurrentVehicle != null)
                        {
                            if (await ThePed.TryRequestNetworkEntityControl(false, 100) &&
                                await ThePed.CurrentVehicle.TryRequestNetworkEntityControl(false, 100))
                            {
                                API.SetEntityAsMissionEntity(ThePed.Handle, true, true);
                                API.SetEntityAsMissionEntity(ThePed.CurrentVehicle.Handle, true, true);
                            }
                        }
                        else if (await ThePed.TryRequestNetworkEntityControl(false, 100))
                            API.SetEntityAsMissionEntity(ThePed.Handle, true, true);

                        SetMigrateInformation();

                        return;
                    }
                }
            }

            var handle = ThePed.Handle;
            API.SetEntityAsNoLongerNeeded(ref handle);
            if (ThePed.CurrentVehicle != null)
            {
                handle = ThePed.CurrentVehicle.Handle;
                API.SetEntityAsNoLongerNeeded(ref handle);
            }
        }
    }
}