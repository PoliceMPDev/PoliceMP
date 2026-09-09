using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Abstraction
{
    public enum VehicleTempAction
    {
        Brake = 1,
        BrakeReverse = 3,
        TurnLeft90Brake = 4,
        TurnRight90Brake = 5,
        Handbrake = 6,
        TurnAndAccelerate = 7,
        AccelerationWeak = 9,
        TurnLeft = 10,
        TurnRight = 11,
        TurnLeftReverse = 13,
        TurnRightReverse = 14,
        StrongBrakeTurn = 19,
        WeakBrakeTurnLeftThenRight = 20,
        WeakBrakeTurnRightThenLeft = 21,
        BrakeReverse2 = 22,
        AccelerationStrong = 23,
        Brake2 = 24,
        BrakeLeft = 25,
        BrakeRight = 26,
        BrakeUntilStop = 27,
        BrakeReverseStrong = 28,
        Burnout = 30,
        AccelerateWithHandbrake = 31,
        AccelerateVeryStrong = 32
    }
}