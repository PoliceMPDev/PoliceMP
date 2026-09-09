using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Police

//@todo Need to make it so all the peds can be asked questions and sort questions and also need to set the ped headings
{
    public class RobberyCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int shopowner;
        private int tillthief;
        private int safeman;

        private readonly Vector3 shopownerlocation = new(-47.35f, -1759.32f, 29.42f);
        private readonly Vector3 tillthieflocation = new(-46.44f, -1757.96f, 29.42f);
        private readonly Vector3 safemanlocation = new Vector3(-42.95f, -1748.87f, 29.42f);


        private Vector3 _calloutLocation = new(-47.51f, -1759.45f, -1759.45f);

        public RobberyCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Shop Theft (Intruders Onsite)";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "Silent alarm triggered at a premises";
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            shopowner = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                shopownerlocation.X,
                shopownerlocation.Y,
                shopownerlocation.Z,
                0,
                true,
                false);

            random = _random.Next(AiCalloutsPeds.AmbientMales.Count);

            tillthief = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                tillthieflocation.X,
                tillthieflocation.Y,
                tillthieflocation.Z,
                0,
                true,
                false);

            random = _random.Next(AiCalloutsPeds.AmbientMales.Count);

            safeman = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                safemanlocation.X,
                safemanlocation.Y,
                safemanlocation.Z,
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
            Ped shopownerped = Entity.FromHandle(shopowner) as Ped;
            Ped tillthiefped = Entity.FromHandle(tillthief) as Ped;
            Ped safemanped = Entity.FromHandle(safeman) as Ped;

            if (null == shopownerped) return;

            var shopownerman = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(shopownerped);
            if (shopownerman != null)
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

                shopownerman.Set(bb => bb.AnimationsTupleList, animationsList);
                shopownerman.Set(bb => bb.Speeches,
                    new List<string>() { "Just leave me alone", "Mate your the one who started this" });
                shopownerman.Set(bb => bb.SpeechTicksInterval, 100);
                shopownerman.Set(bb => bb.AmbientSounds, ambientSoundList);
            }


            if (null == tillthiefped) return;


            var tillthiefman = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(tillthiefped);
            if (tillthiefman != null)
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
                    new Tuple<string, string>("random@mugging4", "struggle_loop_b_thief")
                };

                tillthiefman.Set(bb => bb.AnimationsTupleList, animationsList);
                tillthiefman.Set(bb => bb.Speeches,
                    new List<string>() { "Keep quite man, no sudden movements", "Get down and shut up" });
                tillthiefman.Set(bb => bb.SpeechTicksInterval, 300);
                tillthiefman.Set(bb => bb.AmbientSounds, ambientSoundList);
            }


            var safecrackman = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(safemanped);
            if (null == safecrackman) return;
            if (safecrackman != null)
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
                    new Tuple<string, string>("rcmextreme3", "idle")
                };

                safecrackman.Set(bb => bb.AnimationsTupleList, animationsList);
                safecrackman.Set(bb => bb.Speeches,
                    new List<string>()
                        { "Keep it quiet please, im trying to get in the safe", "Whats the fucking combination!" });
                safecrackman.Set(bb => bb.SpeechTicksInterval, 300);
                safecrackman.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(shopowner))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(shopownerped.NetworkId);
                pedInfo.QuestionList = "RobberyQuestions";
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
            if (!API.DoesEntityExist(tillthief)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(tillthief)) return true;

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
            DeletePedById(shopowner);
            DeletePedById(tillthief);
            DeletePedById(safeman);
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
                    Division = UserDivision.Dsu
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Cid
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.JRU
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.None
                }
            };
        }
    }
}