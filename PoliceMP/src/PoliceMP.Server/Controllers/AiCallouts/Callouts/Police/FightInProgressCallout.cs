using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Police
{
    public class FightInProgressCallout : BaseCallout, IAiCallout
    {
        //***
        //This Is A callout at YellowJacks (Assaults / Afray) located at Postal 949 involving multiple individuals.
        //***
        private readonly IBehaviorService _behaviors;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        public readonly List<Vector3> PedInYellowJacks = new List<Vector3>()
        {
            new((float)1983.18, (float)3053.79, (float)47.21),
            new((float)1982.36, (float)3052.49, (float)47.21),
            new((float)1984.06, (float)3054.65, (float)47.21),
            new((float)1983.01, (float)3050.21, (float)47.21),
            new((float)1984.22, (float)3049.35, (float)47.21),
            new((float)1987.32, (float)3050.83, (float)47.21),
            new((float)1990.14, (float)3048.30, (float)47.21),
            new((float)1990.99, (float)3045.39, (float)47.21),
            new(1993.521f, 3050.355f, 47.21532f),
            new(1995.276f, 3048.846f, 47.21531f),
            new(1994.032f, 3046.033f, 47.21518f),
        };

        private Vector3 _calloutLocation = new((float)1983.18, (float)3053.79, (float)47.21);
        private int _pedId;
        private List<int> pedIds = new List<int>();

        public FightInProgressCallout(IBehaviorService behaviors, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Medium sized public disturbance";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            var responses = new List<string>()
            {
                "Member of the public advising disturbance within a licenced premises",
                "Member of the public reporting a bar fight in progress",
                "Males reported to have knifes on premises",
                "Landlord reporting mass fight on premises",
                "Landlady reporting mass fight on premises",
                "Bar staff have been assaulted on premises",
                "Off-duty police officer reporting medium sized disturbance",
                "Youths reported to drinking on a licenced premises"
            };
            return responses.GetRandom();
        }

        public override void OnCreate()
        {
            foreach (var location in PedInYellowJacks)
            {
                PedHash randomPedHash =
                    AiCalloutsPeds.LowLevelCriminal[_random.Next(AiCalloutsPeds.LowLevelCriminal.Count)];

                _pedId = API.CreatePed(
                    0,
                    (uint)randomPedHash,
                    location.X,
                    location.Y,
                    location.Z,
                    0,
                    true,
                    false);
                pedIds.Add(_pedId);
            }
        }

        public override bool CanBeStarted()
        {
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 30);
        }

        public override async void OnStart()
        {
            Ped ped = Entity.FromHandle(pedIds[0]) as Ped;
            var fightBlackboard = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(ped);
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
                    new List<string>() { "I'm not drunk, you're drunk!", "Fuck you!" });
                fightBlackboard.Set(bb => bb.SpeechTicksInterval, 300);
                fightBlackboard.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            for (var i = 1; i <= 10; i++)
            {
                ped = Entity.FromHandle(pedIds[i]) as Ped;
                fightBlackboard = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(ped);
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
                        new List<string>() { "I'm not drunk, you're drunk!", "Fuck you!" });
                    fightBlackboard.Set(bb => bb.SpeechTicksInterval, 300);
                    fightBlackboard.Set(bb => bb.AmbientSounds, ambientSoundList);
                }

                if (API.DoesEntityExist(_pedId))
                {
                    PedInfo pedInfo = _pedInfo.GetByNetworkId(ped.NetworkId);
                    pedInfo.QuestionList = "FightInProgressQuestions";
                    _pedInfo.AddOrUpdate(pedInfo);
                    await _behaviors.SetPedBehavior<FighterBehavior>(ped);
                }
            }

            foreach (var attendee in _attendees)
            {
                await _xpService.IncreasePlayerXP(attendee.player, 15, "for attending the callout");
            }
        }

        public override bool CanBeResolved()
        {
            // If the callout has been going on for ages (>30 minutes), let's just resolve it
            if (DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(StartTimestamp).UtcDateTime.AddMinutes(30))
                return true;

            foreach (var pedId in pedIds)
            {
                if (API.DoesEntityExist(pedId))
                {
                    if (!IsPedDead(pedId))
                    {
                        // There is at least one ped still alive, so we can't resolve this yet.
                        return false;
                    }
                }
            }

            return true;
        }

        public override IAiCallout OnResolve()
        {
            foreach (var player in GetAttendeesWithinRadius(_calloutLocation, 10))
            {
                _xpService.IncreasePlayerXP(player, 15, "for resolving the callout");
            }

            return null; // Don't spawn a new callout
        }

        public override bool CanBeCompleted()
        {
            if (HasAttendees())
            {
                return false;
            }

            // Have the originally attached players left the area?
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 30);
        }

        public override void OnComplete()
        {
            // Delete the Peds with Nob head Moderators Ray Guns!
            foreach (var pedId in pedIds)
            {
                DeletePedById(pedId);
            }
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
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.Clinical
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Dsu
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