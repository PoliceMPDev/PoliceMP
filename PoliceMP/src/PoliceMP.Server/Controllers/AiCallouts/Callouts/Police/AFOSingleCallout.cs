using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Extensions;
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
    public class AFOSingleCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int loneshooter;


        private Vector3 _calloutLocation;

        public AFOSingleCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Individual with a firearm";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "Reported a male carying a firearm in public";
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            Vector3 loneshooterlocation = AiCalloutsLocations.DrugAlleywaysFoot.GetRandom();
            _calloutLocation = loneshooterlocation;
            loneshooter = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                loneshooterlocation.X,
                loneshooterlocation.Y,
                loneshooterlocation.Z,
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
            Ped loneShooter = Entity.FromHandle(loneshooter) as Ped;

            if (null == loneShooter) return;

            var loneShooterBehavior = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(loneShooter);
            if (loneShooterBehavior != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_CURSE_MED",
                    "PROVOKE_TRESPASS",
                    "SHOUT_THREATEN_GANG",
                    "SHOUT_THREATEN_PED",
                    "GENERIC_FUCK_YOU",
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("cellphone@", "cellphone_call_listen_base"),
                    new Tuple<string, string>("cellphone@", "cellphone_text_read_base"),
                    new Tuple<string, string>("amb@world_human_smoking@male@male_a@enter", "enter"),
                    new Tuple<string, string>("amb@world_human_aa_smoke@male@idle_a", "idle_c"),
                    new Tuple<string, string>("amb@world_human_aa_smoke@male@idle_a", "idle_b"),
                };

                loneShooterBehavior.Set(bb => bb.AnimationsTupleList, animationsList);
                loneShooterBehavior.Set(bb => bb.Speeches,
                    new List<string>()
                    {
                        "Yeah mate, I got the gear here.", "What do you guys want?", "I can sort you a 10 bag",
                        "Crackhead Pete, you ow me money!"
                    });
                loneShooterBehavior.Set(bb => bb.SpeechTicksInterval, 500);
                loneShooterBehavior.Set(bb => bb.AmbientSounds, ambientSoundList);

                API.GiveWeaponToPed(loneShooter.Handle, AiCalloutsWeapons.ArmedRobberyWeapons.GetRandom(), 4000, false, true);
                API.TaskCombatPed(loneShooter.Handle, int.Parse(_attendees.GetRandom().player.Handle), 0, 16);
                API.TaskShootAtEntity(loneShooter.Handle, int.Parse(_attendees.GetRandom().player.Handle), int.MaxValue, 3337513804);
            }

            if (API.DoesEntityExist(loneshooter))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(loneShooter.NetworkId);
                // pedInfo.QuestionList = "DrugQuestions";
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
            if (!API.DoesEntityExist(loneshooter)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(loneshooter)) return true;

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

            if (IsPedDead(loneshooter))
            {
                return new CIDSuddenDeathCallout(loneshooter, _xpService);
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
                    Division = UserDivision.Afo
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Npas
                }
            };
        }
    }
}