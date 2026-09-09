using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Core.Client.Actions.Interfaces;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Actions.Cuff
{
    /*
    public class Cuff : IAction
    {
        public Ped Arrester { get; }
        public Ped Target { get; }

        public Cuff(Ped arrester, Ped target)
        {
            Arrester = arrester;
            Target = target;
        }
    }
    */
    public class CuffHandler : ActionHandler<Cuff>
    {
        private readonly INewNotificationOverlay _notifications;
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly ISpeechService _speech;
        private readonly ILogger<CuffHandler> _logger;
        private readonly IBehaviorService _behavior;
        private readonly IPermissionService _permissionService;

        public CuffHandler(INewNotificationOverlay notifications,
            ILegacyClientCommunicationsManager comms,
            ISpeechService speech, ILogger<CuffHandler> logger, IBehaviorService behavior, ICommandManager _command, IPermissionService permissionService)
        {
            _notifications = notifications;
            _comms = comms;
            _speech = speech;
            _logger = logger;
            _behavior = behavior;
            _permissionService = permissionService;

            _command.Register("resetcuffs").WithHandler(ResetCuffs);
            _command.Register("spithood").WithHandler(ResetSpitHood);
        }

        private async Task ResetCuffs()
        {
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Police)
            {
                API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("weapon_speedcuffs"), 1, false, false);
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You now have a new set of cuffs.", new NewNotificationMessageContent[0]));
            }
            else
            {
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "error", "Police officers only!", new NewNotificationMessageContent[0]));
            }
        }
        
        private async Task ResetSpitHood()
        {
            if (_permissionService.CurrentUserRole.Branch == UserBranch.Police)
            {
                API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("weapon_spithood"), 1, false, false);
                _notifications.SendNotification(new NewNotificationMessage("Spit Hood", "info", "You now have a new spit hood.", new NewNotificationMessageContent[0]));
            }
            else
            {
                _notifications.SendNotification(new NewNotificationMessage("Spit Hood", "error", "Police officers only!", new NewNotificationMessageContent[0]));
            }
        }

        protected override async Task<bool> Handle(Cuff action)
        {
            _behavior.RemovePedBehaviors(action.Target);
			_logger.Debug($"Handle action: {action}");

			if (!action.SkipSendEvent)
            {
                if (action.Target.IsPlayer)
                {
                    _logger.Debug("Sending PlayerArrestPlayer event");
                    await _comms.Request<bool>(ServerEvents.PlayerArrestPlayer, action.Target.NetworkId);
                    return false;
                }
            }
            
            var doTarget = true;
            var doSubject = true;
			_logger.Debug($"action.Subject.IsPlayer: {action.Subject.IsPlayer}");
			_logger.Debug($"action.Target.IsPlayer: {action.Target.IsPlayer}");
            if (action.Subject.IsPlayer && action.Target.IsPlayer)
            {
                if (action.Subject == Game.PlayerPed)
                {
                    doTarget = false;
                }
                else if (action.Target == Game.PlayerPed)
                {
                    doSubject = false;
                }
                else
                {
                    return false;
                }
            }
            
            if (action.Target == action.Subject)
            {
                _logger.Debug("Target and Subject the same?");
                return false;
            }

            if (doTarget && doSubject)
            {
                if (!await action.Target.TryRequestNetworkEntityControl())
                {
                    _logger.Error("Network Entity Control Failed for CuffHandler");
                    return false;
                }
            }

            if (!action.Subject.IsNearEntity(action.Target, new Vector3(2f, 2f, 2f)))
            {
                _notifications.SendNotification(new NewNotificationMessage("Cuff", "error", "You are not close enough to the ped.", new NewNotificationMessageContent[0]));
                return false;
			}

			_logger.Debug($"Action target: {action.Target.IsCuffed}");

			if (action.Target.IsCuffed)
			{
				_logger.Debug($"Uncuff ped");
				await UncuffPed(action, doTarget, doSubject);
            }
            else
			{
				_logger.Debug($"Cuff ped");
				await CuffPed(action, doTarget, doSubject);
            }

            if (doTarget && action.Target.IsPlayer)
            {
                API.SetPlayerControl(API.PlayerId(), true, 0);
                Game.PlayerPed.Task.ClearAllImmediately();
            }

            if (doSubject && doTarget)
            {
                _comms.ToClient(ClientEvents.PedCuffed, action.Target, action.Target.IsCuffed);
                action.Target.State.Set<bool>(PedStates.IsKneeling, false);
                action.Target.State.Set<bool>(PedStates.IsLyingDown, false);
            }
            
            //action.Target.ReleaseInteractionLock();

            return true;
        }

        private async Task CuffPed(Cuff action, bool doTarget, bool doSubject)
        {
            _logger.Debug($"doTarget: {doTarget} doSubject: {doSubject}");
            if (doSubject)
            {
                _speech.Say(Game.PlayerPed,
                    "I am placing you under arrest.",
                    //"I am placing you under arrest. You do not have to say anything." +
                    //      " But, it may harm your defence if you do not mention when questioned something" +
                    //      " which you later rely on in court. Anything you do say may be given in evidence.",
                    10000);
            }

            if (doTarget)
            {
                action.Target.CanRagdoll = false;

                action.Target.Weapons.RemoveAll();
                API.SetEnableHandcuffs(action.Target.Handle, true);

                action.Target.Task.ClearAll();
                while (API.IsPedActiveInScenario(action.Target.Handle))
                {
                    await Delay(0);
                }
                
                // Remove new cuffs weapon from subject
                API.RemoveWeaponFromPed(action.Subject.Handle, (uint) API.GetHashKey("weapon_speedcuffs"));
            }

            if (doTarget)
            {
                await GetInPosition(action.Target);
                action.Target.AttachTo(action.Subject, new Vector3(0f, 0.6f, 0f), Vector3.Zero);
            }

            if (doSubject)
            {
                action.Subject.IsPositionFrozen = true; 
            }
            
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

            if (doTarget)
            {
                action.Target.AlwaysKeepTask = true;
                action.Target.Task.ClearAllImmediately();
            }

            if (doSubject)
            {
                action.Subject.Task.ClearAllImmediately();
            }

            if (doTarget)
            {
                action.Target.Task.PerformSequence(sequence);
            }

            if (doSubject)
            {
                action.Subject.Task.PerformSequence(copSequence);
            }

            if (doTarget)
            {
                API.SetPedCanPlayGestureAnims(action.Target.Handle, false);
            }

            if (doSubject)
            {
                while (action.Subject.TaskSequenceProgress == -1)
                    await Script.Delay(0);

                while (action.Subject.TaskSequenceProgress != -1)
                    await Script.Delay(0);
            }

            if (doTarget)
            {
                API.SetPedAlternateMovementAnim(
                    action.Target.Handle,
                    0, // idle
                    "mp_arresting",
                    "idle",
                    8.0f,
                    true
                );

                API.SetPedAlternateMovementAnim(
                    action.Target.Handle,
                    1, // walk
                    "mp_arresting",
                    "walk",
                    8.0f,
                    true
                );

                API.SetPedAlternateMovementAnim(
                    action.Target.Handle,
                    2, // running
                    "mp_arresting",
                    "sprint",
                    8.0f,
                    true
                );

                action.Target.Detach();
            }

            if (doSubject)
            {
                action.Subject.IsPositionFrozen = false;
                // _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have been cuffed!", new NewNotificationMessageContent[0]));
            }

            if (doTarget)
            {
                action.Target.CanRagdoll = true;
                action.Target.IsCollisionEnabled = true;
            }

            if (doSubject)
            {
                _speech.Do(action.Target, $"Gets cuffed by {Game.Player.Name}.");
                // _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have cuffed the suspect", new NewNotificationMessageContent[0]));
            }
        }

        private async Task UncuffPed(Cuff action, bool doTarget, bool doSubject)
        {

			_logger.Debug($"Action: {action} doTarget: {doTarget} doSubject: {doSubject}");

			if (doSubject)
            {
                _speech.Say(Game.PlayerPed, "Looks like it's your lucky day!");
                action.Subject.Task.PlayAnimation("mp_arresting", "a_uncuff");
            }

            if (doTarget)
            {
                var sequence = new TaskSequence();
                sequence.AddTask.PlayAnimation("mp_arresting", "b_uncuff");
                sequence.AddTask.StandStill(-1);
                sequence.Close();
                action.Target.BlockPermanentEvents = false;
                action.Target.Task.PerformSequence(sequence);

                //await Delay(3000);

                API.SetEnableHandcuffs(action.Target.Handle, false);
                API.UncuffPed(action.Target.Handle);

                API.ClearPedAlternateMovementAnim(action.Target.Handle, 0, 8.0f); // idle
                API.ClearPedAlternateMovementAnim(action.Target.Handle, 1, 8.0f); // walk
                API.ClearPedAlternateMovementAnim(action.Target.Handle, 2, 8.0f); // run
                
                // Give the uncuffer back their cuffs
                API.GiveWeaponToPed(action.Subject.Handle, (uint) API.GetHashKey("weapon_speedcuffs"), 1, false, false);
                
                // _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have uncuffed the suspect", new NewNotificationMessageContent[0]));
            }
            
            _speech.Do(action.Target, "Gets uncuffed.");
            // _notifications.SendNotification(new NewNotificationMessage("Cuff", "info", "You have been uncuffed!", new NewNotificationMessageContent[0]));
        }

        private async Task GetInPosition(Ped ped)
        {
            // Make ped face the way the player is.
            ped.Task.AchieveHeading(ped.Heading);
            ped.Task.StandStill(-1);

            await Delay(1000);
        }
    }
}