using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using MenuAPI;
using PoliceMP.Client.Actions.AskDriverToStepOut;
using PoliceMP.Client.Actions.Cuff;
using PoliceMP.Client.Actions.Grab;
using PoliceMP.Client.Actions.PutPedInCar;
using PoliceMP.Client.Actions.SearchVehicle;
using PoliceMP.Client.Actions.ToggleSirens;
using PoliceMP.Client.Actions.UseVehicleDoor;
using PoliceMP.Client.Scripts.Boot;
using PoliceMP.Client.Scripts.PedInteractionMenu;
using PoliceMP.Client.Scripts.PlayerControllerScript.Interfaces;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Extensions;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using PoliceMP.Client.Actions.AnalyseBody;
using PoliceMP.Client.Actions.BagBody;
using PoliceMP.Client.Actions.AnalyseEngineTemp;
using PoliceMP.Client.Actions.CPR;
using PoliceMP.Client.Actions.Defib;
using PoliceMP.Client.Actions.HandsUp;
using PoliceMP.Client.Actions.LieDown;
using PoliceMP.Client.Actions.Medic;
using PoliceMP.Client.Actions.TacklePed;
using PoliceMP.Client.Scripts.Dsu;
using PoliceMP.Client.Scripts.Taser;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Enums;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Constants.States;
using PoliceMP.Shared.Models;
using Prop = CitizenFX.Core.Prop;
using Weapon = PoliceMP.Shared.Enums.Weapon;
using PoliceMP.Client.Actions.BikePickup;
using PoliceMP.Client.Behaviors;
using PoliceMP.Client.Scripts.DebugScripts;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Shared.Behaviors.Dog;
using PoliceMP.Shared.Behaviors.Jump;
using System.Drawing;
using PoliceMP.Client.Actions.CollectDNA;
using PoliceMP.Client.Actions.SmashWindows;
using System.Drawing.Text;
using PoliceMP.Client.Scripts.Commands;
using PoliceMP.Client.Actions.SearchPed;
using PoliceMP.Client.Actions.CuffPlayer;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
using PoliceMP.Client.Actions;
using PoliceMP.Shared.Behaviors.AiCallouts;
using HandsUp = PoliceMP.Client.Actions.HandsUp.HandsUp;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    public class ContextualActionsProvider : IContextualActionsProvider
    {
        private Ped PlayerPed => Game.PlayerPed;

        private const uint POLICE_RELATIONSHIP_GROUP = 0xa49e591c;

        private readonly IActionManager _actions;
        private readonly IPedInteractionMenuScript _pedInteractionMenu;
        private readonly IBootSystem _bootSystem;
        private readonly IDebugPedScript _debugPedScript;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ILogger<ContextualActionsProvider> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IGameInputManager _gameInput;
        private readonly IBehaviorService _behaviors;
        private readonly IDogScript _dog;
        private readonly IClientCommunicationsManager _newComms;
        private readonly IPlayerService _playerService;
        private UserAces _userAces;

        public ContextualActionsProvider(IActionManager actions,
            IPedInteractionMenuScript pedInteractionMenu,
            IBootSystem bootSystem,
            IDebugPedScript debugPedScript,
            ILegacyClientCommunicationsManager comms,
            ILogger<ContextualActionsProvider> logger,
            IPermissionService permissionService,
            IGameInputManager gameInput,
            IBehaviorService behaviors,
            IDogScript dog,
            IClientCommunicationsManager newComms,
            IPlayerService playerService
        ){
            _actions = actions;
            _pedInteractionMenu = pedInteractionMenu;
            _bootSystem = bootSystem;
            _debugPedScript = debugPedScript;
            _comms = comms;
            _logger = logger;
            _permissionService = permissionService;
            _gameInput = gameInput;
            _behaviors = behaviors;
            _dog = dog;
            _newComms = newComms;
            _playerService = playerService;
        }

        #region Normal Actions (when in normal state)

        public IList<PlayerAction> GetNormalActions(IPlayerController playerController)
        {
            return new List<PlayerAction>()
            {
                new PlayerAction("DogRecall",
                    "Recall Dog",
                    Control.Enter,
                    async () =>
                    {
                        _dog.FollowOwner();
                    }, () => 
                        Game.PlayerPed.IsOnFoot
                        && !Game.PlayerPed.IsDead
                        && _dog.IsActive()
                        && _dog.GetDog() != null
                        && _dog.GetCurrentState() != DogBehaviorState.FollowOwner,
                    PlayerActionFlags.HoldControl),
            };
        }

        #endregion

        #region Interaction Actions (when holding right click)

        public IList<PlayerAction> GetInteractionActions(IPlayerController playerController, Entity context)
        {
            if (context is Ped ped) return GetPedActions(playerController, ped);
            if (context is Vehicle vehicle) return GetVehicleActions(playerController, vehicle);
            if (context is Prop obj) return GetObjectActions(playerController, obj);

            return new List<PlayerAction>();
        }


        private IList<PlayerAction> GetPedActions(IPlayerController playerController, Ped ped)
        {
            bool CanArrestIfFriendly()
            {
                _userAces ??= _permissionService.GetUserAces().Result;

                return (_userAces.IsDeveloper) &&
                       !API.IsPedDeadOrDying(Game.PlayerPed.Handle, true) &&
                       !Game.PlayerPed.IsInVehicle() &&
                       !Game.PlayerPed.IsCuffed &&
                       !Game.PlayerPed.IsAttachedTo(ped) &&
                       ped.IsHuman &&
                       World.GetDistance(ped.Position, Game.PlayerPed.Position) < 5f &&
                       Game.PlayerPed.Weapons.Current.Hash == (WeaponHash)API.GetHashKey("weapon_speedcuffs");
            }
            
            /*bool CanGrabPlayers()
            {
                _userAces ??= _permissionService.GetUserAces().Result;

                // Only whitelisted people can grab other players (when cuffed)
                return _userAces.IsWhiteListed;
            }*/
            
            return new[]
            {
                new PlayerAction(
                    "SetDebugPedAction",
                    "Set Debug",
                    Control.ContextSecondary,
                    () => _debugPedScript.SetPed(ped),
                    () => _debugPedScript.Enabled
                ),

                new PlayerAction(
                    "DebugJump",
                    "Debug Jump",
                    Control.Jump,
                    () =>
                    {
                        var bb = _behaviors.SetPedBehavior<WanderingRadiusBehaviour>(ped);
                        bb.Set(b => b.Position, PlayerPed.Position.ToPmpVector3());
                    },
                    () => _debugPedScript.Enabled
                ),

                new PlayerAction(
                    "DebugAnkleCuff",
                    "Debug Ankle Cuff",
                    Control.Enter,
                    () =>
                    {
                        API.SetEnableHandcuffs(ped.Handle, true);
                        API.SetEnableBoundAnkles(ped.Handle, true);
                        ped.Ragdoll(-1, RagdollType.NarrowLegs);
                    },
                    () => _debugPedScript.Enabled,
                    PlayerActionFlags.HoldControl
                ),


                new PlayerAction(
                    "TalkAction",
                    "Talk",
                    Control.Talk,
                    async () =>
                    {
                        // Must disable back button until player releases interact button
                        MenuController.DisableBackButton = true;
                        Game.DisableControlThisFrame(0, Control.Aim);

                        while(_gameInput.IsPressed(Control.Aim, InputMode.MouseAndKeyboard))
                        {
                            MenuController.DisableBackButton = true;
                            await Script.Delay(0);
                            Game.DisableControlThisFrame(0, Control.Aim);
                        }

                        await BaseScript.Delay(100);
                        await _pedInteractionMenu.InteractWith(ped);
                        var releaseControlTime = Game.GameTime + 1000;
                        while (Game.GameTime < releaseControlTime)
                        {
                            MenuController.DisableBackButton = true;
                            await BaseScript.Delay(0);
                        }

                        while (_pedInteractionMenu.IsMenuActive())
                        {
                            await Script.Delay(100);
                            MenuController.DisableBackButton = false;
                        }
                    },
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsPlayer
                        && ped.IsHuman,

                    // Can't flee when cuffed
                    ped.IsCuffed ? PlayerActionFlags.None : PlayerActionFlags.PedTargetCanFlee
                ),

                new PlayerAction(
                    "StopAction",
                    "\"Hey, Stop!\"",
                    Control.MeleeAttackLight,
                    () => ped.StopAndLookAt(PlayerPed),
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && !ped.IsCuffed
                        && !ped.IsAttachedTo(Game.PlayerPed)
                        && ped.IsWalking
                        && World.GetDistance(ped.Position, PlayerPed.Position) > 2.0f
                        && ped.IsHuman
                        && !ped.IsPlayer
                        && ped.GetBoolDecor(PedDecors.ALWAYS_FLEE) == false,
                    //&& CanArrestIfFriendly(),
                    PlayerActionFlags.PedTargetCanFlee
                ),

                new PlayerAction(
                    "CuffAction",
                    "Cuff",
                    Control.Reload,
                    async () =>
                    {
                        _newComms.PublishToServer<PedCuffedNotification>(new PedCuffedNotification()
                        {
                            cufferNetworkId = Game.PlayerPed.NetworkId,
                            cuffeeNetworkId = ped.NetworkId
                        });
                        // await _actions.Execute(new Cuff(PlayerPed, ped, false));
                    },
                    () =>
                        CanArrestIfFriendly() ||
                        (
                            _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                            !API.IsPedDeadOrDying(ped.Handle, true) &&
                            !ped.IsInVehicle() &&
                            !ped.IsCuffed &&
                            !(ped.State.Get("isFrontCuffed") ?? false) && // Exclude if front cuffed
                            !ped.IsAttachedTo(Game.PlayerPed) &&
                            ped.IsHuman &&
                            World.GetDistance(ped.Position, PlayerPed.Position) < 5f &&
                            Game.PlayerPed.Weapons.Current.Hash == (WeaponHash)API.GetHashKey("weapon_speedcuffs") &&
                            (
                                _playerService.FetchCahcedPlayerInfoFromNetworkId(ped.NetworkId) == null ||
                                _playerService.FetchCahcedPlayerInfoFromNetworkId(ped.NetworkId).ActiveBranch == UserBranch.Civ
                            )
                        ),
                    PlayerActionFlags.DisablePlayerControls
                ),

                new PlayerAction(
                    "UncuffAction",
                    "Uncuff",
                    Control.Reload,
                    async () =>
                    {
                        _newComms.PublishToServer<PedUncuffedNotification>(new PedUncuffedNotification()
                        {
                            cufferNetworkId = Game.PlayerPed.NetworkId,
                            cuffeeNetworkId = ped.NetworkId
                        });
                        // await _actions.Execute(new Cuff(PlayerPed, ped, false));
                    },
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true) &&
                        !ped.IsInVehicle() &&
                        ped.IsCuffed &&
                        !(ped.State.Get("isFrontCuffed") ?? false) && // Exclude if front cuffed
                        !ped.IsAttachedTo(Game.PlayerPed) &&
                        ped.IsHuman &&
                        World.GetDistance(ped.Position, PlayerPed.Position) < 5f,
                        //&& CanArrestIfFriendly(),
                    PlayerActionFlags.DisablePlayerControls
                ),
                
                new PlayerAction(
                    "FrontCuffAction",
                    "Front Cuff",
                    Control.Enter,
                    async () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var targetPed = ped;

                        if (World.GetDistance(targetPed.Position, playerPed.Position) > 1f)
                        {
                            return;
                        }

                        API.ExecuteCommand("+zfrontcuff");
                        await BaseScript.Delay(3000);
                        API.SetCurrentPedWeapon(playerPed.Handle, (uint)WeaponHash.Unarmed, true);
                        API.RemoveWeaponFromPed(playerPed.Handle, (uint)API.GetHashKey("weapon_speedcuffs"));
                        ped.State.Set("isFrontCuffed", true, true);
                    },
                    () =>
                        !(ped.State.Get("isFrontCuffed") ?? false) &&
                        !ped.IsCuffed && // Exclude if regular cuffed
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true) &&
                        !ped.IsInVehicle() &&
                        !ped.IsAttachedTo(Game.PlayerPed) &&
                        ped.IsHuman &&
                        World.GetDistance(ped.Position, PlayerPed.Position) < 5f &&
                        Game.PlayerPed.Weapons.Current.Hash == (WeaponHash)API.GetHashKey("weapon_speedcuffs") &&
                        (
                            _playerService.FetchCahcedPlayerInfoFromNetworkId(ped.NetworkId) == null ||
                            _playerService.FetchCahcedPlayerInfoFromNetworkId(ped.NetworkId).ActiveBranch == UserBranch.Civ
                        ),
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "FrontUncuffAction",
                    "Front Uncuff",
                    Control.Enter,
                    async () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var targetPed = ped;

                        if (World.GetDistance(targetPed.Position, playerPed.Position) > 1f)
                        {
                            return;
                        }
                        
                        API.ExecuteCommand("+zfrontuncuff");
                        await BaseScript.Delay(3000);
                        var playerPedw = Game.PlayerPed.Handle;
                        API.GiveWeaponToPed(playerPedw, (uint)API.GetHashKey("weapon_speedcuffs"), 1, false, true);
                        ped.State.Set("isFrontCuffed", false, true);
                    },
                    () =>
                        (ped.State.Get("isFrontCuffed") ?? false) &&
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true) &&
                        !ped.IsInVehicle() &&
                        !ped.IsAttachedTo(Game.PlayerPed) &&
                        ped.IsHuman &&
                        World.GetDistance(ped.Position, PlayerPed.Position) < 5f,
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "SearchPlayerAction",
                    "Search",
                    Control.Cover,
                    async () => await _actions.Execute(new SearchPed(PlayerPed, ped)),
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && API.DecorGetInt(ped.Handle, "PoliceMP_Ped_Inventory1") != 0 || API.DecorGetInt(ped.Handle, "PoliceMP_Ped_Inventory2") != 0 || API.DecorGetInt(ped.Handle, "PoliceMP_Ped_Inventory3") != 0 || API.DecorGetInt(ped.Handle, "PoliceMP_Ped_Inventory4") != 0
                        && !ped.IsAttachedTo(Game.PlayerPed)
                        && ped.IsHuman
                        && ped.IsPlayer
                        && World.GetDistance(ped.Position, PlayerPed.Position) < 2.0f
                        && World.GetDistance(ped.Position, PlayerPed.Position) < 5f,
                        //&& CanArrestIfFriendly(),
                    PlayerActionFlags.DisablePlayerControls | PlayerActionFlags.HoldControl
                ),

                new PlayerAction(
                    "GrabAction",
                    "Grab",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        await _actions.Execute(new Grab(ped));
                        await playerController.GrabPed(ped);
                    },
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && ped.IsCuffed
                        && ped.IsHuman
                        && !ped.IsPlayer
                        && !ped.IsAttachedTo(Game.PlayerPed),
                        //&& CanArrestIfFriendly(),
                    PlayerActionFlags.DisablePlayerControls
                ),

                
                new PlayerAction(
                    "GrabPlayerAction",
                    "Grab Player",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        API.ExecuteCommand("+xgrab");
                    },
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && ped.IsCuffed
                        && ped.IsHuman
                        && ped.IsPlayer,
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "AnalyseBody",
                    "Analyse Body",
                    Control.Talk,
                    async () => await _actions.Execute(new AnalyseBody(ped)),
                    () =>
                          _permissionService.CurrentUserRole.Division == UserDivision.Cid
                          && !ped.IsInVehicle()
                          && ped.IsDead
                          && ped.IsHuman
                          && !ped.IsAttached()
                          && !ped.GetBoolDecor(PedStates.AnalyseBodyActionActive),
                    PlayerActionFlags.None),

                /*
                new PlayerAction(
                    "CuffPlayer",
                    "Cuff",
                    Control.Talk,
                    async () => await _actions.Execute(new CuffPlayer(ped)),
                    () =>
                          _permissionService.CurrentUserRole.Branch == UserBranch.Police
                          && !ped.IsInVehicle()
                          && ped.IsDead
                          && ped.IsHuman
                          && !ped.IsAttached()
                          && World.GetDistance(ped.Position, PlayerPed.Position) < 2.0f
                          && ped.IsPlayer
                          && API.DecorGetInt(ped.Handle, "PoliceMP_CuffPlayer_State") == 0,
                    PlayerActionFlags.None),

                new PlayerAction(
                    "UnCuffPlayer",
                    "UnCuff",
                    Control.Talk,
                    async () => await _actions.Execute(new CuffPlayer(ped)),
                    () =>
                          _permissionService.CurrentUserRole.Branch == UserBranch.Police
                          && !ped.IsInVehicle()
                          && ped.IsDead
                          && ped.IsHuman
                          && !ped.IsAttached()
                          && ped.IsPlayer
                          && World.GetDistance(ped.Position, PlayerPed.Position) < 2.0f
                          && API.DecorGetInt(ped.Handle, "PoliceMP_CuffPlayer_State") == 2,
                    PlayerActionFlags.None),
                */

                new PlayerAction(
                    "BagBody",
                    "Bag Body",
                    Control.Cover,
                    async () => await _actions.Execute(new BagBody(ped)),
                    () =>
                          _permissionService.CurrentUserRole.Division == UserDivision.Cid
                          && !ped.IsInVehicle()
                          && ped.IsDead
                          && ped.IsHuman
                          && !ped.IsPlayer
                          && !ped.IsAttached()
                          && !ped.GetBoolDecor(PedStates.BagBodyActionActive),
                    PlayerActionFlags.None),


                new PlayerAction(
                    "CollectDNA",
                    "Collect Forensic Evidence",
                    Control.MpTextChatTeam,
                    async () => await _actions.Execute(new CollectDNA(ped)),
                    () =>
                          _permissionService.CurrentUserRole.Division == UserDivision.Cid
                          && !ped.IsInVehicle()
                          && ped.IsDead
                          && ped.IsHuman
                          && !ped.IsAttached()
                          && !ped.GetBoolDecor(PedStates.CollectDNAActionActive),
                    PlayerActionFlags.None),

                new PlayerAction(
                    "Dismiss",
                    "Dismiss",
                    Control.ContextSecondary,
                    async () =>
                    {
                        _behaviors.RemovePedBehaviors(ped);
                        ped.Task.ClearAll();
                        ped.Stop(false);
                        ped.Task.WanderAround();
                        var handle = ped.Handle;
                        API.SetEntityAsNoLongerNeeded(ref handle);
                    },
                    () =>
                        !ped.IsInVehicle()
                        && !ped.IsDead
                        && ped.IsHuman
                        && !ped.IsAttached(),
                    PlayerActionFlags.None),

                #region Medic Actions
                new PlayerAction(
                    "HealthStatus",
                    "Check Health",
                    Control.Reload,
                    async () =>
                    {
                        API.ExecuteCommand("+helpnpc");
                    },
                    () =>
                        API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && ped.IsHuman
                        && !ped.IsAttachedTo(Game.PlayerPed),
                    PlayerActionFlags.DisablePlayerControls
                ),
                
                new PlayerAction(
                    "CPRAction",
                    "CPR",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        await _actions.Execute(new CPR(ped, PlayerPed, false));
                    },
                    () =>
                        API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && ped.IsHuman
                        && !ped.IsAttachedTo(Game.PlayerPed),
                    PlayerActionFlags.DisablePlayerControls
                ),

                new PlayerAction(
                    "DefibAction",
                    "Defib",
                    Control.VehicleHorn,
                    Control.Enter,
                    async () =>
                    {
                        await _actions.Execute(new Defib(ped, Game.PlayerPed, false));
                    },
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Nhs &&
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint) API.GetHashKey("WEAPON_ECG"), false)
                        && API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && !ped.IsCuffed
                        && ped.IsHuman
                        && !ped.IsAttachedTo(Game.PlayerPed),
                    PlayerActionFlags.DisablePlayerControls
                ),

                new PlayerAction(
                    "MedicAction",
                    "Medic",
                    Control.LookBehind,
                    Control.Reload,
                    async () =>
                    {
                        await _actions.Execute(new Medic(ped));
                    },
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Nhs &&
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint) API.GetHashKey("WEAPON_ALS"), false)
                        && ped.Health < 100
                        && !ped.IsInVehicle()
                        && !ped.IsCuffed
                        && ped.IsHuman
                        && !ped.IsPlayer
                        && !ped.IsAttachedTo(Game.PlayerPed),
                    PlayerActionFlags.DisablePlayerControls
                ),

                #endregion

                #region Ped In Vehicle

                new PlayerAction(
                    "AskToLeaveVehicleAction",
                    "Ask to step out",
                    Control.Enter,
                    async () => await _actions.Execute(new AskPedToStepOut(ped)),
                    () =>
                        _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                        !API.IsPedDeadOrDying(ped.Handle, true)
                          && ped.IsInVehicle()
                          && ped.CurrentVehicle.Speed < 5
                          && !ped.IsPlayer
                          && ped.IsHuman,
                          //&& CanArrestIfFriendly(),

                    // Can't flee when in a popo car
                    ped.CurrentVehicle?.ClassType == VehicleClass.Emergency
                        ? PlayerActionFlags.None
                        : PlayerActionFlags.PedTargetCanFlee
                ),

                // new PlayerAction(
                //     "TargetNextPassenger",
                //     "Next passenger",
                //     Control.MeleeAttackLight,
                //     () =>
                //     {
                //         var driver = ped.CurrentVehicle.Driver;
                //         if (ped == driver && ped.CurrentVehicle.PassengerCount > 0)
                //             return ped.CurrentVehicle.Passengers[0];
                //
                //         var passengers = ped.CurrentVehicle.Passengers;
                //         for (int i = 0; i < passengers.Length; i++)
                //         {
                //             if (passengers[i] == ped)
                //             {
                //                 if (i == passengers.Length - 1)
                //                 {
                //                     if (ped.CurrentVehicle.HasDriver())
                //                         return ped.CurrentVehicle.Driver;
                //
                //                     return ped.CurrentVehicle.Passengers[0];
                //                 }
                //
                //                 return ped.CurrentVehicle.Passengers[i + 1];
                //             }
                //         }
                //
                //         return ped;
                //     },
                //     () =>
                //         _permissionService.CurrentUserRole.Branch == UserBranch.Police &&
                //         ped.IsInVehicle()
                //           && ped.IsHuman
                //           && !ped.IsPlayer
                //           && ped.CurrentVehicle.PassengerCount > 0
                // ),

                // new PlayerAction(
                //     "SwapInteractAction",
                //     "Interact with Vehicle",
                //     Control.SelectWeapon,
                //     Control.Cover,
                //     () => ped.CurrentVehicle,
                //     () => ped.CurrentVehicle != null
                //           && !ped.IsPlayer
                //           && ped.IsHuman
                // ),

                #endregion
                
                #region AFO CIV DRAG THING

                new PlayerAction("DragPlayer",
                    "Drag Player",
                    Control.Talk,
                    () =>
                    {
                        BaseScript.TriggerServerEvent("pmp_DragPeople:checkDragPermission", "", 0);
                    },
                    () =>
                        !ped.IsAttached()
                        && ped.IsPlayer
                        && ped.IsHuman
                        && _permissionService.CurrentUserRole.Branch == UserBranch.Civ || _permissionService.CurrentUserRole.Division == UserDivision.Afo),

                #endregion

                #region Dog Shit

                new PlayerAction("DogSniff",
                    "<Dog Sniff>",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        _dog.Sniff(ped);
                    },
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsAttached()
                        && !ped.IsPlayer
                        && _dog.IsActive()
                        && _dog.GetActiveDogOptions().CanSniffTarget
                        && ped != _dog.GetDog()),
                
                new PlayerAction("DogRecall",
                    "Recall Dog",
                    Control.Enter,
                    async () =>
                    {
                        _dog.FollowOwner();
                    },
                    () => 
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && _dog.IsActive()
                        && ped == _dog.GetDog(),
                    PlayerActionFlags.AllowFarAway),
                
                new PlayerAction("DogStay",
                    "Command to Stay",
                    Control.Jump,
                    Control.Sprint,
                    async () =>
                    {
                        _dog.Wait();
                    },
                    () => 
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && _dog.IsActive()
                        && ped == _dog.GetDog(),
                    PlayerActionFlags.AllowFarAway),

                new PlayerAction("DogPickup",
                    "Pickup Dog",
                    Control.MeleeAttackLight,
                    async () =>
                    {
                        _dog.Pickup();
                    },
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && _dog.IsActive()
                        && ped == _dog.GetDog(),
                    PlayerActionFlags.AllowFarAway),

                new PlayerAction("DogTakeDown",
                    "<Dog Chase>",
                    Control.MeleeAttackLight,
                    async () =>
                    {
                        _dog.TakeDownTarget(ped);
                    },
                    () => 
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        //&& (!ped.IsPlayer || _permissionService.GetUserRole(ped.NetworkId).Branch == UserBranch.Civ) 
                        && _dog.IsActive()
                        && _dog.GetActiveDogOptions().CanTakeDownTarget
                        && ped != _dog.GetDog(),
                    PlayerActionFlags.AllowFarAway | PlayerActionFlags.HoldControl),

                //new PlayerAction("PatDog",
                //    $"Pat {DsuNaming.FetchDogName()}",
                //    Control.Enter,
                //    Control.Jump,
                //    async () =>
                //    {
                //        var middlePosition = Game.PlayerPed.Position + ped.Position - Game.PlayerPed.Position;
                //        API.GetGroundZFor_3dCoord(middlePosition.X, middlePosition.Y, middlePosition.Z,
                //            ref middlePosition.Z, true);

                //        var scene = new NetworkSynchronizedScene(middlePosition, Game.PlayerPed.Rotation);
                //        scene.AddPedToScene(Game.PlayerPed, "creatures@rottweiler@tricks@", "petting_franklin", 8f, -8f,
                //            -1, 8, 0f);
                //        scene.AddPedToScene(ped, "creatures@rottweiler@tricks@", "petting_chop", 8f, -8f, -1, 8, 0f);

                //        _dog.StopSitting();

                //        API.TaskTurnPedToFaceEntity(Game.PlayerPed.Handle, ped.Handle, 0);
                //        API.TaskTurnPedToFaceEntity(ped.Handle, Game.PlayerPed.Handle, 0);

                //        await scene.Start(false, faceAnimationStart: true);

                //        if (_dog.FetchDogPed() == null) return;

                //        await Script.Delay(500);
                //        await _dog.PlayDogSound(DogSound.Playful);
                //    },
                //    () =>
                //        !API.IsPedDeadOrDying(ped.Handle, true)
                //        && !ped.IsAttached()
                //        && !ped.IsPlayer
                //        && _dog.FetchDogPed() != null
                //        && _dog.FetchDogPed() == ped),

                //new PlayerAction("DogPee",
                //    $"Urinate Dog",
                //    Control.MeleeAttackHeavy,
                //    async () =>
                //    {
                //        API.ClearPedTasks(ped.Handle);

                //        using var sequence = new TaskSequence();
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "pee_right_enter", 8f, -8f,
                //            -1, (AnimationFlags) 0, 0f);
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "pee_right_idle", 8f, -8f, -1,
                //            (AnimationFlags) 0, 0f);
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "pee_right_exit", 8f, -8f, -1,
                //            (AnimationFlags) 0, 0f);
                //        sequence.Close();

                //        ped.Task.PerformSequence(sequence);
                //        await _dog.PlayDogSound(DogSound.Bark);

                //        await Script.Delay(7000);

                //        _comms.ToServer(ServerEvents.SendDogParticleFxEventToServer, ped.NetworkId, DogFx.Pee);
                //    },
                //    () =>
                //        !API.IsPedDeadOrDying(ped.Handle, true)
                //        && !ped.IsAttached()
                //        && !ped.IsPlayer
                //        && _dog.FetchDogPed() != null
                //        && _dog.FetchDogPed() == ped),

                //new PlayerAction("DogPoo",
                //    $"Doggie Do",
                //    Control.MeleeAttackLight,
                //    async () =>
                //    {
                //        API.ClearPedTasks(ped.Handle);

                //        using var sequence = new TaskSequence();
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "dump_enter", 8f, -8f, -1,
                //            (AnimationFlags) 4096, 0f);
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "dump_loop", 8f, -8f, -1,
                //            (AnimationFlags) 4096, 0f);
                //        await sequence.AddTask.PlayAnimation("creatures@rottweiler@move", "dump_exit", 8f, -8f, -1,
                //            (AnimationFlags) 4096, 0f);
                //        sequence.Close();

                //        ped.Task.PerformSequence(sequence);
                //        await _dog.PlayDogSound(DogSound.Agitated);

                //        await Script.Delay(4000);

                //        _comms.ToServer(ServerEvents.SendDogParticleFxEventToServer, ped.NetworkId, DogFx.Poo);
                //    },
                //    () =>
                //        !API.IsPedDeadOrDying(ped.Handle, true)
                //        && !ped.IsAttached()
                //        && !ped.IsPlayer
                //        && _dog.FetchDogPed() != null
                //        && _dog.FetchDogPed() == ped),

                #endregion
            };
        }

        private IList<PlayerAction> GetVehicleActions(IPlayerController playerController, Vehicle vehicle)
        {
            return new[]
            {
                #region Civilian Vehicles

                //new PlayerAction(
                //    "DogSearchVehicleAction",
                //    $"Search with {DsuNaming.FetchDogName()}",
                //    Control.Detonate,
                //    async () =>
                //    {
                //        var doorIndex = PlayerPed.GetVehicleDoorIsLookingAt(vehicle);
                //        if (doorIndex != null)
                //        {
                //            var vehicleDoor = (VehicleDoorIndex) doorIndex;
                //            await _dog.SearchVehicle(vehicle, vehicleDoor);
                //        }
                //    },
                //    () => DogScript.DogSpawned
                //          && vehicle.ClassType != VehicleClass.Emergency
                //          && DogScript.inVehicle == null
                //          && DsuNaming.FetchDogName() != "none"
                //          && PlayerPed.GetVehicleDoorIsLookingAt(vehicle) != null
                //),
                
                new PlayerAction(
                    "SearchVehicleAction",
                    "Search Vehicle",
                    Control.MeleeAttackLight,
                    Control.Jump,
                    async () => await _actions.Execute(new SearchVehicle(vehicle)),
                    () => playerController.State != PlayerState.EscortingPed
                          && vehicle.ClassType != VehicleClass.Emergency
                          && vehicle.PassengerCount == 0
                          && !vehicle.HasDriver(),
                    PlayerActionFlags.DisablePlayerControls
                ),

                new PlayerAction(
                    "RequestTowTruck",
                    "Request Tow",
                    Control.Context,
                    () => API.ExecuteCommand($"towtruck {vehicle.Handle}"),
                    () => playerController.State != PlayerState.EscortingPed
                          && vehicle.ClassType != VehicleClass.Emergency
                          && vehicle.PassengerCount == 0
                          && !vehicle.HasDriver()
                ),

                #endregion

                #region Emergency Vehicles
                
                new PlayerAction(
                    "ToggleSirensAction",
                    "Toggle Sirens",
                    Control.Cover,
                    Control.Context,
                    async () => await _actions.Execute(new ToggleSirens(PlayerPed, vehicle)),
                    () => !vehicle.IsOnFire
                          && !vehicle.IsDead
                          && vehicle.Speed < 0.1f
                          && vehicle.ClassType == VehicleClass.Emergency
                          && !vehicle.HasDriver(),
                    PlayerActionFlags.DisablePlayerControls
                ),

                new PlayerAction(
                    "AccessBootAction",
                    "Access Boot",
                    Control.VehicleDuck,
                    Control.Jump,
                    async () =>
                    {
                        // Only close the boot if this action was the one to open it
                        bool openedBoot = false;
                        if (!vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
                        {
                            await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk));
                            openedBoot = true;
                        }

                        while(_gameInput.IsPressed(Control.Aim))
                        {
                            MenuController.DisableBackButton = true;
                            await Script.Delay(0);
                            Game.DisableControlThisFrame(0, Control.Aim);
                        }

                        await Script.Delay(200); //Wait half a second to allow boot menu to open nicely
                        
                        _bootSystem.OpenMenu(vehicle);

                        while (_bootSystem.IsMenuActive())
                            await Script.Delay(100);

                        if (openedBoot && vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
                        {
                            await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk));
                        }
                    },
                    () =>
                        vehicle.ClassType == VehicleClass.Emergency &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk,
                    PlayerActionFlags.DisablePlayerControls
                    | PlayerActionFlags.IgnoreNetworkTakeoverFail
                    | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                
                //This is a boot action for any vehicle that don't have a boot.
                new PlayerAction(
                    "AccessBootActionNoBoot",
                    "Access Boot",
                    Control.VehicleDuck,
                    Control.Jump,
                    async () =>
                    {
                        while(_gameInput.IsPressed(Control.Aim))
                        {
                            MenuController.DisableBackButton = true;
                            await Script.Delay(0);
                            Game.DisableControlThisFrame(0, Control.Aim);
                        }

                        _bootSystem.OpenMenu(vehicle);

                        while (_bootSystem.IsMenuActive())
                            await Script.Delay(100);
                    },
                    () =>
                        vehicle.ClassType == VehicleClass.Emergency &&
                        !vehicle.Bones.HasBone("boot"),
                    PlayerActionFlags.DisablePlayerControls
                    | PlayerActionFlags.IgnoreNetworkTakeoverFail
                    | PlayerActionFlags.NoWaitForNetworkTimeout
                ),

                //new PlayerAction(
                //    "ReleaseDogAction",
                //    $"Fast Deploy German Shepard {DsuNaming.FetchDogName()}",
                //    Control.MeleeAttackLight,
                //    async () =>
                //    {
                //        //await _dog.TakeFromVehicle();
                //        bool openedBoot = false;
                //        if (!vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
                //        {
                //            await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk));
                //            await Script.Delay(800);

                //            openedBoot = true;
                //        }

                //        await _dog.Deploy();

                //        if (openedBoot && vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
                //        {
                //            await Script.Delay(400);

                //            if (vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
                //                await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk));
                //        }
                //    },
                //    () => _bootSystem.FetchDsuStatus()
                //          && _dog.HasTakenFromKennel()
                //          //&& Dog.inVehicle == vehicle
                //          && !DogScript.DogSpawned
                //          && DsuNaming.FetchDogName() != "none"
                //          && PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk,
                //    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                //),

//                new PlayerAction(
//                    "ReturnDogAction",
//                    $"Return German Shepard {DsuNaming.FetchDogName()}",
//                    Control.MeleeAttackLight,
//                    async () =>
//                    {
//                        //await _dog.PlaceInVehicle(vehicle);
//                        await _dog.ReturnDog();
///*
//                        if (openedBoot && vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
//                        {
//                            await Script.Delay(400);

//                            if (vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
//                                await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk));
//                        }*/
//                    },
//                    () => _bootSystem.FetchDsuStatus()
//                          && DogScript.DogSpawned
//                          //&& Dog.inVehicle == null
//                          && DsuNaming.FetchDogName() != "none"
//                          && PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk
//                ),
                
                

               
                
                // new PlayerAction(
                //     "SwapInteractAction",
                //     $"Interact with {(vehicle.HasDriver() ? "Driver" : "Passenger")}",
                //     Control.SelectWeapon,
                //     Control.Cover,
                //     () => vehicle.Driver ?? vehicle.Passengers.First(p => API.IsEntityAPed(p.Handle)),
                //     () => vehicle.HasDriver() || vehicle.PassengerCount > 0
                // ),

                #endregion

                #region All Vehicles


                new PlayerAction(
                    "AnalyseEngineTemp",
                    "Analyse Engine Temperature",
                    Control.LookBehind,
                    async () => await _actions.Execute(new AnalyseEngineTemp(vehicle)),
                    () =>
                        _permissionService.CurrentUserRole.Division == UserDivision.Cid &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Hood,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),

                new PlayerAction(
                    "SmashFrontLeftWindow",
                    "Smash Window",
                    Control.VehicleDuck,
                Control.ContextSecondary,
                    async () => await _actions.Execute(new SmashWindows(vehicle, VehicleDoorIndex.FrontLeftDoor)),
                    () =>
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_EmgHammer"), false) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),


                new PlayerAction(
                    "SmashBackLeftWindow",
                    "Smash Window",
                    Control.VehicleDuck,
                Control.ContextSecondary,
                    async () => await _actions.Execute(new SmashWindows(vehicle, VehicleDoorIndex.BackLeftDoor)),
                    () =>
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_EmgHammer"), false) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),

                new PlayerAction(
                    "SmashFrontRightWindow",
                    "Smash Window",
                    Control.VehicleDuck,
                Control.ContextSecondary,
                    async () => await _actions.Execute(new SmashWindows(vehicle, VehicleDoorIndex.FrontRightDoor)),
                    () =>
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_EmgHammer"), false) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),


                new PlayerAction(
                    "SmashBackRightWindow",
                    "Smash Window",
                    Control.VehicleDuck,
                Control.ContextSecondary,
                    async () => await _actions.Execute(new SmashWindows(vehicle, VehicleDoorIndex.BackRightDoor)),
                    () =>
                        API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_EmgHammer"), false) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),


                new PlayerAction(
                    "OpenBackLeftDoorAction",
                    "Open Back Left Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.BackLeftDoor)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.BackLeftDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "GetInBackLeftSeatAction",
                    "Get In Back Left Seat",
                    Control.Enter,
                    async () => API.TaskEnterVehicle(PlayerPed.Handle, vehicle.Handle, 1000, 1, 1f, 1, 0),
                    () =>
                        API.IsVehicleSeatFree(vehicle.Handle, 1) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "OpenBackRightDoorAction",
                    "Open Back Right Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.BackRightDoor)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.BackRightDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "GetInBackRightSeatAction",
                    "Get In Back Right Seat",
                    Control.Enter,
                    async () => API.TaskEnterVehicle(PlayerPed.Handle, vehicle.Handle, 1000, 2, 1f, 1, 0),
                    () =>
                        API.IsVehicleSeatFree(vehicle.Handle, 2) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "OpenFrontRightDoorAction",
                    "Open Front Right Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.FrontRightDoor)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.FrontRightDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "GetInFrontRightSeatAction",
                    "Get In Front Right Seat",
                    Control.Enter,
                    async () => API.TaskEnterVehicle(PlayerPed.Handle, vehicle.Handle, 1000, 0, 1f, 1, 0),
                    () =>
                        API.IsVehicleSeatFree(vehicle.Handle, 0) &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "OpenFrontLeftDoorAction",
                    "Open Front Left Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.FrontLeftDoor)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.FrontLeftDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "OpenBootAction",
                    "Open Boot",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "GetInBootAction",
                    "In Boot",
                    Control.Enter,
                    async () => await _actions.Execute(new BootEnter(vehicle)),
                    () =>
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "OpenBonnetAction",
                    "Open Bonnet",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Hood)),
                    () =>
                        !vehicle.Doors[VehicleDoorIndex.Hood].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Hood,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),

                new PlayerAction(
                    "CloseBackLeftDoorAction",
                    "Close Back Left Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.BackLeftDoor)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.BackLeftDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "CloseBackRightDoorAction",
                    "Close Back Right Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.BackRightDoor)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.BackRightDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.BackRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "CloseFrontRightDoorAction",
                    "Close Front Right Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.FrontRightDoor)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.FrontRightDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontRightDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "CloseFrontLeftDoorAction",
                    "Close Front Left Door",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.FrontLeftDoor)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.FrontLeftDoor].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.FrontLeftDoor,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "CloseBootAction",
                    "Close Boot",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Trunk)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Trunk,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "CloseBonnetAction",
                    "Close Bonnet",
                    Control.MultiplayerInfo,
                    Control.ContextSecondary,
                    async () => await _actions.Execute(new UseVehicleDoor(vehicle, VehicleDoorIndex.Hood)),
                    () =>
                        vehicle.Doors[VehicleDoorIndex.Hood].IsOpen &&
                        PlayerPed.GetVehicleDoorIsLookingAt(vehicle) == VehicleDoorIndex.Hood,
                    PlayerActionFlags.IgnoreNetworkTakeoverFail | PlayerActionFlags.NoWaitForNetworkTimeout
                ),
                new PlayerAction(
                    "PickUpBikeAction",
                    "Pick up Bike",
                    Control.Enter,
                    Control.Enter,
                    async () => await _actions.Execute(new BikePickup(Game.PlayerPed, vehicle)),
                    () =>
                        vehicle.ClassType == VehicleClass.Cycles
                ),
                
                new PlayerAction("DogSniff",
                    "<Dog Sniff>",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        _dog.Sniff(vehicle);
                    },
                    () => 
                        !vehicle.HasDriver()
                        && vehicle.PassengerCount == 0
                        && vehicle.Speed < 0.3f
                        && _dog.IsActive()
                        && _dog.GetActiveDogOptions().CanSniffTarget
                ),
                
                new PlayerAction(
                    "LayOnStretcher",
                    "Lay On Stretcher",
                    Control.Enter,
                    () => API.ExecuteCommand("laybed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "SitStretcher",
                    "Sit Stretcher",
                    Control.Cover,
                    () => API.ExecuteCommand("sitbed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "ToggleStretcher",
                    "Toggle Stretcher",
                    Control.MultiplayerInfo,
                    () => API.ExecuteCommand("togglebed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "GetOffStretcher",
                    "Get Off Stretcher",
                    Control.VehicleDuck,
                    () => API.ExecuteCommand("outbed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "StrapHelimed",
                    "Strap to Helimed",
                    Control.ReplayScreenshot,
                    () => API.ExecuteCommand("+inhelimed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "UnStrapHelimed",
                    "Unstrap From Helimed",
                    Control.ReplayShowhotkey,
                    () => API.ExecuteCommand("+outhelimed"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "DeleteStretcher",
                    "Delete Stretcher",
                    Control.InteractionMenu,
                    () => API.ExecuteCommand("delstretcher"),
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;
                
                        // Get the hash key for the stretcher 
                        int stretcherHash = API.GetHashKey("stretcher");

                        // Find the closest vehicle and check if it's the stretcher
                        int closestVehicle = API.GetClosestVehicle(playerPosition.X, playerPosition.Y, playerPosition.Z, 3.0f, 0, 70);
                        return API.DoesEntityExist(closestVehicle) && API.GetEntityModel(closestVehicle) == stretcherHash;
                    },
                    PlayerActionFlags.None
                ),
                new PlayerAction(
                    "StretcherHelimed",
                    "Take Stretcher",
                    Control.InteractionMenu,
                    () => API.ExecuteCommand("+spstretcher"),
                    () => 
                        (_permissionService.CurrentUserRole.Division == UserDivision.Hems || 
                         _permissionService.CurrentUserRole.Division == UserDivision.HemsDoctor) 
                        && (vehicle.Model == API.GetHashKey("GLAAA") || vehicle.Model == API.GetHashKey("GLAAB"))
                        && vehicle.ClassType == VehicleClass.Helicopters
                        && vehicle.PassengerCount == 0
                        && !vehicle.HasDriver(),
                    
                    PlayerActionFlags.None
                ),

                
                #endregion
            };
        }

        private IList<PlayerAction> GetObjectActions(IPlayerController playerController, Prop obj)
        {
            return new[]
            {
                #region Object Interactions
                
                new PlayerAction(
                    "ObjectAction",
                    $"Object Interaction Test",
                    Control.Detonate,
                    async () =>
                    {
                        _logger.Debug("Wow look at dis ting");
                    },
                    () => obj.Model.GetHashCode() == 167557869

                ),
                
                new PlayerAction(
                    "MOE",
                    "MOE Enforcer",
                    Control.Detonate,
                    async () =>
                    {
                        if (Game.PlayerPed.Weapons.Current.Hash == WeaponHash.GolfClub)
                        {
                            API.ExecuteCommand("moe");
                        }
                        else
                        {
                            Debug.WriteLine("Player is not equipped with an enforcer.");
                        }
                    },
                    () => Game.PlayerPed.Weapons.Current.Hash == WeaponHash.GolfClub,
                    PlayerActionFlags.None
                ),
                
                new PlayerAction(
                    "PickBodyBag",
                    "Pick Up BodyBag",
                    Control.Reload,
                    () => 
                    {
                        API.ExecuteCommand("+pickbodybag");
                    },
                    () =>
                    {
                        var playerPed = Game.PlayerPed;
                        var playerPosition = playerPed.Position;

                        int bodyBagHash = API.GetHashKey("xm_prop_body_bag");

                        // Find the closest object (prop) and check if it's the bodybag
                        int closestObject = API.GetClosestObjectOfType(
                            playerPosition.X, 
                            playerPosition.Y, 
                            playerPosition.Z, 
                            3.0f,
                            (uint)bodyBagHash, 
                            false,
                            false,
                            false
                        );

                        return API.DoesEntityExist(closestObject) && API.GetEntityModel(closestObject) == bodyBagHash;
                    },
                    PlayerActionFlags.None
                ),
                
                #endregion
            };
        }

        #endregion

        #region Attached Actions (when an entity is attached to the player)

        public IList<PlayerAction> GetAttachedActions(IPlayerController playerController, Entity context, Entity adsContext)
        {
            var actions = new List<PlayerAction>();
            if (context is Ped ped)
            {
                actions.AddRange(GetAttachedPedActions(playerController, ped));

                if (adsContext is Vehicle adsVehicle)
                    actions.AddRange(GetAttachedAdsVehicleActions(playerController, ped, adsVehicle));
            }

            return actions;
        }

        private IList<PlayerAction> GetAttachedPedActions(IPlayerController playerController, Ped ped)
        {
            var closestVehicle = GetClosestVehicleInRange(5f);

            return new[]
            {
                new PlayerAction(
                    "UngrabAction",
                    "Release from grab",
                    Control.Enter,
                    Control.Jump,
                    async () =>
                    {
                        await _actions.Execute(new Grab(ped, true));
                        await playerController.UngrabPed();
                    },
                    () => !API.IsPedDeadOrDying(ped.Handle, true),
                    PlayerActionFlags.DisablePlayerControls
                )
            };
        }


        private IList<PlayerAction> GetAttachedAdsVehicleActions(IPlayerController playerController, Ped ped, Vehicle adsContext)
        {
            return new[]
            {
                new PlayerAction(
                    "PlaceInVehicleAction",
                    "Place In Vehicle",
                    Control.Reload,
                    Control.Jump,
                    async () => await _actions.Execute(new PutPedInCar(PlayerPed, playerController.GrabbedPed,
                        adsContext)),
                    () => playerController.GrabbedPed != null
                          && !adsContext.IsOnFire
                          && !adsContext.IsDead
                          && adsContext.Speed < 0.1f
                          && adsContext.ClassType == VehicleClass.Emergency
                          && adsContext.HasFreeSlotForPrisoner(),
                    PlayerActionFlags.DisablePlayerControls | PlayerActionFlags.IgnoreNetworkTakeoverFail
                ),
            };
        }

        #endregion

        #region Aim Down Sight Actions

        public IList<PlayerAction> GetAimDownSightActions(IPlayerController playerController, Entity context)
        {
            if (context is Ped ped)
                return GetAimDownSightPedActions(playerController, ped);

            return new List<PlayerAction>();
        }

        private IList<PlayerAction> GetAimDownSightPedActions(IPlayerController controller, Ped ped)
        {
            return new[]
            {
                new PlayerAction(
                    "AdsStopAction",
                    LieDownHandler.IsPedOnGround(ped) ? "\"GET UP!\"" : "\"HANDS UP!\"",
                    Control.Talk,
                    async () => await _actions.Execute(new HandsUp(ped)),
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && !ped.IsCuffed
                        && !ped.IsPlayer
                        && Game.PlayerPed.Weapons.Current.IsLethal()
                        && Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed
                        && !HandsUpHandler.DoesPedHaveHandsUp(ped)
                ),

                new PlayerAction(
                    "AdsGetOnGroundAction",
                    "\"ON THE GROUND!\"",
                    Control.Talk,
                    async () => await _actions.Execute(new LieDown(ped)),
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && !ped.IsInVehicle()
                        && !ped.IsCuffed
                        && !ped.IsPlayer
                        && Game.PlayerPed.Weapons.Current.IsLethal()
                        && HandsUpHandler.DoesPedHaveHandsUp(ped)
                ),

                new PlayerAction(
                    "AdsGetOutVehicleAction",
                    "\"OUT! NOW!\"",
                    Control.Talk,
                    async () => await _actions.Execute(new AskPedToStepOut(ped, handsUp: true, aggressive: true)),
                    () =>
                        !API.IsPedDeadOrDying(ped.Handle, true)
                        && Game.PlayerPed.Weapons.Current.IsLethal()
                        && !ped.IsPlayer
                        && ped.IsInVehicle()
                ),
            };
        }

        #endregion

        #region Helpers
        Vehicle GetClosestVehicleInRange(float range)
        {
            var vehicles = World.GetAllVehicles();
            Vehicle closestVehicle = null;
            float closestVehicleDistance = float.MaxValue;

            for (int i = 0; i < vehicles.Length; i++)
            {
                var v = vehicles[i];
                if (API.IsAnEntity(v.Handle)
                    && v.ClassType == VehicleClass.Emergency)
                {
                    var distance = World.GetDistance(Game.PlayerPed.Position, v.Position);

                    if (distance < range && distance < closestVehicleDistance)
                    {
                        closestVehicle = v;
                        closestVehicleDistance = distance;
                    }
                }
            }

            return closestVehicle;
        }
        #endregion
    }
}