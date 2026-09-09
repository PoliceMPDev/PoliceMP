using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Shared.Extensions;
using PoliceMP.Main.Core.Server.Enums;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Behaviors.FailToStop;
using PoliceMP.Shared.Enums;
using PoliceMP.Shared.Models;

namespace PoliceMP.Server.Controllers.AiCallouts.Callouts.Police
{
    public class FailToStopCallout : BaseCallout, IAiCallout
    {
        private readonly IBehaviorService _behaviors;
        private readonly IPedInfoService _pedInfo;
        private readonly Random _random = new Random();
        private readonly IXPService _xpService;


        private readonly List<Tuple<Vector3, int>> _calloutLocations = new List<Tuple<Vector3, int>>()
        {
            new(new Vector3((float)1807.313, (float)2129.012, (float)54.93196), -174),
            new(new Vector3((float)2884.379, (float)4185.396, (float)50.16042), 14),
            new(new Vector3((float)2746.129, (float)4656.097, (float)44.55276), 16),
            new(new Vector3((float)2629.960, (float)5170.129, (float)44.77993), 14),
        };

        private Vector3 _calloutLocation = new((float)1807.313, (float)2129.012, (float)54.93196);
        private int _pedId;
        private readonly List<int> _pedIds = new List<int>();
        private string _registerationplate;

        public FailToStopCallout(IBehaviorService behaviors, IPedInfoService pedInfo, IXPService xpService)
        {
            _behaviors = behaviors;
            _pedInfo = pedInfo;
            _xpService = xpService;
        }

        public override string Title()
        {

            var responses = new List<string>()
            {
                "Vehicle stolen with keys",
                "Vehicle stolen without keys",
                "Vehicle stolen from burglary",
                "Reports of vehicle on cloned plates",
                "Previously failed to stop for local unit",
                "Registered keeper with outstanding warrant",
                "Vehicle wanted in relation to county line offences",
                "Vehicle reported for taken without owners consent"
            };
                return responses.GetRandom();
        }

        public override string Subtitle()
        {
            return  $"VRN: {_registerationplate}" ;
        }

        public override string Body()
        {
            return "Bring vehicle to a safe stop.";
        }

        public override void OnCreate()
        {
            var (calloutLocation, calloutHeading) = _calloutLocations.GetRandom();
            _calloutLocation = calloutLocation;

            var pedHash =
                AiCalloutsPeds.LowLevelCriminal[_random.Next(AiCalloutsPeds.LowLevelCriminal.Count)];

            var vehicleId = API.CreateVehicle(
                AiCalloutHighwaysList.VehicleModels[_random.Next(AiCalloutHighwaysList.VehicleModels.Count)],
                _calloutLocation.X, _calloutLocation.Y,
                _calloutLocation.Z, calloutHeading, true, false);

            _pedId = API.CreatePed(
                0,
                (uint)pedHash,
                _calloutLocation.X + 2f,
                _calloutLocation.Y + 2f,
                _calloutLocation.Z + 3f,
                calloutHeading,
                true,
                false);
            _pedIds.Add(_pedId);

            API.TaskWarpPedIntoVehicle(_pedId, vehicleId, (int)VehicleSeat.Driver);
            try
            {
                _registerationplate = API.GetVehicleNumberPlateText(vehicleId);
            }
            catch (Exception _)
            {
                _registerationplate = "Unknown";
            }
        }

        public override bool CanBeStarted()
        {
            return CheckIfAnyAttachedPlayerIsCloseEnough(_attendees, _calloutLocation, 200);
        }

        public override async void OnStart()
        {
            var ped = Entity.FromHandle(_pedId) as Ped;
            if (ped == null) return;
            await _behaviors.SetPedBehavior<FailToStopBehavior>(ped);
            foreach (var attendee in _attendees)
            {
                await _xpService.IncreasePlayerXP(attendee.player, 15, "for attending the callout");
            }
        }

        public override bool CanBeResolved()
        {
            // Can be resolved once the criminal is dealt with
            // Firstly, check if the criminal still exists. If they don't, they may have been picked up by AI PT or deleted by a moderator
            if (!API.DoesEntityExist(_pedId)) return true;

            // If the criminal has died, then clearly we can't continue with the call
            if (IsPedDead(_pedId)) return true;

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
                return false;

            // Have the originally attached players left the area?
            return !CheckIfAnyAttachedPlayerIsCloseEnough(_originalAttendees, _calloutLocation, 150);
        }

        public override void OnComplete()
        {
            // Delete the Peds with Nob head Moderators Ray Guns!
            foreach (var pedId in _pedIds)
                DeletePedById(pedId);
        }

        public override AiCalloutBlip CalloutBlip()
        {
            return AiCalloutBlip.ForLocation(new Vector2(_calloutLocation.X, _calloutLocation.Y), 56);
        }
        
        public List<UserRole> TargetedDivisions()
        {
            return new List<UserRole>()
            {
                new UserRole()
                {
                    Branch = UserBranch.Police,
                    Division = UserDivision.Rpu
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