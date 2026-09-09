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
using PoliceMP.Shared.NetworkMessages.Callouts.Commands;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Highways
{
    //@todo NOT APPLYING THE FUCKING BEHAVIOUR TO THE BASTURD PED, NEEDS FUCKING FIXING BEFORE I SET FIRE TO THIS VIRTUAL WORLD
    //WOODY LOVES YOU
    public class BrokenDownVehicleCallout : BaseCallout, IAiCallout
    {
        private readonly IBehaviorService _behaviors;
        private readonly AiCalloutsPeds _peds;
        private readonly IPedInfoService _pedInfo;
        private readonly IServerCommunicationsManager _comms;
        private Tuple<PoliceMP.Core.Shared.Models.PmpVector3, PoliceMP.Core.Shared.Models.PmpVector3> _selectedLocation;
        private readonly IXPService _xpService;


        private readonly Random _random = new Random();

        public readonly List<string> NumberPlates = new List<string>()
        {
            "W00DDY",
            "A10ON",
            "J03EDGR",
            "D3C0N",
            "CO0NORM",
            "SP1NDID",
            "B3NJ11",
            "FU77H4T",
            "K144N"
        };


        private Vector3 _calloutLocation;
        private int brokendownvictim;
        private List<int> pedIds = new List<int>();
        private int brokenVehicle;
        private int highwaysvictim;


        public BrokenDownVehicleCallout(
            IBehaviorService behaviors,
            IServerCommunicationsManager _comms,
            AiCalloutsPeds peds,
            IPedInfoService pedInfo,
            IXPService xpService)
        {
            _behaviors = behaviors;
            _peds = peds;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {
            return "SOS: Broken Down Vehicle / Motorway";
        }

        public override string Subtitle()
        {
            return "";
        }

        public override string Body()
        {
            return "Highways England control room: Reporting broken down vehicle on live lane.";
        }

        public override async void OnCreate()
        {
            var selectedLocationTuple = AiCalloutHighwaysList.BreakdownLocations.GetRandom();
            _selectedLocation = new Tuple<PoliceMP.Core.Shared.Models.PmpVector3, PoliceMP.Core.Shared.Models.PmpVector3>(
                new PoliceMP.Core.Shared.Models.PmpVector3(selectedLocationTuple.Item1.X, selectedLocationTuple.Item1.Y,
                    selectedLocationTuple.Item1.Z),
                new PoliceMP.Core.Shared.Models.PmpVector3(selectedLocationTuple.Item2.X, selectedLocationTuple.Item2.Y,
                    selectedLocationTuple.Item2.Z));


            _calloutLocation = new Vector3(_selectedLocation.Item1.X, _selectedLocation.Item1.Y,
                _selectedLocation.Item1.Z);
            brokenVehicle = API.CreateVehicle(AiCalloutHighwaysList.VehicleModels.GetRandom(),
                _selectedLocation.Item2.X, _selectedLocation.Item2.Y, _selectedLocation.Item2.Z, 1.0f, true, true);

            API.SetVehicleDoorBroken(brokenVehicle, 1, true);
            // API.SetVehicleNumberPlateText(brokenVehicle, NumberPlates.GetRandom());
            API.SetVehicleBodyHealth(brokenVehicle, 0.0f);
            API.SetVehicleDirtLevel(brokenVehicle, 15);

            

            int random = _random.Next(AiCalloutsPeds.AmbientMales.Count);
            brokendownvictim = API.CreatePed(
                0,
                (uint)AiCalloutsPeds.AmbientMales[random],
                _selectedLocation.Item1.X,
                _selectedLocation.Item1.Y,
                _selectedLocation.Item1.Z,
                0,
                true,
                false);
            pedIds.Add(brokendownvictim);


            Ped brokendownped = Entity.FromHandle(brokendownvictim) as Ped;
            if (null == brokendownped) return;

            var highwaysvictimbehaviour = await _behaviors.SetPedBehavior<PlayAnimationBehavior>(brokendownped);
            if (highwaysvictimbehaviour != null)
            {
                var ambientSoundList = new List<string>()
                {
                    "APOLOGY_NO_TROUBLE"
                };

                var animationsList = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>("cellphone@", "cellphone_call_listen_base"),
                    new Tuple<string, string>("cellphone@", "cellphone_text_read_base")
                };

                highwaysvictimbehaviour.Set(bb => bb.AnimationsTupleList, animationsList);
                highwaysvictimbehaviour.Set(bb => bb.Speeches,
                    new List<string>()
                        { "Hey mate, Any chance you can help?", "I have broken down, what am I going to do?" });
                highwaysvictimbehaviour.Set(bb => bb.SpeechTicksInterval, 500);
                highwaysvictimbehaviour.Set(bb => bb.AmbientSounds, ambientSoundList);
            }

            if (API.DoesEntityExist(brokendownvictim))
            {
                PedInfo pedInfo = _pedInfo.GetByNetworkId(brokendownped.NetworkId);
                // pedInfo.IsOnCannabis = false;
                // pedInfo.IsOnCocaine = false;
                // pedInfo.IsOnEcstasy = false;
                // pedInfo.Items = new();
                // pedInfo.Charges = new();
                // pedInfo.Warrants = new();
                pedInfo.QuestionList = "BrokenDownVehicleQuestions";
                _pedInfo.AddOrUpdate(pedInfo);
            }

            foreach (var attendee in _attendees)
            {
                await _xpService.IncreasePlayerXP(attendee.player, 15, "for attending the callout");
            }
        }

        public override bool CanBeStarted()
        {
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 300);
        }

        public override async void OnStart()
        {
            // Set criminal to be arguing with teller
        }

        public override bool CanBeResolved()
        {
            // Can be resolved once the criminal is dealt with
            // Firstly, check if the criminal still exists. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (!API.DoesEntityExist(brokendownvictim)) return true;

            // If the vehicle no longer exists (has been towed away), then we can resolve
            if (!API.DoesEntityExist(brokenVehicle)) return true;

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
            if (IsPedDead(brokendownvictim))
            {
                return new CIDSuddenDeathCallout(brokendownvictim, _xpService);
            }

            return null;
        }

        public override bool CanBeCompleted()
        {
            if (HasAttendees())
            {
                return false;
            }

            // Have the originally attached players left the area?
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 100);
        }

        public override void OnComplete()
        {
            // Delete the Peds with Nob head Moderators Ray Guns!
            foreach (var pedId in pedIds)
            {
                DeletePedById(pedId);
            }

            if (API.DoesEntityExist(brokenVehicle))
            {
                API.DeleteEntity(brokenVehicle);
            }
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y), 380);
        }

        public List<UserRole> TargetedDivisions()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Branch = UserBranch.Highways,
                    Division = UserDivision.None
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.None
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Ert
                },
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Rpu
                }
            };
        }
    }
}