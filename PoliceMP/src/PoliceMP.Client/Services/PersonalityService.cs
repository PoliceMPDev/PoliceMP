using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client.Extensions;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services
{
    public class PersonalityService : IPersonalityService
    {
        private readonly ILogger<PersonalityService> _logger;
        private readonly IPedInfoService _pedInfo;
        private readonly IVehicleInfoService _vehicleInfo;

        public PersonalityService(ILogger<PersonalityService> logger, IPedInfoService pedInfo, IVehicleInfoService vehicleInfo)
        {
            _logger = logger;
            _pedInfo = pedInfo;
            _vehicleInfo = vehicleInfo;
        }

        public async Task<bool> WillPedResist(Ped ped)
        {
            if (ped == null
                || !API.IsEntityAPed(ped.Handle)
                || API.IsPedDeadOrDying(ped.Handle, true))
                return false;

            if (ped.GetBoolDecor(PedDecors.ALWAYS_FLEE))
                return true;

            var pedInfo = await _pedInfo.GetByNetworkId(ped.NetworkId);
            VehicleInfo vehicleInfo = null;

            if (ped.CurrentVehicle != null)
            {
                vehicleInfo = await _vehicleInfo.GetByNetworkId(ped.CurrentVehicle.NetworkId,
                    ped.CurrentVehicle.GetPlateText(), ped.NetworkId);
            }

            var chance = GetChanceOfFlee(pedInfo, vehicleInfo);

            if (chance == 100)
                return true;

            var draw = AppRandom.Next(100);
            _logger.Debug($"Personality service determined that ped has a {chance}% chance of fleeing. Lottery draw = {draw}");
            return draw < chance;
        }

        private int GetChanceOfFlee(PedInfo pedInfo, VehicleInfo vehicleInfo)
        {
            var chance = pedInfo.Attitude / 10;

            if (pedInfo.IsOnAnyDrugs)
                chance += 10;

            if (pedInfo.HasIllegalItems)
                chance += 10;

            if (pedInfo.CriminalMarkers.Any())
                chance += 10 * pedInfo.CriminalMarkers.Count;

            if (pedInfo.Warrants.Any())
                chance += 10 * pedInfo.Warrants.Count;

            if (vehicleInfo != null)
            {
                var v = vehicleInfo;

                if (v.Markers.Any())
                    chance += (10 * v.Markers.Count);

                if (v.IsInsuranceExpired)
                    chance += 10;

                if (v.IsMotExpired)
                    chance += 10;

                if (v.IsTaxExpired)
                    chance += 10;

                if (v.HasIllegalItems)
                    chance += 10;
            }

            return MathUtil.Clamp(chance, 0, 100);
        }
    }
}
