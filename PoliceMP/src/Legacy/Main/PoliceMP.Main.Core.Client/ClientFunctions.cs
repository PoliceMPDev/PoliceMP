using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Main.Core.Client.Extensions;
using PoliceMP.Main.Core.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core.UI;

namespace PoliceMP.Main.Core.Client
{
    public static class ClientFunctions
    {
        public static void ShowToast(string title, string message, string type)
        {
            BaseScript.TriggerEvent("PoliceMP:ShowNotification", title, message, type);
        }

        public static async Task<bool> LoadModelAsync(string model)
        {
            API.RequestModel((uint)API.GetHashKey(model));
            while (!API.HasModelLoaded((uint)API.GetHashKey(model)))
            {
                Debug.WriteLine($"Waiting for model {model} to load...");
                await BaseScript.Delay(100);
            }
            return true;
        }

        public static Ped[] GetAllDeadPeds()
        {
            List<Ped> peds = new List<Ped>();
            int entHandle = -1;
            int handle = API.FindFirstPed(ref entHandle);
            Ped ped = (Ped)Entity.FromHandle(entHandle);
            if (ped != null && ped.Exists() && ped.IsDead)
            {
                peds.Add(ped);
            }
            entHandle = -1;
            while (API.FindNextPed(handle, ref entHandle))
            {
                ped = (Ped)Entity.FromHandle(entHandle);
                if (ped != null && ped.Exists() && ped.IsDead)
                {
                    peds.Add(ped);
                }
                entHandle = -1;
            }
            API.EndFindPed(handle);
            return peds.ToArray();
        }

        public static void DisplayMessage(string msg, int time)
        {
            API.ClearPrints();
            API.SetTextEntry_2("STRING");
            API.AddTextComponentString(msg);
            API.DrawSubtitleTimed(time, true);
        }

        public static Vector3 GetNextPositionOnStreet(Vector3 position, int nthClosest = 1, int maxClosest = 40)
        {
            Vector3 outPos = Vector3.Zero;

            for (var i = nthClosest; i < maxClosest; i++)
            {
                API.GetNthClosestVehicleNode(position.X, position.Y, position.Z, i, ref outPos, 1, 3, 0);
                if (!outPos.IsZero && !API.IsPointObscuredByAMissionEntity(outPos.X, outPos.Y, outPos.Z, 5f, 5f, 5f, 0))
                    break;
            }

            return outPos;
        }

        /// <summary>
        /// Shows a notification with a picture.
        /// </summary>
        /// <param name="text">The notification text.</param>
        /// <param name="title">The notification title.</param>
        /// <param name="subtitle">The notification subtitle.</param>
        /// <param name="icon">The icon name to be displayed.</param>
        /// <param name="type">The icon type to be given.</param>
        /// <param name="flash">Whether the notification should flash.</param>
        public static void ShowAdvancedNotification(string text, string title, string subtitle, string icon, int type,
            bool flash = false)
        {
            API.SetNotificationTextEntry("STRING");
            API.AddTextComponentString(text);
            API.SetNotificationMessage(icon, icon, flash, type, title, subtitle);
            API.DrawNotification(false, true);
        }

        public static string GetStreetName(Vector3 location)
        {
            uint streetName = 0, crossingRoad = 0;
            API.GetStreetNameAtCoord(location.X, location.Y, location.Z, ref streetName, ref crossingRoad);
            if (streetName < 1) return "Unknown";
            return API.GetStreetNameFromHashKey(streetName);
        }

        /// <summary>
        /// Sends a chat message to the player.
        /// </summary>
        /// <param name="message">The chat message.</param>
        /// <param name="title">The title.</param>
        public static void SendChatMessage(string message, string title = "PoliceMP")
        {
            BaseScript.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 51, 153, 255 },
                multiline = true,
                args = new[] { title, message }
            });
        }

        public static void SendErrorMessage(string message)
        {
            BaseScript.TriggerEvent("PoliceMP:ShowNotification", "Oops!", message, "error");
        }

        public static void Draw3DText(Vector3 position, string text)
        {
            var screenX = 0f;
            var screenY = 0f;
            var onScreen = API.World3dToScreen2d(position.X, position.Y, position.Z, ref screenX, ref screenY);
            var p = API.GetGameplayCamCoords();
            var distance = API.GetDistanceBetweenCoords(p.X, p.Y, p.Z, position.X, position.Y, position.Z, true);
            var scale = 1 / distance * 2;
            var fov = 1 / API.GetGameplayCamFov() * 100;
            scale *= fov;

            if (onScreen)
            {
                API.SetTextScale(0f, 0.35f);
                API.SetTextFont(0);
                API.SetTextProportional(true);
                API.SetTextColour(255, 255, 255, 255);
                API.SetTextDropshadow(0, 0, 0, 0, 255);
                API.SetTextOutline();
                API.SetTextEntry("STRING");
                API.SetTextCentre(true);
                API.AddTextComponentString(text);
                API.DrawText(screenX, screenY);
            }
        }

        /// <summary>
        /// Finds the nearest place to the position to spawn a vehicle.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The nearest place to spawn a vehicle.</returns>
        public static async Task<Vector3> FindPlaceToSpawnVehicleAsync(Vector3 position, int nthClosest = 50)
        {
            var spawnPos = Vector3.Zero;
            var heading = 0f;
            var unkRef = -1;

            while (!API.GetNthClosestVehicleNodeWithHeading(position.X, position.Y, position.Z, nthClosest,
                ref spawnPos,
                ref heading, ref unkRef, 0, 0, 0))
            {
                await BaseScript.Delay(500);
                nthClosest += 10;
            }

            return spawnPos;
        }

        /// <summary>
        /// Get the vehicle from the number plate text. Only
        /// works for vehicles around the player.
        /// </summary>
        /// <param name="plateText">The number plate text.</param>
        /// <returns>The vehicle or null if none found.</returns>
        public static Vehicle GetVehicleFromPlateText(string plateText)
        {
            return World.GetAllVehicles()
                .ToList()
                .FirstOrDefault(v => v.GetNumberPlateText().Equals(plateText, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the name of the weapon.
        /// </summary>
        /// <param name="name">The weapon.</param>
        /// <returns>The name.</returns>
        public static string GetWeaponDisplayName(string name)
        {
            var underscoreIndex = name.IndexOf("_", StringComparison.Ordinal);
            var displayName = name.Substring(underscoreIndex + 1).ToLower();
            return displayName.FirstCharToUpper();
        }

        /// <summary>
        /// Set the player's relationships with a group.
        /// </summary>
        /// <param name="groupName">The group name.</param>
        /// <param name="pedToPlayer">The relationship from ped to player.</param>
        /// <param name="playerToPed">The relationship from player to ped.</param>
        public static void SetPlayerRelationships(string groupName, int pedToPlayer = 1, int playerToPed = 5)
        {
            var playerGroupHash = (uint)API.GetHashKey("bigpopo");
            var toGroupHash = (uint)API.GetHashKey(groupName);

            API.SetRelationshipBetweenGroups(pedToPlayer, toGroupHash, playerGroupHash);
            API.SetRelationshipBetweenGroups(playerToPed, playerGroupHash, toGroupHash);
        }

        /// <summary>
        /// Gets input from the GTA V native on screen keyboard.
        /// </summary>
        /// <param name="defaultText">The default text to be entered into the textbox.</param>
        /// <returns>The input.</returns>
        public static async Task<string> GetDialogInputAsync(string defaultText = "")
        {
            API.DisplayOnscreenKeyboard(1, "FMMC_MPM_NA", "", defaultText, "", "", "", 40);
            while (API.UpdateOnscreenKeyboard() == 0)
            {
                API.DisableAllControlActions(0);
                await BaseScript.Delay(0);
            }

            return API.GetOnscreenKeyboardResult();
        }
    }
}
