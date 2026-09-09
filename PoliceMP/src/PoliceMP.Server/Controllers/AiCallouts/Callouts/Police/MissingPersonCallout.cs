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
    public class MissingPersonCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int walker;


        private readonly Vector3 patientlocation = new(495.86f, 5622.62f, 792.07f);


        private Vector3 _calloutLocation = new(495.86f, 5622.62f, 792.07f);

        public MissingPersonCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {

            var mispalist = new List<string>()
            {
                "High Risk: Missing persons",
                "Low Risk: Missing persons",
                "Medium Risk: Missing persons"
            };
            return mispalist.GetRandom();
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            var bodylist = new List<string>()
            {
                "My family member has gone jogging but not returned",
                "Reports of a persons in distress just of pike pass",
                "Reports of an injured individual just off lane pikes",
                "Patient with dementia hasn't returned back to his care home",
                "Coast guard reported a male spotted in distress during a training operation",
                "Boyle lost on pike pass"
            };
            return bodylist.GetRandom();
        }


        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            Vector3 MissingPersonLocation = AiCalloutsLocations.MissingPersonLocation.GetRandom();
            _calloutLocation = MissingPersonLocation;
            walker = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                MissingPersonLocation.X,
                MissingPersonLocation.Y,
                MissingPersonLocation.Z,
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
            Ped walkerped = Entity.FromHandle(walker) as Ped;

            if (null == walkerped) return;

            var walkerpedbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(walkerped);
            if (walkerpedbehaviour != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_FRIGHTENED_MED",
                    "GENERIC_SHOCKED_MED",
                    "GENERIC_SHOCKED_HIGH",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("anim@heists@ornate_bank@hostages@hit", "hit_loop_ped_b"),
                    new Tuple<string, string>("random@mugging5", "001445_01_gangintimidation_1_female_idle_b"),
                    new Tuple<string, string>("friends@frj@ig_1", "wave_b"),
                    new Tuple<string, string>("friends@frj@ig_1", "wave_c"),
                    new Tuple<string, string>("friends@frj@ig_1", "wave_d"),
                    new Tuple<string, string>("friends@frj@ig_1", "wave_d")
                };

                walkerpedbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                walkerpedbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                    {
                        "Help please someone help me!", "I see the helicopter!", "Thank you so much for coming to me"
                    });
                walkerpedbehaviour.Set(bb => bb.SpeechTicksInterval, 100);
                walkerpedbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(walker))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(walkerped.NetworkId);
                pedInfo.QuestionList = "MissingPersonQuestions";
                _pedInfo.AddOrUpdate(pedInfo);
            }

            foreach (var attendee in _attendees)
            {
                await
                    _xpService.IncreasePlayerXP(attendee.player, 15, "for attending the callout");
            }
        }

        public override bool CanBeResolved()
        {
            // Can be resolved once the criminal is dealt with
            // Firstly, check if the criminal still exists. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (!API.DoesEntityExist(walker)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(walker)) return true;

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
            if (IsPedDead(walker))
            {
                return new CIDSuddenDeathCallout(walker, _xpService);
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
            DeletePedById(walker);
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForRadius(
                new Vector2(_calloutLocation.X, _calloutLocation.Y),
                1000
            );
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
                    Division = UserDivision.Npas
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.hart
                },
                new UserRole()
                {
                    Branch = UserBranch.Fire,
                    Division = UserDivision.CoastGuard
                },
                new UserRole()
                {
                    Branch = UserBranch.Fire,
                    Division = UserDivision.mountainRescue
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Npas
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.Survive
                }
            };
        }
    }
}