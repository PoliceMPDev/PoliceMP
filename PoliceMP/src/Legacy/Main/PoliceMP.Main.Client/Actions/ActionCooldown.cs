using CitizenFX.Core;
using CitizenFX.Core.UI;
using System.Threading.Tasks;

namespace PoliceMP.Main.Client.Actions
{
    /// <summary>
    /// </summary>
    public class ActionCooldown : BaseScript
    {
        /// <summary>
        /// </summary>
        private const int COOLDOWN_SECONDS = 3;

        /// <summary>
        /// </summary>
        private static bool _canPlayAction = true;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [Tick]
        private async Task CanPlayActionTick()
        {
            if (!_canPlayAction)
            {
                await Delay(COOLDOWN_SECONDS * 1000);
                _canPlayAction = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static bool Check()
        {
            if (_canPlayAction == false)
            {
                Screen.ShowSubtitle("~r~Please wait before doing another action.");
                return false;
            }

            _canPlayAction = false;
            return true;
        }
    }
}