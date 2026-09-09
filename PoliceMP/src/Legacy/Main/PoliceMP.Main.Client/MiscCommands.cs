using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using PoliceMP.Client.Managers;

namespace PoliceMP.Client
{
    /// <summary>
    ///     All the commands.
    /// </summary>
    public class MiscCommands : BaseScript
    {
        [Command("fixme")]
        private void Cmd_FixMe()
        {
            API.DetachEntity(Game.PlayerPed.Handle, true, false);

            API.ClearPedTasksImmediately(Game.PlayerPed.Handle);

            if (Game.PlayerPed.CurrentVehicle != null)
                Game.PlayerPed.CurrentVehicle.IsPositionFrozen = false;

            Game.PlayerPed.IsPositionFrozen = false;

            this.SendChatMessage("fixed");
        }

        [Command("pos")]
        private async void Cmd_Pos()
        {
            this.SendChatMessage($"{Game.PlayerPed.Position.ToString()}");
        }
    }
}