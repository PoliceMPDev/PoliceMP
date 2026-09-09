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
    public class MentalHealthWorldCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int patient;
        private int doctor;
        private int securityguard;


        private Vector3 _calloutLocation;

        public MentalHealthWorldCallout(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Crisis Team Attendance Request";
        }

        public override string Subtitle()
        {
            return "Individual frequent caller";
        }

        public override string Body()
        {
            return "Female caller noticeably distressed believed to be in crisis. [MCT Report]";
        }

        public override void OnCreate()
        {
            int random = +_random.Next(AiCalloutsPeds.AmbientFemales.Count);
            Vector3 patientlocation = AiCalloutsLocations.DrugAlleywaysFoot.GetRandom();
            _calloutLocation = patientlocation;
            patient = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientFemales[random],
                patientlocation.X,
                patientlocation.Y,
                patientlocation.Z,
                0,
                true,
                false);
        }

        public override bool CanBeStarted()
        {
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 50);
        }

        public override async void OnStart()
        {
            // Set criminal to be arguing with teller
            Ped patientped = Entity.FromHandle(patient) as Ped;

            if (null == patientped) return;

            var patientpedbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(patientped);
            if (patientpedbehaviour != null)
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
                    new Tuple<string, string>("anim@mp_player_intcelebrationmale@cut_throat", "cut_throat"),
                    new Tuple<string, string>("anim@mp_player_intcelebrationfemale@cut_throat", "cut_throat"),
                    new Tuple<string, string>("anim@mp_player_intcelebrationmale@mind_blown", "mind_blown"),
                    new Tuple<string, string>("anim@mp_player_intcelebrationmale@shadow_boxing", "shadow_boxing"),
                    new Tuple<string, string>("anim@deathmatch_intros@unarmed", "intro_male_unarmed_c"),
                    new Tuple<string, string>("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper")
                };

                patientpedbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                patientpedbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                        { "I am fed up with everything", "I will hurt all the doctors here", "I am known to MHS." });
                patientpedbehaviour.Set(bb => bb.SpeechTicksInterval, 350);
                patientpedbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(patient))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(patientped.NetworkId);
                pedInfo.QuestionList = "MentalHealthQuestions";
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
            // Firstly, check if the patient still exists. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (!API.DoesEntityExist(patient)) return true;

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
            DeletePedById(patient);
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y), 126);
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
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.StJohn
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.Clinical
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.ClinicalAdv
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.JRU
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