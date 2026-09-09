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

namespace PoliceMP.Client.Actions.Defib
{
    public class DefibHandler : ActionHandler<Defib>
    {
        private readonly ILogger<DefibHandler> _logger;
		private readonly INewNotificationOverlay _newNotificationOverlay;
		private readonly ISpeechService _speech;
        private readonly ICommonFunctionsService _common;
        private readonly ISoundService _sound;
        private readonly ILegacyClientCommunicationsManager _comms;

        private Random _random;
        
        public DefibHandler(ILogger<DefibHandler> logger, INewNotificationOverlay newNotificationOverlay, ISpeechService speech, ICommonFunctionsService common, ISoundService sound, ILegacyClientCommunicationsManager comms)
		{
            _logger = logger;
			_newNotificationOverlay = newNotificationOverlay;
			_speech = speech;
            _common = common;
            _sound = sound;
            _comms = comms;

            _random = new Random();
        }

        private const string DEFIB_MODEL = "w_am_ecg";
        protected override async Task<bool> Handle(Defib action)
        {
            if (!action.SkipSendEvent)
            {
                if (action.Target.IsPlayer)
                {
                    await _comms.Request<bool>(ServerEvents.PlayerDefibPlayer, action.Target.NetworkId);
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
            
            
            API.RequestAnimDict("rcmextreme3");
            while (!API.HasAnimDictLoaded("rcmextreme3"))
            {
                API.RequestAnimDict("rcmextreme3");
                await Delay(0);
            }

            await _common.RequestModelToBeLoaded(DEFIB_MODEL);

            Prop defib = null;
            if (doSubject)
            {
                API.TaskGoToEntity(Game.PlayerPed.Handle, action.Target.Handle, 1000, 0.1f, 1f, 1073741824, 0);
                await Delay(200);

                API.TaskPlayAnim(Game.PlayerPed.Handle, "rcmextreme3", "idle", 8.0f, 2.0f, 10000, 1, 2f, false, false, false);
                await Delay(1000);

                var defibPos = Game.PlayerPed.Position;
                defibPos.X += 1f;
                defibPos.Y += 1f;
                defib = await World.CreateProp(new Model(API.GetHashKey(DEFIB_MODEL)), defibPos,
                    Game.PlayerPed.Rotation, false, true);
                defib.SetNoCollision(action.Target, true);
            
                API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"));
                Game.PlayerPed.Weapons.Select((WeaponHash)API.GetHashKey("WEAPON_UNARMED"));
            
                _sound.Play("DefibBeeps.ogg");
            
                while (API.IsEntityPlayingAnim(Game.PlayerPed.Handle, "rcmextreme3", "idle", 3))
                {
                    await Delay(0);
                }
            }

            if (doTarget)
            {
                if (_random.Next(0, 3) == 1)
                {
                    _speech.Do(action.Target, "Defib Unsuccessful", 1000);
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Defib", "error", "Defib was unsuccessful", new NewNotificationMessageContent[0]));
                }
                else
                {
                    _sound.Play("DefibNoShockAdvised.ogg");
                    _newNotificationOverlay.SendNotification(new NewNotificationMessage("Defib", "success", "Defib was a success!", new NewNotificationMessageContent[0]));
                    _speech.Do(action.Target, "Defib Successful", 1000);
                    action.Target.Resurrect();

                    while (action.Target.IsDead)
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
            }

            if (doSubject)
            {
                defib.Delete();
                API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("WEAPON_ECG"), 1000, false, true);
            }
            
            return true;
        }

    }
}