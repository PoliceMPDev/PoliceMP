using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.Police;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires

//@todo Need to make it so all the peds can be asked questions and sort questions
{
    public class FireServiceInspectionCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int fireinspectorid;


        private Vector3 _calloutLocation;

        public FireServiceInspectionCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Fire Inspection";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "A new request for a fire inspection has come in.";
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.FireInspectors.Count);
            Vector3 fireinspectorlocations = AiCalloutsLocations.ReceptionLocations.GetRandom();
            _calloutLocation = fireinspectorlocations;
            fireinspectorid = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                fireinspectorlocations.X,
                fireinspectorlocations.Y,
                fireinspectorlocations.Z,
                0,
                true,
                false);
        }

        public override bool CanBeStarted()
        {
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 30);
        }

        public override async void OnStart()
        {
            // Set criminal to be arguing with teller
            Ped fireinspector = Entity.FromHandle(fireinspectorid) as Ped;

            if (null == fireinspector) return;

            var fireinspectorbehavioru = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(fireinspector);
            if (fireinspectorbehavioru != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_HI"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("missfam4", "base"),
                    new Tuple<string, string>("cellphone@","cellphone_call_listen_base")
                };

                fireinspectorbehavioru.Set(bb => bb.AnimationsTupleList, animationsList);
                fireinspectorbehavioru.Set(bb => bb.Speeches,
                    new List<string>()
                    {
                        "Hello, Do you have any questions for me?", "I am the onsite fire officer"
                    });
                fireinspectorbehavioru.Set(bb => bb.SpeechTicksInterval, 600);
                fireinspectorbehavioru.Set(bb => bb.AmbientSounds, ambientSoundList);

                API.TaskCombatPed(fireinspector.Handle, _attendees.GetRandom().player.Character.Handle, 0, 16);
            }

            if (API.DoesEntityExist(fireinspectorid))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(fireinspector.NetworkId);
                pedInfo.QuestionList = "FireInspectorQuestions";
                pedInfo.IsOnCannabis = false;
                pedInfo.IsOnHeroin = false;
                pedInfo.IsOnEcstasy = false;
                _pedInfo.AddOrUpdate(pedInfo);
            }

            foreach (var attendee in _attendees)
            {
                await _xpService.IncreasePlayerXP(attendee.player, 15, "for attending the callout");
            }
        }

        public override bool CanBeResolved()
        {
            // Can be resolved once the criminal is dealt with
            // Firstly, check if the criminal still exists. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (!API.DoesEntityExist(fireinspectorid)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(fireinspectorid)) return true;

            // If the callout has been going on for ages (>30 minutes), let's just resolve it
            if (DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(StartTimestamp).UtcDateTime.AddMinutes(30))
                return true;

            // Not met any criteria yet
            return false;
        }

        public override IAiCallout OnResolve()
        {
            foreach (var player in GetAttendeesWithinRadius(_calloutLocation, 10))
            {
                _xpService.IncreasePlayerXP(player, 15, "for resolving the callout");
            }
            if (IsPedDead(fireinspectorid))
            {
                return new CIDSuddenDeathCallout(fireinspectorid, _xpService);
            }

            return null;
        }

        public override bool CanBeCompleted()
        {
            if (HasAttendees())
            {
                // Players are still attached to the call
                return false;
            }

            // Have the originally attached players left the area?
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 30);
        }

        public override void OnComplete()
        {
            // Delete the Peds
            DeletePedById(fireinspectorid);
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y),
                436);
        }
        
        public List<UserRole> TargetedDivisions()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Branch = UserBranch.Fire,
                    Division = UserDivision.LFB
                }
            };
        }
    }
}