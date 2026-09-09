using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Police

//@todo Need to make it so all the peds can be asked questions and sort questions
{
    public class DomesticDisputeCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int angrywomanone;
        private int angrymantwo;
        private int kitchenman;

        private readonly Vector3 angrywomanlocation = new(1018.46f, -473.54f, 64.52f);
        private readonly Vector3 angrymantwolocation = new(1017.62f, -472.07f, 64.52f);
        private readonly Vector3 kitchenmanlocation = new Vector3(1021.99f, -469.45f, 64.08f);


        private Vector3 _calloutLocation = new(1018.46f, -473.54f, 64.52f);

        public DomesticDisputeCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Alleged Domestic Dispute";
        }

        public override string Subtitle()
        {
            var possibilities = new List<string>()
            {
                "Known caller",
                "Residence known to police",
                "Violent Towards Police. [Marker]"
            };
            return possibilities.GetRandom();
        }

        public override string Body()
        {
            var responses = new List<string>()
            {
                "Female caller in distress, reporting partner has assaulted them",
                "Male caller, reported than female has assaulted them",
                "Neighbour has reported a disturbance at a property next door"
            };
            return responses.GetRandom();
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientFemales.Count);
            angrywomanone = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientFemales[random],
                angrywomanlocation.X,
                angrywomanlocation.Y,
                angrywomanlocation.Z,
                0,
                true,
                false);

            random = _random.Next(AiCalloutsPeds.AmbientMales.Count);

            angrymantwo = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                angrymantwolocation.X,
                angrymantwolocation.Y,
                angrymantwolocation.Z,
                0,
                true,
                false);

            random = _random.Next(AiCalloutsPeds.AmbientFemales.Count);

            kitchenman = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientFemales[random],
                kitchenmanlocation.X,
                kitchenmanlocation.Y,
                kitchenmanlocation.Z,
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
            Ped angrywomanped = Entity.FromHandle(angrywomanone) as Ped;
            Ped angrymantwoped = Entity.FromHandle(angrymantwo) as Ped;
            Ped kitchenmanped = Entity.FromHandle(kitchenman) as Ped;

            if (null == angrywomanped) return;

            var manOne = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(angrywomanped);
            if (manOne != null)
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
                    new Tuple<string, string>("random@domestic", "f_distressed_loop")
                    
                };

                manOne.Set(bb => bb.AnimationsTupleList, animationsList);
                manOne.Set(bb => bb.Speeches,
                    new List<string>() { "Just leave me alone", "Please get out of our house!", "I'm scared!", "Help Help Help!" });
                manOne.Set(bb => bb.SpeechTicksInterval, 300);
                manOne.Set(bb => bb.AmbientSounds, ambientSoundList);
            }


            if (null == angrymantwoped) return;


            var manTwo = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(angrymantwoped);
            if (manTwo != null)
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
                    new Tuple<string, string>("anim@deathmatch_intros@unarmed", "intro_male_unarmed_c"),
                    new Tuple<string, string>("anim@deathmatch_intros@unarmed", "intro_male_unarmed_e")
                };

                manTwo.Set(bb => bb.AnimationsTupleList, animationsList);
                manTwo.Set(bb => bb.Speeches,
                    new List<string>() { "I will knock your head off!", "I am fed up with this shit!" });
                manTwo.Set(bb => bb.SpeechTicksInterval, 700);
                manTwo.Set(bb => bb.AmbientSounds, ambientSoundList);
            }


            var kitchen = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(kitchenmanped);
            if (null == kitchen) return;
            if (kitchen != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_FRIGHTENED_HIGH",
                    "GENERIC_SHOCKED_HIGH",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("amb@world_human_maid_clean@", "base")
                };

                kitchen.Set(bb => bb.AnimationsTupleList, animationsList);
                kitchen.Set(bb => bb.Speeches,
                    new List<string>() { "Please take them away, please!", "Please make them stop!" });
                kitchen.Set(bb => bb.SpeechTicksInterval, 1200);
                kitchen.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(angrywomanone))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(angrywomanped.NetworkId);
                pedInfo.QuestionList = "DomesticDisputeQuestions";
                _pedInfo.AddOrUpdate(pedInfo);
            }

            foreach (var player in GetAttendeesWithinRadius(_calloutLocation, 10))
            {
                await _xpService.IncreasePlayerXP(player, 15, "for resolving the callout");
            }
        }

        public override bool CanBeResolved()
        {
            // Can be resolved once the criminals are dealt with
            // Firstly, check if all the criminals still exist. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (
                !API.DoesEntityExist(angrywomanone)
                && !API.DoesEntityExist(angrymantwo)
            ) return true;

            // If the criminals have died, then clearly we can't continue with the call
            if (IsPedDead(angrywomanone) && IsPedDead(angrymantwo)) return true;

            // If the callout has been going on for ages (>30 minutes), let's just resolve it
            if (DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(StartTimestamp).UtcDateTime.AddMinutes(30))
                return true;

            // Not met any criteria yet
            return false;
        }

        public override IAiCallout OnResolve()
        {
            foreach (var attendee in _attendees)
            {
                _xpService.IncreasePlayerXP(attendee.player, 15, "for resolving the callout");
            }
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
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 30);
        }

        public override void OnComplete()
        {
            // Delete the Peds
            DeletePedById(angrywomanone);
            DeletePedById(angrymantwo);
            DeletePedById(kitchenman);
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