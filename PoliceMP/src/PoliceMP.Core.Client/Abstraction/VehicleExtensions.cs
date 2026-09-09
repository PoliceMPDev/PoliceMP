using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace PoliceMP.Core.Client.Abstraction
{
    public static class VehicleExtensions
    {
        private static Random _random = new Random();

        /// <summary>
        /// Tries to flip the car if conditions are right
        /// </summary>
        /// <param name="vehicle">Vehicle to try flip</param>
        /// <param name="threshold">0.0-1.0 value of how sensitive this is. 0.0 will flip almost constantly.</param>
        /// <param name="chancePerSecond">0.0-1.0 chance is there for the flip to happen? 1.0 = always</param>
        /// <returns></returns>
        public static bool TryFlip(this Vehicle vehicle, float threshold = 0.7f, float chancePerSecond = 0.001f)
        {
            int hVehicle = vehicle.Handle;
            threshold = MathUtil.Clamp(threshold, 0.0f, 1.0f);
            chancePerSecond = MathUtil.Clamp(chancePerSecond, 0.0f, 1.0f);
            var normalizedVelocity = vehicle.Velocity;
            normalizedVelocity.Normalize();
            var rollValue = Vector3.Dot(vehicle.RightVector, normalizedVelocity);

            if (vehicle.ClassType is VehicleClass.Boats
                || vehicle.ClassType is VehicleClass.Cycles
                || vehicle.ClassType is VehicleClass.Helicopters
                || vehicle.ClassType is VehicleClass.Motorcycles
                || vehicle.ClassType is VehicleClass.Planes
                || vehicle.ClassType is VehicleClass.Trains)
            {
                return false;
            }

            if (Math.Abs(rollValue) < threshold)
            {
                return false;
            }

            if (vehicle.Speed < 10f || !vehicle.IsOnAllWheels)
            {
                return false;
            }

            float randomValue;
            lock (_random)
            {
                randomValue = _random.Next(100);
            }

            if (randomValue < chancePerSecond * Game.LastFrameTime)
            {
                var mass = API.GetVehicleHandlingFloat(hVehicle, "CHandlingData", "fMass");
                vehicle.ApplyForce(
                    Vector3.Up * mass * 0.001f,
                    new Vector3(rollValue * mass * -1f, 0f, 0f),
                    ForceType.MaxForceRot
                );

                return true;
            }

            return false;
        }
    }
}
