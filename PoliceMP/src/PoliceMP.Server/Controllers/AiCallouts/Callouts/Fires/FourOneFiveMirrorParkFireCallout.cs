using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Server.Controllers.AiCallouts.Callouts.Police;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.AiCallouts;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.NetworkMessages.Callouts.Notifications;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Fires

//@todo Need to make it so all the peds can be asked questions and sort questions
{
    public class FourOneFiveMirrorParkFireCallout : BaseCallout, IAiCallout
    {
        //***
        //This happens at the bank at legion square, Natwest Postal 206
        //***

        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly ILegacyServerCommunicationsManager _legacyComms;
        private readonly IServerCommunicationsManager _comms;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private int victim;

        public readonly List<SmartFireType> FireTypes = new List<SmartFireType>()
        {
            SmartFireType.Bonfire,
            SmartFireType.Chemical,
            SmartFireType.Electrical,
            SmartFireType.Normal3,
            SmartFireType.Normal,
            SmartFireType.Normal2
        };

        public readonly List<int> FireSize = new List<int>()
        {
            5,
            6,
            7
        };

        private Vector3 _calloutLocation;

        public FourOneFiveMirrorParkFireCallout(IBehaviorService behaviors, AiCalloutsPeds peds,
            IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "Building Fire";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "reported structure fire, persons trapped unknown.";
        }

        public override void OnCreate()
        {
            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            Vector3 housefirelocations = AiCalloutsLocations.HouseFireLocation.GetRandom();
            _calloutLocation = housefirelocations;
            victim = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                housefirelocations.X,
                housefirelocations.Y,
                housefirelocations.Z,
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
            Ped victimped = Entity.FromHandle(victim) as Ped;

            if (null == victimped) return;

            var victimpedbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(victimped);
            if (victimpedbehaviour != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "GENERIC_SHOCKED_HIGH"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("missminuteman_1ig_2", "tasered_2"),
                    new Tuple<string, string>("anim@mp_player_intupperface_palm", "idle_a"),
                    new Tuple<string, string>("random@drunk_driver_1", "drunk_fall_over"),
                    new Tuple<string, string>("missarmenian2", "drunk_loop"),
                };
                
                foreach (var atendee in _attendees)
                {
                    try
                    {
                        _comms.PublishToClient(atendee.player, new PlaySpeechEvent()
                        {
                            NetworkId = victimped.NetworkId,
                            Text = "Help, the building is on fire!"
                        });
                    }
                    catch (Exception ex)
                    {
                    }

                    var location = _calloutLocation;
                    await Controller.StartFire(location, FireSize.GetRandom(), FireTypes.GetRandom());
                }

                victimpedbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                victimpedbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                        { "" });
                victimpedbehaviour.Set(bb => bb.SpeechTicksInterval, 5000);
                victimpedbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(victim))
            {
                // PedInfo pedInfo = _pedInfo.GetByNetworkId(victimped.NetworkId);
                // pedInfo.QuestionList = "";
                // // pedInfo.IsOnCannabis = true;
                // // pedInfo.IsOnHeroin = true;
                // // pedInfo.IsOnEcstasy = true;
                // _pedInfo.AddOrUpdate(pedInfo);
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
            if (!API.DoesEntityExist(victim)) return true;

            // If the callout has been going on for ages (>20 minutes), let's just resolve it
            if (DateTime.UtcNow > DateTimeOffset.FromUnixTimeSeconds(StartTimestamp).UtcDateTime.AddMinutes(20))
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
            
            if (IsPedDead(victim))
            {
                return new CIDSuddenDeathCallout(victim, _xpService);
            }

            DeletePedById(victim);

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
            DeletePedById(victim);
        }
        
        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y), 436);
        }
        
        public List<UserRole> TargetedDivisions()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Branch = UserBranch.Fire,
                    Division = UserDivision.LFB
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Rpu
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.None
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.Clinical
                },
                new UserRole()
                {
                    Branch = UserBranch.Nhs,
                    Division = UserDivision.StJohn
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