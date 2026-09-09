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

//@todo Need to make it so all the peds can be asked questions and sort questions
{
    public class PublicOrderOffenceCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int publicorderped;


        private Vector3 _calloutLocation;

        public PublicOrderOffenceCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {

            var titlelist = new List<string>()
            {
                "Public order offence"
                
            };
            return titlelist.GetRandom();
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            var bodylist = new List<string>()
            {
                "Male reported shouting racial slurs",
                "Individual reported intoxicated in public place",
                "Male reported urinating in a public place",
                "Male reported throwing objects off bridge",
                "TFL: Reporting drunk male ejected from bus",
                "Landlord ejected male from licenced premises"
                
            };
            return bodylist.GetRandom();
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            Vector3 publicorderlocation = AiCalloutsLocations.DrugAlleywaysFoot.GetRandom();
            _calloutLocation = publicorderlocation;
            publicorderped = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                publicorderlocation.X,
                publicorderlocation.Y,
                publicorderlocation.Z,
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
            Ped publicorderpedvar = Entity.FromHandle(publicorderped) as Ped;

            if (null == publicorderpedvar) return;

            var publicorderbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(publicorderpedvar);
            if (publicorderbehaviour != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_FUCK_YOU",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("friends@frf@ig_2", "knockout_plyr"),
                    new Tuple<string, string>("anim@gangops@hostage@", "victim_fail"),
                    new Tuple<string, string>("timetable@amanda@drunk@base", "base")

                };

                publicorderbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                publicorderbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                    {
                        "Stop bothering me", "I'll knock your head off", "Move away now before I smash you",
                        "My mate Boyle will batter you"
                    });
                publicorderbehaviour.Set(bb => bb.SpeechTicksInterval, 500);
                publicorderbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);

                API.GiveWeaponToPed(publicorderpedvar.Handle, (uint)WeaponHash.Knife, 1000, false, true);
                API.TaskCombatPed(publicorderpedvar.Handle, _attendees.GetRandom().player.Character.Handle, 0, 16);
                API.SetPedRandomProps(publicorderped);
            }

            if (API.DoesEntityExist(publicorderped))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(publicorderpedvar.NetworkId);
                //@todo questions for the public order offence

                pedInfo.QuestionList = "PublicOrderQuestions";
                pedInfo.IsOnCannabis = true;
                pedInfo.IsOnHeroin = true;
                pedInfo.IsOnEcstasy = true;
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
            if (!API.DoesEntityExist(publicorderped)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(publicorderped)) return true;

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
            DeletePedById(publicorderped);
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
                    Division = UserDivision.JRU
                },
            };
        }
    }
}