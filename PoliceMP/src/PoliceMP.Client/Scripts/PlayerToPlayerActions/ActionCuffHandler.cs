using System;
using System.Data.SqlTypes;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows.Input;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Behaviors.Cuff;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.NetworkMessages.Game.Notifications;
namespace PoliceMP.Client.Scripts.PlayerToPlayerActions
{
    public class ActionCuffHandler : Script
    {
        /// <summary>
        /// Track whether we are currently cuffing/uncuffing someone, stops the multiple calls to cuff the same ped.
        /// </summary>
        private bool InAnimation = false;
        
        private readonly ILogger<ActionCuffHandler> _logger;
        private readonly IClientCommunicationsManager _comms;
        private readonly INewNotificationOverlay _notifications;
        private readonly ISpeechService _speech;
        private readonly IBehaviorService _behaviour;
        private readonly IPlayerService _playerService;
        private readonly ICommandManager _commandManager;

        public ActionCuffHandler(
            ILogger<ActionCuffHandler> logger,
            IClientCommunicationsManager comms,
            INewNotificationOverlay notifications,
            ISpeechService speech,
            IBehaviorService behaviour,
            IPlayerService playerService,
            ICommandManager commandManager
        ) {
            _logger = logger;
            _comms = comms;
            _notifications = notifications;
            _speech = speech;
            _behaviour = behaviour;
            _playerService = playerService;
            _commandManager = commandManager;

            _comms.AddNotificationHandler<PedCuffedNotification>(PedCuffedHandler);
            _comms.AddNotificationHandler<PedUncuffedNotification>(PedUncuffedHandler);
            
            _commandManager.Register("togglecuffs").WithHandler(ToggleCuffsHandler);
        }

        protected override Task OnStartAsync()
        {
            API.RegisterKeyMapping("togglecuffs", "Toggles getting out or putting away your handcuffs", "keyboard", "u");
            
            return base.OnStartAsync();
        }

        private async Task ToggleCuffsHandler()
        {
            if (!(
                Game.PlayerPed.IsOnFoot
                && !Game.PlayerPed.IsDead
                && API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)API.GetHashKey("weapon_speedcuffs"), false)
            )) return;
            
            uint weaponhash = 0;
            API.GetCurrentPedWeapon(Game.PlayerPed.Handle, ref weaponhash, true);
            API.SetCurrentPedWeapon(
                Game.PlayerPed.Handle, 
                (uint)API.GetHashKey("weapon_speedcuffs") == weaponhash
                    ? (uint)WeaponHash.Unarmed
                    : (uint)API.GetHashKey("weapon_speedcuffs")
                ,
                true
            );
        }

        private async Task PedCuffedHandler(PedCuffedNotification notification)
        {
            _logger.Debug("PedCuffedNotification");
            
            Ped cuffer;
            Ped cuffee;
            try
            {
                cuffer = (Ped)Ped.FromNetworkId(notification.cufferNetworkId);
                cuffee = (Ped)Ped.FromNetworkId(notification.cuffeeNetworkId);
            }
            catch (Exception ex)
            {
                return;
            }

            try
            {
                // Stop cuffing again when we're already cuffing
                if (cuffer.Handle == Game.PlayerPed.Handle && InAnimation) return;
                if (cuffer.Handle == Game.PlayerPed.Handle) InAnimation = true;
                if (cuffer.Handle == Game.PlayerPed.Handle) _logger.Debug("I AM CUFFING!");
                if (cuffee.Handle == Game.PlayerPed.Handle) _logger.Debug("I AM BEING CUFFED!");
            }
            catch (Exception ex) 
            {
                return;
            }
            
            _logger.Debug($"Cuffer: {cuffer.Handle} : Cuffee: {cuffee.Handle}");
            
            if (!cuffer.IsNearEntity(cuffee, new Vector3(2f, 2f, 2f)))
            {
                if (cuffer.Handle == Game.PlayerPed.Handle)
                {
                    _notifications.SendNotification(new NewNotificationMessage("Cuff", "error", "You are not close enough to the ped.", new NewNotificationMessageContent[0]));
                }

                InAnimation = false;
                return;
            }

            if (cuffee.IsCuffed)
            {
                InAnimation = false;
                return;
            }
            
            _speech.Say(cuffer,
                "I am placing you under arrest.",
                //"I am placing you under arrest. You do not have to say anything." +
                //      " But, it may harm your defence if you do not mention when questioned something" +
                //      " which you later rely on in court. Anything you do say may be given in evidence.",
                10000);

            // Stop Cuffee in their tracks
            cuffee.CanRagdoll = false;
            cuffee.Weapons.RemoveAll();
            API.SetEnableHandcuffs(cuffee.Handle, true);
            cuffee.Task.ClearAll();
            while (API.IsPedActiveInScenario(cuffee.Handle))
            {
                await Delay(0);
            }
            
            // Move cuffee into position and attach
            cuffee.Task.AchieveHeading(cuffee.Heading); // Make cuffee face same direction as cuffer
            cuffee.Task.StandStill(-1);
            await Delay(1000);
            cuffee.IsPositionFrozen = true;
            cuffee.AttachTo(cuffer, new Vector3(0f, 0.6f, 0f), Vector3.Zero);
            
            // Setup animation sequences
            var copSequence = new TaskSequence();
            await copSequence.AddTask.PlayAnimation("mp_arrest_paired", "cop_p1_rf_fwd_0", 1.5f, -1.5f, 1500, 0, 0);
            await copSequence.AddTask.PlayAnimation("mp_arrest_paired", "cop_p2_back_right", 1.5f, -1.5f, 1500, 0, 0);
            await copSequence.AddTask.PlayAnimation("mp_arrest_paired", "cop_p3_fwd", 1.5f, -1.5f, 1500, 0, 0);
            copSequence.Close();
            var sequence = new TaskSequence();
            await sequence.AddTask.PlayAnimation("mp_arrest_paired", "crook_p1_idle", 1.5f, -1.5f, 1500, 0, 0);
            await sequence.AddTask.PlayAnimation("mp_arrest_paired", "crook_p2_back_right", 1.5f, -1.5f, 1500, 0, 0);
            await sequence.AddTask.PlayAnimation("mp_arrest_paired", "crook_p3", 1.5f, -1.5f, 1500, 0, 0);
            await sequence.AddTask.PlayAnimation("mp_arresting", "idle", 8f, -8f, -1, AnimationFlags.Loop, 0);
            sequence.Close();

            // Prepare peds to do the animations
            cuffee.AlwaysKeepTask = true;
            cuffee.Task.ClearAllImmediately();
            cuffer.Task.ClearAllImmediately();
            
            // Start animations
            cuffee.Task.PerformSequence(sequence);
            cuffer.Task.PerformSequence(copSequence);
            await Delay(1000);
            _behaviour.RemovePedBehaviors(cuffee);
            _behaviour.SetPedBehavior<CuffedBehaviour>(cuffee);
            
            // Detach the cuffee and allow them to move if they are a player
            cuffee.Detach();
            cuffer.IsPositionFrozen = false;
            cuffer.Task.ClearAllImmediately();
            if (cuffee.IsPlayer)
            {
                cuffee.IsPositionFrozen = false;
                cuffee.Task.ClearAllImmediately();
            }
            cuffee.CanRagdoll = true;
            cuffee.IsCollisionEnabled = true;
            string playerName = _playerService.FetchPlayerNameFromNetworkId(cuffer.NetworkId);
            _speech.Do(cuffee, $"Gets cuffed by {playerName}.");
            
            // Send notifications where relevant
            if (cuffee.Handle == Game.PlayerPed.Handle)
            {
                // We are the cuffee
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have been cuffed!", new NewNotificationMessageContent[0]));
            } else if (cuffer.Handle == Game.PlayerPed.Handle)
            {
                // We are the cuffer
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have cuffed the suspect", new NewNotificationMessageContent[0]));
            }
            
            // @todo Render cuff prop on cuffee
            // int propHandle = API.CreateObject(API.GetHashKey("w_me_speedcuffs"), 0, 0, 0, true, false, false);
            // API.AttachEntityToEntity(
            //     propHandle, 
            //     Game.PlayerPed.Handle, 
            //     API.GetPedBoneIndex(propHandle, 18905), 
            //     -0.03f, -0.07f, -0.03f, -104.4f, -78.56f, 15.49f, true, false, false, true, 1, true
            // );
            
            // Remove cuffs from cuffer
            API.RemoveWeaponFromPed(cuffer.Handle, (uint) API.GetHashKey("weapon_speedcuffs"));
            API.SetCurrentPedWeapon(cuffer.Handle, (uint) API.GetHashKey("weapon_unarmed"), false);

            // All done!
            InAnimation = false;
        }
        
        private async Task PedUncuffedHandler(PedUncuffedNotification notification)
        {
            _logger.Debug("PedUncuffedNotification");

            Ped cuffer;
            Ped cuffee;
            try
            {
                cuffer = (Ped)Ped.FromNetworkId(notification.cufferNetworkId);
                cuffee = (Ped)Ped.FromNetworkId(notification.cuffeeNetworkId);
            }
            catch (Exception ex)
            {
                return;
            }

            try
            {
                // Stop uncuffing again when we're already uncuffing
                if (cuffer.Handle == Game.PlayerPed.Handle && InAnimation) return;
                if (cuffer.Handle == Game.PlayerPed.Handle) InAnimation = true;
                if (cuffer.Handle == Game.PlayerPed.Handle) _logger.Debug("I AM UNCUFFING!");
                if (cuffee.Handle == Game.PlayerPed.Handle) _logger.Debug("I AM BEING UNCUFFED!");
            }
            catch (Exception ex)
            {
                return;
            }

            _logger.Debug($"Cuffer: {cuffer.Handle} : Cuffee: {cuffee.Handle}");
            
            _speech.Say(cuffer, "Looks like it's your lucky day!");
            cuffer.Task.PlayAnimation("mp_arresting", "a_uncuff");
            
            var sequence = new TaskSequence();
            sequence.AddTask.PlayAnimation("mp_arresting", "b_uncuff");
            sequence.AddTask.StandStill(-1);
            sequence.Close();
            cuffee.BlockPermanentEvents = false;
            cuffee.Task.PerformSequence(sequence);
            await Delay(1000);
            _behaviour.RemovePedBehaviors(cuffee);
            API.SetEnableHandcuffs(cuffee.Handle, false);
            API.UncuffPed(cuffee.Handle);
            API.ClearPedAlternateMovementAnim(cuffee.Handle, 0, 8.0f); // idle
            API.ClearPedAlternateMovementAnim(cuffee.Handle, 1, 8.0f); // walk
            API.ClearPedAlternateMovementAnim(cuffee.Handle, 2, 8.0f); // run
            
            cuffee.Detach();
            cuffer.IsPositionFrozen = false;
            cuffer.Task.ClearAllImmediately();
            cuffee.IsPositionFrozen = false;
            cuffee.Task.ClearAllImmediately();
            cuffee.CanRagdoll = true;
            cuffee.IsCollisionEnabled = true;
            _speech.Do(cuffee, "Gets uncuffed.");
            
            // Give the uncuffer back their cuffs
            API.GiveWeaponToPed(cuffer.Handle, (uint) API.GetHashKey("weapon_speedcuffs"), 1, false, false);

            if (cuffer.Handle == Game.PlayerPed.Handle)
            {
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have uncuffed the suspect", new NewNotificationMessageContent[0]));
            } else if (cuffee.Handle == Game.PlayerPed.Handle)
            {
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have been uncuffed!", new NewNotificationMessageContent[0]));
            }

            InAnimation = false;
        }
    }
}