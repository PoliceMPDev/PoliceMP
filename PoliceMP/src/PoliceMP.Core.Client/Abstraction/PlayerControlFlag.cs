using System;

namespace PoliceMP.Core.Client.Abstraction
{
    [Flags]
    public enum PlayerControlFlag
    {
        None = 0,
        AmbientScript = (1 << 1),
        ClearTasks = (1 << 2),
        RemoveFires = (1 << 3),
        RemoveExplosions = (1 << 4),
        RemoveProjectiles = (1 << 5),
        DeactivateGadgets = (1 << 6),
        ReenableControlOnDeath = (1 << 7),
        LeaveCameraControlOn = (1 << 8),
        AllowPlayerDamage = (1 << 9),
        DontStopOtherCarsAroundPlayer = (1 << 10),
        PreventEverybodyBackoff = (1 << 11),
        AllowPadShake = (1 << 12)
	}
}
