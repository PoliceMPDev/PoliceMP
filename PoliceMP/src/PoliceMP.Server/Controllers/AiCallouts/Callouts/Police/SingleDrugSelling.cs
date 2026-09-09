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
    public class SingleDrugSelling : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int drugdealer;


        private Vector3 _calloutLocation;

        public SingleDrugSelling(IBehaviorService behaviors, AiCalloutsPeds peds, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Suspicious Individual Reported";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "Individual has been seen exchanging something with multiple people over the last 30 minutes.";
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            Vector3 drugdealerlocations = AiCalloutsLocations.DrugAlleywaysFoot.GetRandom();
            _calloutLocation = drugdealerlocations;
            drugdealer = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                drugdealerlocations.X,
                drugdealerlocations.Y,
                drugdealerlocations.Z,
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
            Ped drugdealerped = Entity.FromHandle(drugdealer) as Ped;

            if (null == drugdealerped) return;

            var drugdealerbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(drugdealerped);
            if (drugdealerbehaviour != null)
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

                drugdealerbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                drugdealerbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                    {
                        "Yeah mate, I got the gear here.", "What do you guys want?", "I can sort you a 10 bag",
                        "Crackhead Pete, you ow me money!"
                    });
                drugdealerbehaviour.Set(bb => bb.SpeechTicksInterval, 500);
                drugdealerbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);

                API.GiveWeaponToPed(drugdealerped.Handle, (uint)WeaponHash.Knife, 1000, false, true);
                API.TaskCombatPed(drugdealerped.Handle, _attendees.GetRandom().player.Character.Handle, 0, 16);
            }

            if (API.DoesEntityExist(drugdealer))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(drugdealerped.NetworkId);
                pedInfo.QuestionList = "DrugQuestions";
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
            if (!API.DoesEntityExist(drugdealer)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(drugdealer)) return true;

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
            if (IsPedDead(drugdealer))
            {
                return new CIDSuddenDeathCallout(drugdealer, _xpService);
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
            DeletePedById(drugdealer);
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
                    Division = UserDivision.Cid
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Dsu
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Npas
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