using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.PlayerControllerScript
{
    [Flags]
    public enum PlayerActionFlags
    {
        None =                          0,

        /// <summary>
        /// Disable all player input apart from camera
        /// </summary>
        DisablePlayerControls =         1 << 0,

        /// <summary>
        /// Don't ask for a network migration
        /// </summary>
        SkipNetworkTakeover =           1 << 1,

        /// <summary>
        /// Ignore any network takeover failures
        /// </summary>
        IgnoreNetworkTakeoverFail =     1 << 2,

        /// <summary>
        /// Don't wait for network migration
        /// </summary>
        NoWaitForNetworkTimeout =       1 << 3,

        /// <summary>
        /// The target can flee while this action is being performed
        /// </summary>
        PedTargetCanFlee =              1 << 4,

        /// <summary>
        /// The control must be held to activate
        /// </summary>
        HoldControl =                   1 << 5,

        /// <summary>
        /// Ignores always flee flag
        /// </summary>
        IgnoreAlwaysFlee =              1 << 6, 

        /// <summary>
        /// Allow a far greater distance
        /// </summary>
        AllowFarAway =                  1 << 7, 

        /// <summary>
        /// Don't disable the bound input
        /// </summary>
        InputPassThrough =              1 << 8, 
    }
}
