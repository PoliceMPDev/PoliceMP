using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using Color = System.Drawing.Color;
using Vector3 = CitizenFX.Core.Vector3;

namespace PoliceMP.Client.Scripts.DebugScripts
{
    public interface IDebugPedScript
    {
        Ped Ped { get; }
        void SetPed(Ped ped);
        void Enable();
        void Disable();
        bool Enabled { get; }
    }

    public class DebugPedScript : Script, IDebugPedScript
    {
        private readonly ILogger<DebugPedScript> _logger;
        private readonly ICommandManager _commands;
        private readonly ITickManager _ticks;
        private readonly INotificationService _notifications;
        private readonly IPermissionService _permission;
        public Ped Ped { get; private set; } = Game.PlayerPed;

        public bool Enabled { get; set; }
        private readonly Dictionary<PedConfigFlags, bool> _pedConfigFlags = new();

        private readonly List<PedConfigFlags> SuppressNotifications = new()
        {
            PedConfigFlags.NoCriticalHits,
            PedConfigFlags.DrownsInWater,
            PedConfigFlags.DisableReticuleFixedLockon,
            PedConfigFlags.UpperBodyDamageAnimsOnly,
            PedConfigFlags.NeverLeavesGroup,
            PedConfigFlags.BlockNonTemporaryEvents,
            PedConfigFlags.IgnoreSeenMelee,
            PedConfigFlags.DieWhenRagdoll,
            PedConfigFlags.HasHelmet,
            PedConfigFlags.UseHelmet,
            PedConfigFlags.DisableEvasiveDives,
            PedConfigFlags.DontInfluenceWantedLevel,
            PedConfigFlags.DisablePlayerLockon,
            PedConfigFlags.DisableLockonToRandomPeds,
            PedConfigFlags.PedBeingDeleted,
            PedConfigFlags.BlockWeaponSwitching,
            PedConfigFlags.IsFiring,
            PedConfigFlags.WasFiring,
            PedConfigFlags.IsStanding,
            PedConfigFlags.WasStanding,
            PedConfigFlags.InVehicle,
            PedConfigFlags.OnMount,
            PedConfigFlags.AttachedToVehicle,
            PedConfigFlags.IsSwimming,
            PedConfigFlags.WasSwimming,
            PedConfigFlags.IsSkiing,
            PedConfigFlags.IsSitting,
            PedConfigFlags.KilledByStealth,
            PedConfigFlags.KilledByTakedown,
            PedConfigFlags.Knockedout,
            PedConfigFlags.UsingCoverPoint,
            PedConfigFlags.IsInTheAir,
            PedConfigFlags.IsAimingGun,
            PedConfigFlags.ForcePedLoadCover,
            PedConfigFlags.VaultFromCover,
            PedConfigFlags.ForcedAim,
            PedConfigFlags.ForceReload,
            PedConfigFlags.BumpedByPlayer,
            PedConfigFlags.IsHandCuffed,
            PedConfigFlags.IsAnkleCuffed,
            PedConfigFlags.DisableMelee,
            PedConfigFlags.CanBeAgitated,
            PedConfigFlags._FaceDirInsult,
            PedConfigFlags.WillArrestRatherThanJack,
            PedConfigFlags.RidingTrain,
            PedConfigFlags.ArrestResult,
            PedConfigFlags.CanAttackFriendly,
            //PedConfigFlags.ShootingAnimFlag,
            PedConfigFlags.DisableLadderClimbing,
            PedConfigFlags.StairsDetected,
            PedConfigFlags.SlopeDetected,
            PedConfigFlags.CanPerformArrest,
            PedConfigFlags.CanBeArrested,
            PedConfigFlags.IsInjured,
            PedConfigFlags.IsAgitated,
            PedConfigFlags.PreventAutoShuffleToDriversSeat,
            PedConfigFlags.EnableWeaponBlocking,
            PedConfigFlags.HasHurtStarted,
            PedConfigFlags.DisableHurt,
            PedConfigFlags.PlayerIsWeird,
            PedConfigFlags.UsingScenario,
            PedConfigFlags.VisibleOnScreen,
            PedConfigFlags._AvoidUnderSide,
            PedConfigFlags.DisableExplosionReactions,
            PedConfigFlags.DodgedPlayer,
            PedConfigFlags.DontEnterLeadersVehicle,
            PedConfigFlags.DisablePotentialToBeWalkedIntoResponse,
            PedConfigFlags.DisablePedAvoidance,
            PedConfigFlags.DisablePanicInVehicle,
            PedConfigFlags.IsHoldingProp,
            PedConfigFlags._BlocksPathingWhenDead,
            PedConfigFlags.OnStairs,
            PedConfigFlags.OnStairSlope,
            PedConfigFlags.DontBlipCop,
            PedConfigFlags.ClimbedShiftedFence,
            PedConfigFlags._KillWhenTrapped,
            PedConfigFlags.EdgeDetected,
            PedConfigFlags.AvoidTearGas,
            PedConfigFlags.RagdollingOnBoat,
            PedConfigFlags.HasBrandishedWeapon,
            PedConfigFlags.DisableShockingEvents,
            PedConfigFlags.DisablePedConstraints,
            PedConfigFlags.IsInCluster,
            PedConfigFlags.HasHighHeels,
            PedConfigFlags._SpawnedAtScenario,
            PedConfigFlags.DisableTalkTo,
            PedConfigFlags.DontBlip,
            PedConfigFlags.IsSwitchingWeapon,
            PedConfigFlags.EquipJetpack,
            PedConfigFlags.IsDuckingInVehicle,
            PedConfigFlags.HasReserveParachute,
            PedConfigFlags.UseReserveParachute,
            PedConfigFlags.NeverLeaveTrain,
            PedConfigFlags.IsClimbingLadder,
            PedConfigFlags.HasBareFeet,
            PedConfigFlags.IsHolsteringWeapon,
            PedConfigFlags.IsSwitchingHelmetVisor,
            PedConfigFlags.DisableVehicleCombat,
            PedConfigFlags.FallsLikeAircraft,
            PedConfigFlags.DisableStartEngine,
            PedConfigFlags.IgnoreBeingOnFire,
            PedConfigFlags.DisableHomingMissileLockon,
            PedConfigFlags.DisableHelmetArmor,
            PedConfigFlags.PedIsArresting,
            PedConfigFlags.IsDecoyPed,
            PedConfigFlags.CanBeIncapacitated
        };

        public DebugPedScript(
            ILogger<DebugPedScript> logger,
            ICommandManager commands,
            ITickManager ticks,
            INotificationService notifications,
            IPermissionService permission)
        {
            _logger = logger;
            _commands = commands;
            _ticks = ticks;
            _notifications = notifications;
            _permission = permission;
        }

        protected override async Task OnStartAsync()
        {
            var aces = await _permission.GetUserAces();

            if (aces.IsDeveloper || aces.IsTierTwo)
            {
                _commands.Register("togglepeddebug").WithHandler(TogglePedConfigTest);
            }
        }

        private void TogglePedConfigTest()
        {
            if (!Enabled)
                Enable();
            else
                Disable();
        }

        private Task DebugPedTick()
        {
            foreach (var config in Enum.GetValues(typeof(PedConfigFlags)))
            {
                var enumValue = (int)config;
                var value = Ped.GetConfigFlag((int)config);
                if (_pedConfigFlags.TryGetValue((PedConfigFlags)enumValue, out var existingValue))
                {
                    if (existingValue != value)
                    {
                        _logger.Debug($"PedConfigFlag changed! ({config}) {existingValue} => {value}");
                        _pedConfigFlags[(PedConfigFlags)enumValue] = value;

                        if (!SuppressNotifications.Contains((PedConfigFlags)enumValue))
                            _notifications.Info("Config Change",
                                $"PedConfigFlag changed! ({config}) {existingValue} => {value}");
                    }
                }
                else
                {
                    _logger.Debug($"PedConfigFlag initialized! ({config}) {value}");
                    _pedConfigFlags.Add((PedConfigFlags)enumValue, value);
                }
            }

            World.DrawMarker(MarkerType.DebugSphere, Ped.Position + Vector3.ForwardLH, Vector3.Zero, Vector3.Zero,
                Vector3.One * 0.2f, Color.FromArgb(50, 50, 255));

            return Task.FromResult(0);
        }

        public void SetPed(Ped ped)
        {
            _pedConfigFlags.Clear();
            Ped = ped;
            _notifications.Info("ped network", $"{ped.NetworkId}");
        }

        public void Enable()
        {
            if (!Enabled)
            {
                Ped = Game.PlayerPed;
                _pedConfigFlags.Clear();
                Enabled = true;
                _ticks.On(DebugPedTick);
            }
        }

        public void Disable()
        {
            if (Enabled)
            {
                Enabled = false;
                _ticks.Off(DebugPedTick);
            }
        }
    }
}