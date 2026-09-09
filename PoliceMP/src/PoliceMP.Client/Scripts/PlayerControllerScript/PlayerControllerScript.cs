using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Scripts.PlayerControllerScript.Interfaces;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Client.Utils;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Options.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Enums;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Options;
using Stateless;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.UI;
using PoliceMP.Client.Overlays.Interaction;
using PoliceMP.Client.Scripts.DebugScripts;
using PoliceMP.Client.Scripts.Dsu;
using PoliceMP.Client.Scripts.Pullover;
using PoliceMP.Client.Utils.CameraUtils;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.FailToStop;
using PoliceMP.Shared.Constants.Decors;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    public enum PlayerState
    {
        Unknown,
        Normal,
        Interacting,
        PerformingAction,
        EscortingPed,
        AimingDownSight
    }

    public enum PlayerTriggers
    {
        ResourceStarted,
        Interact,
        StopInteracting,
        PerformAction,
        FinishPerformingAction,
        GrabPed,
        ContinueEscortingPed,
        ReleasePed,
        AimDownSight,
        StopAimDownSight
    }

    public class PlayerController : Script, IPlayerController
    {
        private Ped PlayerPed => Game.PlayerPed;
        private readonly ILogger<PlayerController> _log;
        private readonly ITickManager _tickManager;
        private readonly IOptionsManager _optionsManager;
        private PlayerControllerOptions _options;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly IAnimationService _anims;
        private readonly IPersonalityService _personality;
        private readonly ISpeechService _speech;
        private readonly IPulloverScript _pulloverScript;
        private readonly INotificationService _notifications;
        private readonly IDebugPedScript _debugPed;
        private readonly IGameInputManager _gameInput;
        private readonly IInteractionHud _interactionHud;
        private readonly IBehaviorService _behaviors;
        private readonly IDogScript _dog;
        private Entity _lastAdsEntity = null;
        private Weapon _previousWeapon;
        private bool ShouldUseFarDistance() => _dog.IsActive();

        private const uint _weaponHoseHash = 2739996767;

        #region State Machine Vars

        private readonly StateMachine<PlayerState, PlayerTriggers> _stateMachine;
        private readonly StateMachine<PlayerState, PlayerTriggers>.TriggerWithParameters<Entity, PlayerAction> _triggerAction;
        private readonly StateMachine<PlayerState, PlayerTriggers>.TriggerWithParameters<Ped> _triggerEscort;
        private PlayerState _previousState = PlayerState.Unknown;

        #endregion

        #region Interaction Vars

        public Entity InteractionTarget { get; private set; }
        public Entity LastInteractionTarget { get; private set; }
        public Ped _grabbedPed = null;
        private readonly ICommandManager _commands;
        private readonly IContextualActionsProvider _contextualActions;
        private readonly List<Tuple<int, Control>> _disabledActionControls = new();
        private Task _currentInteractionTask;

        private const IntersectOptions InteractionCameraIntersects = (IntersectOptions)int.MaxValue & ~IntersectOptions.Peds1;

        // How far away is the camera pushed away from walls.
        private const float InteractionCameraSize = 0.2f;

        #endregion

        public PlayerController(ILogger<PlayerController> log,
            ITickManager tickManager,
            IOptionsManager optionsManager,
            ICommandManager commands,
            IContextualActionsProvider contextualActions,
            ILegacyClientCommunicationsManager comms,
            IAnimationService anims,
            IPersonalityService personality,
            ISpeechService speech,
            IPulloverScript pulloverScript,
            INotificationService notifications,
            IDebugPedScript debugPed,
            IGameInputManager gameInput,
            IInteractionHud interactionHud,
            IBehaviorService behaviors,
            IDogScript dog)
        {
            _log = log;
            _tickManager = tickManager;
            _optionsManager = optionsManager;
            _commands = commands;
            _contextualActions = contextualActions;
            _comms = comms;
            _anims = anims;
            _personality = personality;
            _speech = speech;
            _pulloverScript = pulloverScript;
            _notifications = notifications;
            _debugPed = debugPed;
            _gameInput = gameInput;
            _interactionHud = interactionHud;
            _behaviors = behaviors;
            _dog = dog;
            RegisterDecor(EntityDecors.IsBeingInteractedWith, DecorType.Bool);
            RegisterDecor(EntityDecors.InteractionLockNetworkId, DecorType.Int);

            #region Configure State Machine

            _stateMachine = new StateMachine<PlayerState, PlayerTriggers>(PlayerState.Unknown);
            _triggerAction = _stateMachine.SetTriggerParameters<Entity, PlayerAction>(PlayerTriggers.PerformAction);
            _triggerEscort = _stateMachine.SetTriggerParameters<Ped>(PlayerTriggers.GrabPed);

            // Log the State Machine transitions
            //_stateMachine.OnTransitioned(transition => _log.Debug($"Player State Transition: {transition.Source} => {transition.Destination} via {transition.Trigger}"));
            _stateMachine.OnTransitioned(transition => _previousState = transition.Source);

            SetupStateTransitions();
            #endregion
        }

        #region Setup
        /// <summary>
        /// Sets up the permitted transitions between states
        /// </summary>
        private void SetupStateTransitions()
        {
            // I can go from an unknown state to my normal state when the resource is started
            _stateMachine.Configure(PlayerState.Unknown)
                .Permit(PlayerTriggers.ResourceStarted, PlayerState.Normal);

            // I can interact only when my state is normal
            _stateMachine.Configure(PlayerState.Normal)
                .OnEntryFrom(PlayerTriggers.StopInteracting, () =>
                {
                    //_log.Debug("Stopped Interacting");
                    //_lastAdsEntity?.ReleaseInteractionLock();
                    //InteractionTarget?.ReleaseInteractionLock();
                })
                .Permit(PlayerTriggers.Interact, PlayerState.Interacting)
                .Permit(PlayerTriggers.GrabPed, PlayerState.EscortingPed)
                .Permit(PlayerTriggers.AimDownSight, PlayerState.AimingDownSight)
                .Permit(PlayerTriggers.PerformAction, PlayerState.PerformingAction);

            _stateMachine.Configure(PlayerState.EscortingPed)
                .Permit(PlayerTriggers.ReleasePed, PlayerState.Normal)
                .Permit(PlayerTriggers.PerformAction, PlayerState.PerformingAction)
                .Permit(PlayerTriggers.Interact, PlayerState.Interacting)
                .OnEntryFromAsync(_triggerEscort, OnEscortPedEnter)
                .OnExitAsync(OnEscortPedExit);

            _stateMachine.Configure(PlayerState.Interacting)
                .Permit(PlayerTriggers.StopInteracting, PlayerState.Normal)
                .Permit(PlayerTriggers.PerformAction, PlayerState.PerformingAction)
                .Permit(PlayerTriggers.ContinueEscortingPed, PlayerState.EscortingPed)
                .Permit(PlayerTriggers.AimDownSight, PlayerState.AimingDownSight)
                .OnEntry(InteractEnter)
                .OnExitAsync(OnInteractExit);

            _stateMachine.Configure(PlayerState.PerformingAction)
                .OnEntryFromAsync(_triggerAction, OnPerformingActionEnter)
                .Permit(PlayerTriggers.FinishPerformingAction, PlayerState.Normal)
                .Permit(PlayerTriggers.GrabPed, PlayerState.EscortingPed)
                .Permit(PlayerTriggers.ContinueEscortingPed, PlayerState.EscortingPed)
                .OnExitAsync(OnPerformingActionExit);

            _stateMachine.Configure(PlayerState.AimingDownSight)
                .OnEntryAsync(OnAimingDownSightEnter)
                .Permit(PlayerTriggers.StopAimDownSight, PlayerState.Normal)
                .Permit(PlayerTriggers.PerformAction, PlayerState.PerformingAction)
                .OnExitAsync(OnAimingDownSightExit);
        }

        protected override async Task OnStartAsync()
        {
            _options = _optionsManager.Options.PlayerController;
            //_log.Debug("Setting up FSM...");
            await _stateMachine.FireAsync(PlayerTriggers.ResourceStarted);
            _tickManager.On(PlayerControllerTick);
            _tickManager.On(() => DisableRequiredControls());
            _tickManager.On(ShowTargetConeTick);
        }

        private async Task PlayerControllerTick()
        {
            if (_grabbedPed != null && !_grabbedPed.IsAttachedTo(PlayerPed))
            {
                //_grabbedPed.ReleaseInteractionLock();
                _grabbedPed = null;
            }

            if (_stateMachine.IsInState(PlayerState.Normal))
                await NormalTick();

            if (_stateMachine.IsInState(PlayerState.Interacting))
                await InteractTick();

            if (_stateMachine.IsInState(PlayerState.PerformingAction))
                await PerformActionTick();

            if (_stateMachine.IsInState(PlayerState.EscortingPed))
                await EscortPedTick();

            if (_stateMachine.IsInState(PlayerState.AimingDownSight))
                await AimDownSightsTick();
        }

        #endregion

        #region Normal State
        private async Task NormalTick()
        {
            ResetControl();
            var currentWeapon = PlayerPed.Weapons.Current;

            if ((uint)currentWeapon.Hash == _weaponHoseHash)
            {
                return;
            }

            if (PlayerPed.CurrentVehicle == null 
                && !PlayerPed.IsInCombat
                && !PlayerPed.IsAiming)
            {
                Game.DisableControlThisFrame(32, Control.Aim);

                if (_gameInput.IsPressed(Control.Aim) && _gameInput.IsPressed(Control.CharacterWheel))
                {
                    await Delay(0);
                    await DisableRequiredControls(true);
                    await _stateMachine.FireAsync(PlayerTriggers.AimDownSight);
                    return;
                }
                
                if (Game.IsDisabledControlPressed(32, Control.Aim) && World.RenderingCamera.Handle == -1)
                {
                    await _stateMachine.FireAsync(PlayerTriggers.Interact);
                }
                else
                {
                    await DisableRequiredControls(true);
                    if (Game.IsDisabledControlPressed(32, Control.Aim)
                        && Game.IsDisabledControlPressed(32, Control.Attack)) // If the player is holding both aim and attack
                    {
                        while (Game.IsDisabledControlPressed(32, Control.Attack))
                        {
                            await Delay(0);
                            await DisableRequiredControls(true);
                            Game.DisableControlThisFrame(32, Control.Aim);
                        }

                        await Delay(0);
                        await DisableRequiredControls(true);
                        await _stateMachine.FireAsync(PlayerTriggers.AimDownSight);
                    }
                }
            }
            else if (PlayerPed.IsAiming && CanAdsWithWeapon(currentWeapon))
            {
                await _stateMachine.FireAsync( PlayerTriggers.AimDownSight);
            }

            if (MenuController.DisableBackButton
                && !Game.IsDisabledControlPressed(32, Control.Aim))
            {
                MenuController.DisableBackButton = false;
            }

            var actions = _contextualActions.GetNormalActions(this);
            if (actions.Any(a => a.IsValid()))
            {
                if (!_interactionHud.Enabled)
                {
                    _interactionHud.ClearInteractionContext();
                    _interactionHud.Enable();
                    while (!_interactionHud.Enabled)
                    {
                        await Delay(0);
                    }
                }

                await HandlePlayerActions(actions);
            }
            else
            {
                if (_interactionHud.Enabled)
                {
                    _interactionHud.Disable();
                    while (_interactionHud.Enabled)
                    {
                        await Delay(0);
                    }
                }
            }
        }
        #endregion

        #region Interacting State

        private void InteractEnter()
        {
            _interactionHud.Enable();
            _tickManager.On(ShowReticleTick);
        }
        
        private async Task InteractTick()
        {
            // Check if we should leave the interact state because the player is engaging combat
            
            if (_grabbedPed != null
                && PlayerPed.CurrentVehicle != null)
            {
                await _stateMachine.FireAsync(PlayerTriggers.ContinueEscortingPed);
                return;
            }
            
            Game.DisableControlThisFrame(32, Control.Attack);
            if (Game.IsDisabledControlPressed(32, Control.Attack))
            {
                while (Game.IsDisabledControlPressed(32, Control.Attack))
                {
                    await DisableRequiredControls(true);
                    Game.DisableControlThisFrame(32, Control.Aim);
                    await Delay(0);
                }

                await Delay(0);
                await _stateMachine.FireAsync(PlayerTriggers.AimDownSight);
            }
            
            if (!Game.IsDisabledControlPressed(32, Control.Aim))
            {
                await _stateMachine.FireAsync(PlayerTriggers.StopInteracting);
                return;
            }

            // If we're escorting someone ignore peds.
            var includePeds = !_stateMachine.IsInState(PlayerState.EscortingPed);
            if (!TryGetInteractionTarget(out var target, includePeds, true, true)
                && InteractionTarget != null)
            {
                LastInteractionTarget = InteractionTarget;
                InteractionTarget = null;
                _interactionHud.ClearInteractionContext();
                return;
            }

            if (InteractionTarget != null)
            {
                LastInteractionTarget = InteractionTarget;
            }

            InteractionTarget = target;
            if (InteractionTarget == null)
            {
                if (_grabbedPed != null)
                    await _stateMachine.FireAsync(PlayerTriggers.ContinueEscortingPed);
                
                _interactionHud.ClearInteractionContext();
                return;
            }

            #region InteractionActions

            var interactionActions = _contextualActions.GetInteractionActions(this, InteractionTarget);
            await HandlePlayerActions(interactionActions, InteractionTarget);

            #endregion
            
            if (!_interactionHud.Enabled)
            {
                _interactionHud.Enable();
            }
            
            _interactionHud.SetInteractionEntity(InteractionTarget);
        }

        private async Task OnInteractExit()
        {
            LastInteractionTarget = InteractionTarget;
            InteractionTarget = null;
            _disabledActionControls.Clear();

            PlayerPed.Task.ClearLookAt();

            _interactionHud.ClearInteractionContext();
            _interactionHud.Disable();
            _tickManager.Off(ShowReticleTick);
        }
        #endregion

        #region Escorting Ped State

        private async Task OnEscortPedEnter(Ped ped)
        {
            if (!await ped.TryRequestNetworkEntityControl())
            {
                _log.Error($"Failed to obtain network control for entity {ped.NetworkId}");
                await _stateMachine.FireAsync(PlayerTriggers.ReleasePed);
                return;
            }

            //_log.Debug($"Got network control for ped {ped.NetworkId}");

            if (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed)
            {
                _previousWeapon = Game.PlayerPed.Weapons.Current;
                Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            }
            
            _grabbedPed = ped;
        }

        private async Task EscortPedTick()
        {
            Game.DisableControlThisFrame(32, Control.Aim);
            await DisableRequiredControls(forceDisableAttackControls: true);

            if (!API.IsEntityPlayingAnim(PlayerPed.Handle, "rcmnigel1d", "base_club_shoulder", 3))
                await _anims.Play(Game.PlayerPed, "rcmnigel1d", "base_club_shoulder", 8f, flag: 50, duration: -1);

            if (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed)
            {
                _previousWeapon = Game.PlayerPed.Weapons.Current;
                Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            }

            if (_grabbedPed == null 
                || !API.DoesEntityExist(_grabbedPed.Handle) 
                || !API.IsEntityAPed(_grabbedPed.Handle) 
                || !_grabbedPed.IsAttachedTo(PlayerPed)
                //|| !_grabbedPed.HasInteractionLock() && !await _grabbedPed.TryObtainInteractionLock(100)
                )
            {
                await _stateMachine.FireAsync(PlayerTriggers.ReleasePed);
            }
            
            Entity adsEntity = null;
            if (_gameInput.IsPressed(Control.Aim))
            {
                TryGetInteractionTarget(out adsEntity, includePeds: false);
            }
            
            var actions = _contextualActions.GetAttachedActions(this, _grabbedPed, adsEntity);

            if (!_interactionHud.Enabled)
            {
                _interactionHud.Enable();
            }
            
            _interactionHud.SetInteractionEntity(_grabbedPed);
            _interactionHud.SetInteractionActions(actions);

            await HandlePlayerActions(actions, GrabbedPed);
        }

        private async Task OnEscortPedExit()
        {
            //_grabbedPed?.ReleaseInteractionLock();
            _disabledActionControls.Clear();
            _interactionHud.ClearInteractionContext();
            _interactionHud.Disable();

            PlayerPed.Task.ClearSecondary();

            if (_previousWeapon != null)
            {
                Game.PlayerPed.Weapons.Select(_previousWeapon ?? WeaponHash.Unarmed);
                _previousWeapon = null;
            }
        }

        #endregion

        #region Performing Action State

        private async Task OnPerformingActionEnter(Entity entity, PlayerAction action)
        {
            if (_interactionHud.Enabled)
            {
                _interactionHud.Disable();
            }

            if(action.Flags.HasFlag(PlayerActionFlags.DisablePlayerControls))
                SetControl(false, PlayerControlFlag.LeaveCameraControlOn | PlayerControlFlag.AmbientScript);

            if (!Game.PlayerPed.IsAiming && action.Flags.HasFlag(PlayerActionFlags.DisablePlayerControls))
            {
                _previousWeapon = Game.PlayerPed.Weapons.Current;
                Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            }

            bool flee = false;
            Ped pedEntity = null;

            if (action.Flags.HasFlag(PlayerActionFlags.PedTargetCanFlee))
            {
                if (entity is Ped ped)
                    pedEntity = ped;

                else if (entity is Vehicle vehicle && vehicle.Driver != null)
                    pedEntity = vehicle.Driver;

                flee = await _personality.WillPedResist(pedEntity);
            }

            if (flee && pedEntity != null && !action.Flags.HasFlag(PlayerActionFlags.IgnoreAlwaysFlee))
            {
                var vehicle = pedEntity.CurrentVehicle;
                if (vehicle != null)
                {
                    await _pulloverScript.ForceRelease();

                    Screen.ShowSubtitle($"~r~The driver of the {vehicle.LocalizedName} has driven off!");
                    
                    _notifications.Warning("Vehicle Fleeing!",
                        $"The {vehicle.LocalizedName} has failed to stop!");
                }

                pedEntity.SetBoolDecor(PedDecors.ALWAYS_FLEE, true);
                // await pedEntity.DesperatelyFlee(PlayerPed);
                _behaviors.SetPedBehavior<FailToStopBehavior>(pedEntity);

                _speech.Say(pedEntity, "You'll never take me alive!");
            }
            else
            {
                try
                {
                    pedEntity?.SetBoolDecor(PedDecors.ALWAYS_FLEE, false);
                    _currentInteractionTask = action.Callback(); }
                catch
                {
                    _log.Trace("Action failed!");
                    throw;
                }
            }
        }

        private async Task PerformActionTick()
        {
            if (_currentInteractionTask == null ||
                _currentInteractionTask.IsCompleted)
            {
                if (_grabbedPed != null)
                {
                    await _stateMachine.FireAsync(PlayerTriggers.ContinueEscortingPed);
                }
                else
                {
                    await _stateMachine.FireAsync(PlayerTriggers.FinishPerformingAction);
                }
            }

            if (_interactionHud.Enabled)
            {
                _interactionHud.Disable();
                _interactionHud.ClearInteractionContext();
            }
        }

        private Task OnPerformingActionExit()
        {
            _disabledActionControls.Clear();
            ResetControl();
            
            _currentInteractionTask = null;
            //InteractionTarget?.ReleaseInteractionLock();

            if (_previousWeapon != null)
            {
                Game.PlayerPed.Weapons.Select(_previousWeapon ?? WeaponHash.Unarmed);
                _previousWeapon = null;
            }

            if (!Game.PlayerPed.IsAiming)
            {
                PlayerPed.Task.ClearAll();
            }
            else
            {
                PlayerPed.Task.ClearSecondary();
            }

            return Task.FromResult(0);
        }

        #endregion

        #region Aim Down Sight State

        private async Task AimDownSightsTick()
        {
            if (!PlayerPed.IsAiming && !Game.IsControlPressed(0, Control.Aim))
            {
                await _stateMachine.FireAsync(PlayerTriggers.StopAimDownSight);
                return;
            }
            
            int handle = -1;
            bool entityFound = API.GetEntityPlayerIsFreeAimingAt(Game.Player.Handle, ref handle);

            if (!entityFound)
            {
                return;
            }

            if (!API.DoesEntityExist(handle)) return;
            
            if (API.IsEntityAPed(handle))
            {
                _lastAdsEntity = Entity.FromHandle(handle);
            }

            if (_lastAdsEntity != null)
            {
                var actions = _contextualActions.GetAimDownSightActions(this, _lastAdsEntity);
                if (!_interactionHud.Enabled)
                {
                    _interactionHud.Enable();
                }

                _interactionHud.SetInteractionEntity(_lastAdsEntity);
                _interactionHud.SetInteractionActions(actions);

                await HandlePlayerActions(actions, _lastAdsEntity);
            }


        }

        private async Task OnAimingDownSightEnter()
        {
            //_log.Debug("OnAimingDownSightEnter");
            _lastAdsEntity = null;
            _disabledActionControls.Clear();
            _interactionHud.ClearInteractionContext();
        }

        private async Task OnAimingDownSightExit()
        {
            //_log.Debug("OnAimingDownSightExit");
            //_lastAdsEntity?.ReleaseInteractionLock();
            _lastAdsEntity = null;
            _disabledActionControls.Clear();
            //InteractionTarget?.ReleaseInteractionLock();

            _interactionHud.ClearInteractionContext();
        }

        #endregion

        #region Helper Methods
        private Task DisableRequiredControls(bool forceDisableAttackControls = false)
        {
            // If the ped is attacking the player. Remove the restrictions
            if (InteractionTarget != null && API.IsPedInCombat(InteractionTarget.Handle, PlayerPed.Handle))
            {
                return Task.FromResult(0);
            }

            if ((uint)PlayerPed.Weapons.Current.Hash == _weaponHoseHash)
            {
                return Task.FromResult(0);
            }

            var performingActionForAds =
                State == PlayerState.PerformingAction && _previousState == PlayerState.AimingDownSight;

            if (forceDisableAttackControls || (State != PlayerState.AimingDownSight && !performingActionForAds))
            {
                if (forceDisableAttackControls || Game.PlayerPed.Weapons.Current.Hash == WeaponHash.Unarmed)
                {
                    Game.DisableControlThisFrame(32, Control.Attack);
                    Game.DisableControlThisFrame(32, Control.Attack2);
                }

                Game.DisableControlThisFrame(32, Control.Aim);
                Game.DisableControlThisFrame(32, Control.MeleeAttack1);
                Game.DisableControlThisFrame(32, Control.MeleeAttack2);
                Game.DisableControlThisFrame(32, Control.MeleeAttackAlternate);
                Game.DisableControlThisFrame(32, Control.MeleeAttackHeavy);
                Game.DisableControlThisFrame(32, Control.MeleeAttackLight);
            }

            foreach (var control in _disabledActionControls)
                Game.DisableControlThisFrame(control.Item1, control.Item2);

            return Task.FromResult(0);
        }


        private bool TryGetInteractionTarget(out Entity target, bool includePeds = true, bool includeVehicles = true, bool includeObjects = true)
        {
            //TODO: Replace this with proper vehicle values for normal and big vehicles
            var maxDistance = ShouldUseFarDistance() 
                ? _options.InteractionCameraLockOnRangeOnFootFar
                : _options.InteractionCameraLockOnRangeOnFootClose;
            
            var gameplayCamCoord = API.GetGameplayCamCoord();
            var gameplayCamForward = GameMath.RotationToDirection(API.GetGameplayCamRot(0));
            var distanceToPlayer = World.GetDistance(gameplayCamCoord, Game.PlayerPed.Position);
            
            Screen.Hud.ShowComponentThisFrame(HudComponent.Reticle);
            
            target = null;
            
            if (!includePeds && !includeVehicles && !includeObjects)
            {
                throw new ArgumentException(
                    "Cannot find target as includePeds, includeVehicle, and includeObjects are all false!");
            }
            
            RaycastResult raycast = new RaycastResult();
            // High precision tests
            if (includePeds)
            {
                raycast = World.Raycast(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    IntersectOptions.Peds1,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 255, 0, 0));
            }

            if (!raycast.DitHitEntity && includeVehicles)
            {
                raycast = World.Raycast(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    IntersectOptions.MissionEntities,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 0, 255, 0));
            }

            if (!raycast.DitHitEntity && includeObjects)
            {
                raycast = World.Raycast(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    IntersectOptions.Objects,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 0, 0, 255));
            }
            
            // Low precision tests
            if (!raycast.DitHitEntity && includePeds)
            {
                raycast = World.RaycastCapsule(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    1f,
                    IntersectOptions.Peds1,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 255, 255, 0));
            }

            if (!raycast.DitHitEntity && includeVehicles)
            {
                raycast = World.RaycastCapsule(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    1f,
                    IntersectOptions.MissionEntities,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 255, 0, 255));
            }

            if (!raycast.DitHitEntity && includeObjects)
            {
                raycast = World.RaycastCapsule(
                    gameplayCamCoord + gameplayCamForward * distanceToPlayer,
                    gameplayCamForward,
                    maxDistance + distanceToPlayer,
                    1f,
                    IntersectOptions.Objects,
                    Game.PlayerPed);
                
                if(DebugUtils.DebugEnabled)
                    World.DrawMarker(MarkerType.DebugSphere, raycast.HitPosition, Vector3.Zero, Vector3.Zero, Vector3.One * 0.2f, Color.FromArgb(255, 0, 255, 255));
            }

            if (!raycast.DitHitEntity)
            {
                return false;
            }

            Entity entity = raycast.HitEntity;
            if (!entity.Exists() 
                || World.GetDistance(Game.PlayerPed.Position, entity.Position) > maxDistance)
            {
                return false;
            }

            if (entity is Vehicle vehEntity
                && (vehEntity.HasDriver() || vehEntity.PassengerCount > 0))
            {
                if (vehEntity.Driver.Exists() && !vehEntity.Driver.IsPlayer)
                {
                    entity = vehEntity.Driver;
                }
                else
                {
                    entity = vehEntity.Passengers.FirstOrDefault(p => p.Exists() && !p.IsPlayer) ?? entity;
                }
            }

            target = entity;
            return true;
        }

        private async Task HandlePlayerActions(IList<PlayerAction> actions, Entity entity = null)
        {
            entity ??= new Ped(0);
            _disabledActionControls.Clear();

            bool isFarAway = false;
            if (entity.Exists())
            {
                var camCoord = API.GetGameplayCamCoord();
                var distance = World.GetDistance(camCoord, entity.Position) -
                               World.GetDistance(camCoord, Game.PlayerPed.Position);

                if (distance > _options.InteractionCameraLockOnRangeOnFootClose)
                {
                    isFarAway = true;
                }
            }

            var validActions = actions.Where(a => a.IsValid() 
                                                  && (!isFarAway || a.Flags.HasFlag(PlayerActionFlags.AllowFarAway)))
                .ToList();
            
            _interactionHud.SetInteractionActions(validActions);
            foreach (var action in validActions)
            {
                if (!action.Flags.HasFlag(PlayerActionFlags.InputPassThrough))
                {
                    if (Game.CurrentInputMode == InputMode.MouseAndKeyboard)
                    {
                        if (Game.IsControlEnabled(0, action.MouseAndKeyboardControl))
                            Game.DisableControlThisFrame(0, action.MouseAndKeyboardControl);

                        _disabledActionControls.Add(new Tuple<int, Control>(0, action.MouseAndKeyboardControl));
                    }
                    else if (Game.CurrentInputMode == InputMode.GamePad)
                    {
                        if (Game.IsControlEnabled(2, action.GamepadControl))
                            Game.DisableControlThisFrame(2, action.GamepadControl);

                        _disabledActionControls.Add(new Tuple<int, Control>(2, action.GamepadControl));
                    }
                }

                bool performAction = false;
                // New input manager -- everything in here needs updated to use this!
                if(action.Flags.HasFlag(PlayerActionFlags.HoldControl))
                {
                    performAction = _gameInput.IsBeingHeld(action.MouseAndKeyboardControl, action.GamepadControl);

                    // Handle actions where we want to hold, and have no actions that require a tap, pass the tap input back to the game
                    if (!performAction && _gameInput.IsJustReleased(action.MouseAndKeyboardControl, action.GamepadControl) &&
                        !validActions.Any(a =>
                            (Game.CurrentInputMode == InputMode.MouseAndKeyboard && a.MouseAndKeyboardControl == action.MouseAndKeyboardControl || 
                             Game.CurrentInputMode == InputMode.GamePad && a.GamepadControl == action.GamepadControl) &&
                            !a.Flags.HasFlag(PlayerActionFlags.HoldControl)))
                    {
                        _log.Debug($"Simulating keypress {(Game.CurrentInputMode == InputMode.MouseAndKeyboard ? action.MouseAndKeyboardControl : action.GamepadControl)}");

                        if (Game.CurrentInputMode == InputMode.MouseAndKeyboard)
                        {
                            Game.EnableControlThisFrame(0, action.MouseAndKeyboardControl);
                            Game.SetControlNormal(0, action.MouseAndKeyboardControl, 1.0f); // Sets the control on the next frame
                        }
                        else
                        {
                            Game.EnableControlThisFrame(2, action.GamepadControl);
                            Game.SetControlNormal(2, action.GamepadControl, 1.0f); // Sets the control on the next frame
                        }

                        // We're waiting 2 ticks here
                        await Delay(0); // Allow the input to fire next frame
                    }
                }
                else
                {
                    performAction = _gameInput.IsJustReleased(action.MouseAndKeyboardControl, action.GamepadControl);
                }

                if(performAction)
                {
                    //_log.Debug($"Attempting to Preform Action: {action.Name}");
                    
                    var skipNetwork = action.Flags.HasFlag(PlayerActionFlags.SkipNetworkTakeover);
                    var requireNetwork = skipNetwork || !action.Flags.HasFlag(PlayerActionFlags.IgnoreNetworkTakeoverFail);
                    var timeout = action.Flags.HasFlag(PlayerActionFlags.NoWaitForNetworkTimeout) ? 1 : 100;
                    SetControl(false, PlayerControlFlag.LeaveCameraControlOn);

                    var isPlayer = API.IsPedAPlayer(entity.Handle);
                    if (!isPlayer)
                    {
                        //_log.Debug($"Action: {action.Name} - Attempting to gain Network Control of {entity.Handle} ");
                        if (entity.Exists() && !skipNetwork && !await entity.TryRequestNetworkEntityControl(timeoutMs: timeout))
                        {
                            if (requireNetwork)
                            {
                                _log.Error($"Failed to obtain interaction lock for entity {entity.NetworkId}");
                                ResetControl();
                                return;
                            }
                        }
                    }
                    
                    ResetControl();
                    if (action.EntityChangeCallback != null)
                    {
                        //_log.Debug($"Grabbing target change via {action.Name}.");
                        var newTarget = await action.EntityChangeCallback();
                        //_log.Debug($"Switching from target {InteractionTarget?.Handle} > {newTarget?.Handle}");

                        InteractionTarget = newTarget;
                    }

                    if (action.Callback != null)
                    {
                        //_log.Debug($"Invoking callback for action {action.Name}.");
                        await _stateMachine.FireAsync(_triggerAction, InteractionTarget, action);
                        return;
                    }
                }
            }
        }

        bool CanAdsWithWeapon(Weapon weapon)
        {
            return weapon.IsLethal();
        }

        private Task ShowReticleTick()
        {
            Screen.Hud.ShowComponentThisFrame(HudComponent.Reticle);
            return Task.FromResult(0);
        }

        public Task ShowTargetConeTick()
        {
            if (InteractionTarget != null && InteractionTarget.Opacity > 0)
            {
                var dimensions = InteractionTarget.Model.GetDimensions() * InteractionTarget.UpVector;
                var position = InteractionTarget is Ped pedTarget ? 
                    pedTarget.Bones[Bone.SKEL_Spine2].Position
                    : InteractionTarget.Position;
                
                World.DrawMarker(
                    MarkerType.UpsideDownCone, 
                    position + Vector3.Up * dimensions.Length() / 2 + Vector3.Up * 0.2f, 
                    Vector3.Zero, 
                    Vector3.Zero, 
                    Vector3.One * 0.3f, 
                    Color.FromArgb(128, 100, 100, 255));
            }

            return Task.FromResult(0);
        }

        #endregion

        #region IPlayerController Implementation
        public PlayerState State => _stateMachine.State;
        public Ped GrabbedPed => _grabbedPed;

        public void SetControl(bool hasControl, PlayerControlFlag flags = PlayerControlFlag.None)
        {
            API.SetPlayerControl(API.GetPlayerIndex(), hasControl, (int)flags);
        }

        public void ResetControl()
        {
            API.SetPlayerControl(API.PlayerId(), true, 0);
        }

        public async Task GrabPed(Ped ped)
        {
            await _stateMachine.FireAsync(_triggerEscort, ped);
        }

        public async Task UngrabPed()
        {
            if (_stateMachine.IsInState(PlayerState.EscortingPed))
                _stateMachine.Fire(PlayerTriggers.ReleasePed);

            foreach (var ped in World.GetAllPeds())
            {
                if (ped.IsAttachedTo(Game.PlayerPed))
                {
                    if (!await ped.TryRequestNetworkEntityControl())
                    {
                        throw new Exception("Failed to get control of grabbed ped in time!");
                    }
                    
                    ped.Detach();
                }
            }
        }

        #endregion

    }
}
