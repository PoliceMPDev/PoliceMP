using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Core.Client.Abstraction
{
    public enum DrivingStyleFlag
    {
        StopBeforeVehicle = 1 << 0,
        StopBeforePeds = 1 << 1,
        AvoidVehicle = 1 << 2,
        AvoidEmptyVehicles = 1 << 3,
        AvoidPeds = 1 << 4,
        AvoidObjects = 1 << 5,
        Unk1 = 1 << 6,
        StopAtTrafficLights = 1 << 7,
        UseBlinkers = 1 << 8,
        AllowGoingWrongWay = 1 << 9,
        DriveInReverse = 1 << 10,
        Unk2 = 1 << 11,
        Unk3 = 1 << 12,
        Unk4 = 1 << 13,
        Unk5 = 1 << 14,
        Unk6 = 1 << 15,
        Unk7 = 1 << 16,
        Unk8 = 1 << 17,
        TakeShortestPath = 1 << 18,
        Reckless = 1 << 19,
        Unk9 = 1 << 20,
        Unk10 = 1 << 21,
        IgnoreRoads = 1 << 22,
        Unk11 = 1 << 23,
        IgnoreAllPathing = 1 << 24,
        Unk12 = 1 << 25,
        Unk13 = 1 << 26,
        Unk14 = 1 << 27,
        Unk15 = 1 << 28,
        AvoidHighways = 1 << 29,
        Unk16 = 1 << 30
    }
}