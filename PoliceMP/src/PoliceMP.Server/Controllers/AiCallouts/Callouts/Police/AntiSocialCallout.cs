using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Police
{
    public class AntiSocialCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly ILogger<AntiSocialCallout> _logger;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int tellerId;
        private readonly Vector3 tellerLocation = new((float)149.44, (float)-1042.38, (float)29.36);
        private int criminalId;
        private readonly Vector3 criminalLocation = new(149.92f, -1040.84f, 29.37f);


        private Vector3 _calloutLocation = new((float)149.99, (float)-1040.54, (float)29.37);

        public AntiSocialCallout(IBehaviorService behaviors, ILogger<AntiSocialCallout> logger, AiCalloutsPeds peds,
            IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _logger = logger;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Disturbance in public place";
        }

        public override string Subtitle()
        {
            return "Bank Alarm Tripped";
        }

        public override string Body()
        {
            return "Reports from bank clerk, a member of the public has verbally abused staff";
        }

        public override void OnCreate()
        {
            // Create Teller
            tellerId = API.CreatePed(
                0,
                (uint)PedHash.Bankman,
                tellerLocation.X,
                tellerLocation.Y,
                tellerLocation.Z,
                0,
                true,
                false);

            // Create criminal

            criminalId = API.CreatePed(
                0,
                (uint)PedHash.Business01AMM,
                criminalLocation.X,
                criminalLocation.Y,
                criminalLocation.Z,
                0,
                true,
                false);
        }

        public override bool CanBeStarted()
        {
            // if (checkmyboys == false)
            // {
            //     DeletePedById(tellerId);
            //     DeletePedById(criminalId);
            // }
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 30);
        }

        public override async void OnStart()
        {
            // Set criminal to be arguing with teller
            Ped criminal = Entity.FromHandle(criminalId) as Ped;
            Ped bankman = Entity.FromHandle(tellerId) as Ped;
            if (null == bankman) return;

            var bankTeller = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(bankman);
            if (bankTeller != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_FRIGHTENED_HIGH",
                    "GENERIC_SHOCKED_HIGH",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("random@domestic", "f_distressed_loop")
                };

                bankTeller.Set(bb => bb.AnimationsTupleList, animationsList);
                bankTeller.Set(bb => bb.Speeches,
                    new List<string>() { "Help officers, Please help!", "He has just attacked me!" });
                bankTeller.Set(bb => bb.SpeechTicksInterval, 100);
                bankTeller.Set(bb => bb.AmbientSounds, ambientSoundList);
            }


            if (null == criminal) return;


            var fightBlackboard = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(criminal);
            if (fightBlackboard != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_INSULT_HIGH",
                    "GENERIC_CURSE_HIGH",
                    "GENERIC_CURSE_MED",
                    "PROVOKE_TRESPASS",
                    "SHOUT_THREATEN_GANG",
                    "SHOUT_THREATEN_PED",
                    "GENERIC_FUCK_YOU",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("misscarsteal4@actor", "actor_berating_loop")
                };

                fightBlackboard.Set(bb => bb.AnimationsTupleList, animationsList);
                fightBlackboard.Set(bb => bb.Speeches,
                    new List<string>() { "I'm going to burn this bank to the ground", "I want my money now!" });
                fightBlackboard.Set(bb => bb.SpeechTicksInterval, 300);
                fightBlackboard.Set(bb => bb.AmbientSounds, ambientSoundList);
                
                
            }

            if (API.DoesEntityExist(criminalId))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(criminal.NetworkId);
                pedInfo.QuestionList = "BankArgumentQuestions";
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
            if (!API.DoesEntityExist(criminalId)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(criminalId)) return true;

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
            DeletePedById(tellerId);
            return null; // Don't spawn a new callout
        }

        public override bool CanBeCompleted()
        {
            if (HasAttendees())
            {
                // Players are still attached to the call
                return false;
            }

            // Have the originally attached players left the area?
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 45);
        }

        public override void OnComplete()
        {
            // Delete the Peds
            DeletePedById(criminalId);
            DeletePedById(tellerId);
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y), 188);
        }

        public List<UserRole> TargetedDivisions()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Ert
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.JRU
                }
            };
        }
    }
}