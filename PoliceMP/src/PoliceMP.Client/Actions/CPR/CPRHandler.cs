using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.NewNotification;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Actions;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Actions.CPR
{
    public class CPRHandler : ActionHandler<CPR>
    {
        private readonly ILogger<CPRHandler> _logger;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ISpeechService _speech;
        private readonly ILegacyClientCommunicationsManager _comms;

        private Random _random;
        
        public CPRHandler(ILogger<CPRHandler> logger, INewNotificationOverlay newNotificationOverlay, ISpeechService speech, ILegacyClientCommunicationsManager comms)
        {
            _logger = logger;
			_newNotificationOverlay = newNotificationOverlay;
			_speech = speech;
            _comms = comms;

            _random = new Random();
        }

        protected override async Task<bool> Handle(CPR action)
        {
            if (!action.SkipSendEvent)
            {
                if (action.Target.IsPlayer)
                {
                    await _comms.Request<bool>(ServerEvents.PlayerCPRPlayer, action.Target.NetworkId);
                    return false;
                }
            }
            
            var doTarget = true;
            var doSubject = true;
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
            
            _logger.Debug($"CPR Action Do subject: {doSubject} do target: {doTarget}");
            API.RequestAnimDict("mini@cpr@char_a@cpr_str");
            while (!API.HasAnimDictLoaded("mini@cpr@char_a@cpr_str"))
            {
                API.RequestAnimDict("mini@cpr@char_a@cpr_str");
                await Delay(0);
            }

            if (doSubject)
            {
                API.TaskGoToEntity(action.Subject.Handle, action.Target.Handle, 1000, 0.1f, 1f, 1073741824, 0);
            }
            
            await Delay(200);
            _speech.Do(action.Subject, "Starting CPR", 1000);
            
            if (doSubject)
            {
                API.TaskPlayAnim(action.Subject.Handle, "mini@cpr@char_a@cpr_str", "cpr_pumpchest", 8.0f, 2.0f, 10000, 1, 2f, false, false, false);
            }
            await Delay(1000);
            
            while (API.IsEntityPlayingAnim(action.Subject.Handle, "mini@cpr@char_a@cpr_str", "cpr_pumpchest", 3))
            {
                await Delay(0);
            }

            if (doTarget)
            {
                if (_random.Next(0, 20) == 1)
                {
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("CPR", "success", "CPR was a success!", new NewNotificationMessageContent[0]));
                    _speech.Do(action.Target, "CPR Successful", 1000);
                    action.Target.Resurrect();    
                
                    while (action.Target != null && action.Target.IsDead)
                    {
                        action.Target.Resurrect();
                        await Delay(10);
                    }

                    await Delay(100);
                
                    var sequence = new TaskSequence();
                    await sequence.AddTask.PlayAnimation("amb@world_human_sunbathe@male@back@base", "base", 8f, -8f, -1, AnimationFlags.Loop, 0f);
                    sequence.Close();
                    action.Target.AlwaysKeepTask = true;
                    action.Target.Task.PerformSequence(sequence);
                }
                else
                {
                    _speech.Do(action.Target, "CPR Unsuccessful", 1000);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("CPR", "error", "CPR was unsuccessful", new NewNotificationMessageContent[0]));
                }
            }
            

            return true;
        }

    }
}