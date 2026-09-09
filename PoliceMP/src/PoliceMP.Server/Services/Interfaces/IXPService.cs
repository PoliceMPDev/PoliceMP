using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Server.Services.Interfaces
{
    public interface IXPService
    {
        /// <summary>
        /// Notify player's client of their current XP value.
        /// Usually to be called when the player spawns.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        Task NotifyInitialXP(Player player);
        
        /// <summary>
        /// Increase the given Player's XP by the xpIncrease value.
        /// Raises an event to the client to notify the client to display XP.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="xpIncrease"></param>
        /// <param name="reason">Reason for the XP increase, will be shown to the player</param>
        /// <returns>New XP value</returns>
        Task IncreasePlayerXP(Player player, int xpIncrease, string reason);

        /// <summary>
        /// Decrease the given Player's XP by the xpDecrease value.
        /// Raises an event to the client to notify the client to display XP.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="xpDecrease"></param>
        /// <param name="reason">Reason for the XP decrease, will be shown to the player</param>
        /// <returns>New XP value</returns>
        Task DecreasePlayerXP(Player player, int xpDecrease, string reason);
    }
}