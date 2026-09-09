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
    public class BtpFightInProgress : BaseCallout, IAiCallout
    {
        //***
        //This Is A callout at YellowJacks (Assaults / Afray) located at Postal 949 involving multiple individuals.
        //***
        private readonly IBehaviorService _behaviors;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        public readonly List<Vector3> PedInTubeStation = new List<Vector3>()
        {
            new((float)-840.3309, (float)-148.8459, (float)19.9504),
            new((float)-836.7032, (float)-151.7574, (float)19.9504),
            new((float)-839.6224, (float)-154.3406, (float)19.9504),
            new((float)-838.2465, (float)-147.8867, (float)19.9504),
            new((float)-836.0451, (float)-146.0194, (float)19.9504),
            new((float)-846.5429, (float)-151.4074, (float)19.9504),
            new((float)-846.1670, (float)-154.4074, (float)19.9504),
            new((float)-842.3066, (float)-156.0213, (float)19.9504),
            new((float)-846.0674, (float)-160.5947, (float)19.9504),
            new((float)-826.6807, (float)-149.1237, (float)19.9504),
            new((float)-830.5389, (float)-146.2841, (float)19.9504),
            
        };

        private Vector3 _calloutLocation = new((float)-840.6364, (float)-149.1861, (float)19.9504);
        private int _pedId;
        private List<int> pedIds = new List<int>();

        public BtpFightInProgress(IBehaviorService behaviors, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Public disturbance at Tube Station";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            var responses = new List<string>()
            {
                "Member of the public advising disturbance within a tube station",
                "Member of the public reporting a fight in progress",
                "Males reported to have knifes on premises",
                "Commuters reporting mass fight in tube station",
                "Pedestrians reporting mass fight in tube station",
                "Staff at the tube have been assaulted",
                "Off-duty police officer reporting medium sized disturbance",
                "Youths reported to drinking in a tube station"
            };
            return responses.GetRandom();
        }

        public override void OnCreate()
        {
            foreach (var location in PedInTubeStation)
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
                    new List<string>() { "What are you looking at? Piss Off!" });
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
                        new List<string>() { "No! You piss off!, You're drunk!", "Get Away From Me!" });
                    fightBlackboard.Set(bb => bb.SpeechTicksInterval, 300);
                    fightBlackboard.Set(bb => bb.AmbientSounds, ambientSoundList);
                }

                if (API.DoesEntityExist(_pedId))
                {
                    PedInfo pedInfo = _pedInfo.GetByNetworkId(ped.NetworkId);
                    pedInfo.QuestionList = "BtpFightInProgressQuestions";
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