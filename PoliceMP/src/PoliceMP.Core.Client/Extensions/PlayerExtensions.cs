using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace PoliceMP.Core.Client.Extensions
{
    [Flags]
    public enum PlayerSwitchFlags
    {
        Normal = 0,
        NoTransition = 1,
        SwitchIn = 255
    }

    public enum PlayerSwitchType
    {
        Auto = 0,
        Long = 1,
        Medium = 2,
        Short = 3,
    }

    public static class PlayerExtensions
    {
        public static void SendChatMessage(this Player player,
            string message,
            string title = "PoliceMP",
            bool multiline = true)
        {
            // TODO: Allow color to be specified (maybe use like Color.Red and convert to rgb int)
            BaseScript.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 255, 255 },
                multiline = multiline,
                args = new[] { title, message }
            });
        }

        public static void Freeze(this Player player, bool freeze = true)
        {
            player.CanControlCharacter = !freeze;
            player.Character.IsVisible = !freeze;
            player.Character.IsCollisionEnabled = !freeze;
            player.Character.IsPositionFrozen = freeze;
            player.Character.IsInvincible = freeze;
            player.Character.Task.ClearAllImmediately();
        }

        public static bool IsFirstPerson(this Player _)
        {
            if (Game.PlayerPed.IsInVehicle())
                return API.GetFollowVehicleCamViewMode() == 4;

            return API.GetFollowPedCamViewMode() == 4;
        }

        public static async Task SwitchOut(this Player _, 
            PlayerSwitchFlags switchFlags = PlayerSwitchFlags.Normal, 
            PlayerSwitchType switchType = PlayerSwitchType.Auto,
            int timeoutMs = 30000)
        {
            // Make sure we're on the main thread
            await BaseScript.Delay(0);
            
            API.SwitchOutPlayer(API.PlayerPedId(), (int)switchFlags, (int)switchType);

            var timeout = Game.GameTime + timeoutMs;
            while (API.GetPlayerSwitchState() < 5 && Game.GameTime < timeout)
            {
                Debug.WriteLine($"SwitchOut: {API.GetPlayerSwitchState()}");
                await BaseScript.Delay(1);
            }
        }

        public static async Task SwitchIn(this Player _, Ped ped, int timeoutMs = 30000)
        {
            // Make sure we're on the main thread
            await BaseScript.Delay(0);

            if (!API.IsEntityAPed(ped.Handle))
                return;

            API.SwitchInPlayer(ped.Handle);
            var timeout = Game.GameTime + timeoutMs;
            while (API.GetPlayerSwitchState() < 8 && Game.GameTime < timeout)
                await BaseScript.Delay(1);
        }

        public static async Task SwitchPed(this Player _,
            Ped newPed,
            PlayerSwitchFlags flags = PlayerSwitchFlags.Normal,
            PlayerSwitchType switchType = PlayerSwitchType.Auto,
            int timeoutMs = 30000)
        {
            // Make sure we're on the main thread
            await BaseScript.Delay(0);

            if (newPed == null || !API.IsEntityAPed(newPed.Handle) || newPed.IsPlayer)
                return;

            var totalTimeout = Game.GameTime + timeoutMs;
            await newPed.TryRequestNetworkEntityControl(timeoutMs: timeoutMs);

            API.StartPlayerSwitch(API.PlayerPedId(), newPed.Handle, (int) flags, (int) switchType);
            while (Game.GameTime < totalTimeout)
            {
                var actualSwitchType = (PlayerSwitchType) API.GetPlayerSwitchType();
                var state = API.GetPlayerSwitchState();
                Debug.WriteLine($"[PlayerSwitch] Type: {actualSwitchType}; State: {state}");

                if(state == 8 || state == 12)
                    return;

                await BaseScript.Delay(1);
            }
        }
    }
}