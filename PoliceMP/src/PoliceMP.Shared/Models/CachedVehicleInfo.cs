using System;

namespace PoliceMP.Shared.Models
{
    public class CachedVehicleInfo
    {
        public VehicleInfo VehicleInfo { get; }
        public DateTime CachedAt { get; }

        public CachedVehicleInfo(VehicleInfo vehicleInfo)
        {
            VehicleInfo = vehicleInfo;
            CachedAt = DateTime.Now;
        }
    }
}