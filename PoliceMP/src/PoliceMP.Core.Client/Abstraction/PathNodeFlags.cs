using System;

namespace PoliceMP.Core.Client.Abstraction
{
    [Flags]
    public enum PathNodeFlags
    {
        Offroad = 0x1,
        WanderTarget = 0x2,
        NoBigVehicles = 0x4,
        Disabled = 0x8,
        Tunnel = 0x10,
        DeadEnd = 0x20,
        Highway = 0x40,
        Junction = 0x80,
        TrafficLightStop = 0x100,
        Stop = 0x200,
        Unk1 = 0x400
    }

    [Flags]
    public enum NodeSearchFlags
    {
        None = 0x0,
        IncludeDisabled = 0x1,
        OnlyWaterNodes = 0x2,
        IgnoreSecondaryNodes = 0x4,
        IgnoreDeadEnds = 0x8
    }
}
