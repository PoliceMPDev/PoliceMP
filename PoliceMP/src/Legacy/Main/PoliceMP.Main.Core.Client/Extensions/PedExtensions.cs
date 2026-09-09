using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;
using PoliceMP.Main.Core.Shared;

namespace PoliceMP.Main.Core.Client.Extensions
{
    public static class PedExtensions
    {
        public static void SetInteractable(this Ped ped, bool isInteractable)
        {
            ped.SetBoolDecor("IsSelected", !isInteractable);
        }

        public static bool IsInteractable(this Ped ped)
        {
            if (API.IsPedAPlayer(ped.Handle)) return false;

            // Make sure ped isn't player, cop, swat, animal, or army
            // https://runtime.fivem.net/doc/natives/#_0xC33AB876A77F8164
            var hash = API.GetPedRelationshipGroupHash(ped.Handle);
            if (hash == API.GetHashKey("unselectable"))
                return false;

            var pedType = API.GetPedType(ped.Handle);
            if (pedType == 1 || pedType == 6 || pedType == 27 || pedType == 28 || pedType == 29)
                return false;

            return !ped.HasDecor("IsSelected") || !ped.GetBoolDecor("IsSelected");
        }

        /// <summary>
        /// Makes it easier to play anims so we don't have to dupe code for
        /// the RequestAnimDict
        /// </summary>
        public static async Task<bool> PlayAnimAsync(this Ped ped,
            string dict,
            string name,
            float blendInSpeed = 8f,
            float blendOutSpeed = -8f,
            int duration = 5000,
            int flag = 49,
            float playbackRate = 0,
            bool lockX = false,
            bool lockY = false,
            bool lockZ = false)
        {
            if (!API.DoesEntityExist(ped.Handle) || API.IsPedDeadOrDying(ped.Handle, true) || !API.DoesAnimDictExist(dict))
                return false;

            API.RequestAnimDict(dict);
            if (!API.HasAnimDictLoaded(name))
                await BaseScript.Delay(100);

            API.TaskPlayAnim(ped.Handle, dict, name, blendInSpeed, blendOutSpeed, duration, flag, playbackRate, lockX,
                lockY, lockZ);

            return true;
        }

        public static void Stop(this Ped ped, bool toggle = true)
        {
            ped.BlockPermanentEvents = toggle;

            if (toggle)
                ped.Task.StandStill(-1);
        }

        /// <summary>
        ///     Makes the entity play the breathalyser anim.
        /// </summary>
        /// <param name="entityId">The entity ID.</param>
        public static async Task PlayBreathalyserAnimAsync(this Ped ped)
        {
            var entityPos = API.GetEntityCoords(ped.Handle, true);

            var breathalyser = API.CreateObject(API.GetHashKey("prop_inhaler_01"), entityPos.X, entityPos.Y,
                entityPos.Z, true, true, true);
            var boneIndex = API.GetPedBoneIndex(ped.Handle, 64016);
            API.AttachEntityToEntity(breathalyser, ped.Handle, boneIndex, 0.08f, 0f, 0f, 180f, 115f, 10f, true, true,
                false, false, 1, true);

            await ped.PlayAnimAsync("weapons@pistol_1h@gang", "aim_med_loop", flag: 50);

            await BaseScript.Delay(2000);

            API.DetachEntity(breathalyser, true, true);
            API.DeleteEntity(ref breathalyser);
        }

        public static async Task PlayGrabPedAnimAsync(this Ped ped, float blendSpeed)
        {
            await ped.PlayAnimAsync("rcmnigel1d", "base_club_shoulder", blendSpeed, flag: 50, duration: -1);
        }

        public static async Task PlayHandsupAnimAsync(this Ped ped, bool standingOnly = false)
        {
            if (!standingOnly)
            {
                var randomNum = PoliceMpRandom.Next(100);

                if (randomNum < 25)
                    await ped.PlayAnimAsync("misscarsteal2chad_garage",
                        "chad_parking_garage_handsuploop_chad",
                        duration: -1, flag: 50);
                else if (randomNum < 50)
                    await ped.PlayAnimAsync("busted", "idle_b", duration: -1, flag: 50);
                else if (randomNum < 75)
                    await ped.PlayAnimAsync("random@mugging5", "ig_2_guy_handsup_loop", duration: -1,
                        flag: 50);
                else
                    await ped.PlayAnimAsync("mp_pol_bust_out", "guard_handsup_loop", duration: -1,
                        flag: 50);
            }
            else
            {
                await ped.PlayAnimAsync("busted", "idle_b", duration: -1, flag: 50);
            }

            ped.Task.StandStill(-1);
        }

        public static void ClearHandsupAnim(this Ped ped)
        {
            ped.Task.ClearAnimation("misscarsteal2chad_garage", "chad_parking_garage_handsuploop_chad");
            ped.Task.ClearAnimation("busted", "idle_b");
            ped.Task.ClearAnimation("random@mugging5", "ig_2_guy_handsup_loop");
            ped.Task.ClearAnimation("mp_pol_bust_out", "guard_handsup_loop");
        }

        public static void Follow(this Ped ped, Ped pedToFollow)
        {
            var group = API.GetPlayerGroup(pedToFollow.Handle);

            ped.Task.ClearAll();

            API.SetPedAsGroupMember(ped.Handle, group);

            API.SetPedCanTeleportToGroupLeader(ped.Handle, API.GetPlayerGroup(Game.PlayerPed.Handle), true);
        }

        public static void Unfollow(this Ped ped, Ped pedToUnfollow)
        {
            API.SetPedAsGroupMember(ped.Handle, -1);

            API.RemovePedFromGroup(ped.Handle);

            ped.Task.StandStill(-1);
        }

        public static void Kneel(this Ped ped)
        {
            API.TaskSetBlockingOfNonTemporaryEvents(ped.Handle, true);
            API.SetPedFleeAttributes(ped.Handle, 0, false);
            API.SetPedCombatAttributes(ped.Handle, 17, true);
            API.SetBlockingOfNonTemporaryEvents(ped.Handle, true);
            ped.BlockPermanentEvents = true;
            ped.Task.PlayAnimation("random@arrests", "kneeling_arrest_idle", 10f, -1, AnimationFlags.StayInEndFrame);
        }

        public static void LieDown(this Ped ped)
        {
            ped.BlockPermanentEvents = true;

            API.TaskSetBlockingOfNonTemporaryEvents(ped.Handle, true);
            API.SetPedFleeAttributes(ped.Handle, 0, false);
            API.SetPedCombatAttributes(ped.Handle, 17, true);
            API.SetBlockingOfNonTemporaryEvents(ped.Handle, true);

            if (API.IsPedUsingScenario(ped.Handle, "WORLD_HUMAN_SUNBATHE"))
                return;

            API.TaskStartScenarioInPlace(ped.Handle, "WORLD_HUMAN_SUNBATHE", 0, true);
        }

        public static void StandUp(this Ped ped)
        {
            ped.Task.PlayAnimation("random@arrests", "kneeling_arrest_get_up");
        }

        public static void FacePed(this Ped ped, Ped pedToFace)
        {
            API.SetEntityHeading(ped.Handle, ped.GetHeadingToEntity(pedToFace));
        }

        /// <summary>
        ///     Makes the ped drunk by giving them the drunk anim set.
        /// </summary>
        /// <param name="entityId">The ped entity Id.</param>
        public static async Task SetDrunkAsync(this Ped ped)
        {
            API.RequestAnimSet("move_m@drunk@verydrunk");
            var count = 0;
            while (!API.HasAnimSetLoaded("move_m@drunk@verydrunk"))
            {
                await BaseScript.Delay(100);
                if (count >= 20)
                    return;
                count++;
            }


            API.SetPedMovementClipset(ped.Handle, "move_m@drunk@verydrunk", 80000);
        }
    }
}