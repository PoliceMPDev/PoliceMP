using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Shared.Constants.States;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Abstraction;
using PoliceMP.Shared.Constants.Decors;
using PoliceMP.Shared.Models;
using PoliceMP.Shared.Enums;

namespace PoliceMP.Core.Client.Extensions
{
    public static class PedExtensions
    {
        public static int DesperateFleeDrivingStyle = 2130442876;

        public static bool IsDrivingEmergencyVehicle(this Ped ped)
        {
            return ped.CurrentVehicle?.GetPedOnSeat(VehicleSeat.Driver) == ped &&
                   ped.CurrentVehicle.ClassType == VehicleClass.Emergency;
        }

        public static bool IsInEmergencyVehicle(this Ped ped)
        {
            return ped.CurrentVehicle != null && ped.CurrentVehicle.ClassType == VehicleClass.Emergency;
        }

        /// <summary>
        /// See https://www.vespura.com/fivem/drivingstyle/ for flags
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="speed"></param>
        /// <param name="drivingStyle"></param>
        public static void TaskDriveWander(this Ped ped, float speed = 15f, int drivingStyle = 447)
        {
            var vehicle = ped.CurrentVehicle;
            if (vehicle == null) return;

            API.TaskVehicleDriveWander(ped.Handle, vehicle.Handle, speed, drivingStyle);
        }

        public static async Task StandStill(this Ped ped)
        {
            var sequence = new TaskSequence();
            await sequence.AddRequired(ped);
            sequence.AddTask.StandStill(-1);
            sequence.Close();
            ped.AlwaysKeepTask = true;
            ped.Task.PerformSequence(sequence);
        }

        public static async Task StandStillFacingPlayer(this Ped ped)
        {
            var sequence = new TaskSequence();
            await sequence.AddRequired(ped);
            sequence.AddTask.TurnTo(Game.PlayerPed);
            sequence.AddTask.StandStill(-1);
            sequence.Close();
            ped.AlwaysKeepTask = true;
            ped.Task.PerformSequence(sequence);
        }

        public static bool IsDrivingTaxi(this Ped ped)
        {
            return ped != null &&
                   ped.CurrentVehicle != null &&
                   ped.CurrentVehicle.Driver == ped &&
                   ped.CurrentVehicle.Model == VehicleHash.Taxi;
        }

        public static void CanPlayAmbientAnims(this Ped ped, bool toggle)
        {
            if (ped == null) return;
            API.SetPedCanPlayAmbientAnims(ped.Handle, toggle);
        }

        public static void TaskUseNearestScenarioToCoord(this Ped ped)
        {
            if (ped == null) return;
            API.TaskUseNearestScenarioToCoord(ped.Handle, ped.Position.X, ped.Position.Y, ped.Position.Z, 500f, -1);
        }

        public static bool GetIsNameKnown(this Ped ped)
        {
            return ped != null && ped.State.Get<bool>(PedStates.NameKnown);
        }

        public static void SetIsNameKnown(this Ped ped, bool toggle)
        {
            if (ped == null) return;
            ped.State.Set<bool>(PedStates.NameKnown, toggle, true);
        }

        public static void SetName(this Ped ped, string fullName)
        {
            if (ped == null) return;
            ped.State.Set<string>(PedStates.FullName, fullName);
        }

        public static VehicleSeat GetLastSeatInVehicle(this Ped ped, Vehicle vehicle)
        {
            if (vehicle == null) return VehicleSeat.None;

            if (API.GetLastPedInVehicleSeat(vehicle.Handle, -1) == ped.Handle)
                return VehicleSeat.Driver;

            if (API.GetLastPedInVehicleSeat(vehicle.Handle, 0) == ped.Handle)
                return VehicleSeat.Passenger;

            if (API.GetLastPedInVehicleSeat(vehicle.Handle, 1) == ped.Handle)
                return VehicleSeat.LeftRear;

            if (API.GetLastPedInVehicleSeat(vehicle.Handle, 2) == ped.Handle)
                return VehicleSeat.RightRear;

            return VehicleSeat.None;
        }

        public static void Stop(this Ped ped, bool toggle = true)
        {
            ped.BlockPermanentEvents = toggle;

            if (toggle)
                ped.Task.StandStill(-1);
        }

        public static void StopAndLookAt(this Ped ped, Entity target)
        {
            ped.BlockPermanentEvents = true;
            ped.Task.LookAt(target);
            ped.Task.TurnTo(target, -1);
        }

        public static void GiveDefaultEquipment(this Ped ped, UserRole currentRole, UserAces userAces)
        {
            ped.Health = 100;
            ped.Armor = userAces.IsAfoTrained ? 0 : 0;
            ped.Armor = userAces.IsAfoTrained ? 0 : 0;

            var isMod = userAces.IsModerator || userAces.IsDeveloper || userAces.IsAdmin;

            switch (currentRole?.Branch)
            {
                case UserBranch.Police:
                    ped.Weapons.Give(WeaponHash.Nightstick, 100, false, true);
                    var SpeedCuff = (WeaponHash)API.GetHashKey("weapon_speedcuffs");
                    API.SetWeaponDamageModifier((uint)SpeedCuff, 0f);
                    ped.Weapons.Give(SpeedCuff, 100, false, true);
                    //Give PC+ Pava spray on Spawn
                    if (userAces.IsWhiteListed)
                    {
						API.ExecuteCommand("pepperspray");
                        API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)API.GetHashKey("weapon_spithood"), 1, false, false);
                        
					}
                    break;
                case UserBranch.Fire:
                    ped.Weapons.Give(WeaponHash.FireExtinguisher, 999999, false, true);
                    ped.Weapons[WeaponHash.FireExtinguisher].InfiniteAmmo = true;
                    ped.Weapons[WeaponHash.FireExtinguisher].InfiniteAmmoClip = true;
                    break;
                case UserBranch.Nhs:
                case UserBranch.Highways:
                case UserBranch.Civ:
                default:
                    break;
            }


            if (isMod || (currentRole?.Branch == UserBranch.Police && userAces.IsTaserTrained))
            {
                ped.Weapons.Give(WeaponHash.StunGun, 100, false, true);
            }


            ped.Weapons.Give(WeaponHash.Flashlight, 100, false, true);
            ped.Weapons.Select(WeaponHash.Unarmed);
        }

        public static PedOutfit FetchCurrentPedOutfit(this Ped ped, string outfitName = "")
        {
            PedOutfit outfit = new PedOutfit()
            {
                Name = outfitName,
                AceGroupsRequired = null,
                MaleOutfit = true,
                HEAD = null,
                BERD = null,
                HAIR = null,
                UPPR = GetCombination(ped, 3),
                LOWR = GetCombination(ped, 4),
                HAND = GetCombination(ped, 5),
                FEET = GetCombination(ped, 6),
                TEEF = GetCombination(ped, 7),
                ACCS = GetCombination(ped, 8),
                TASK = GetCombination(ped, 9),
                DECL = GetCombination(ped, 10),
                JBIB = GetCombination(ped, 11),
                headProp = GetPropCombination(ped, 0),
                EYES = GetPropCombination(ped, 1),
                EARS = GetPropCombination(ped, 2),
                MOUTH = GetPropCombination(ped, 3),
                LEFT_HAND = null,
                RIGHT_HAND = null,
                LEFT_WRIST = null,
                RIGHT_WRIST = null,
                HIP = null,
                LEFT_FOOT = null,
                RIGHT_FOOT = null,
                UNK_604819740 = null,
                UNK_2358626934 = null,
            };
            return outfit;
        }

        public static void SetPedOutfit(this Ped ped, PedOutfit outfit)
        {
            var handle = ped.Handle;

            //Set component variations
            /*API.SetPedComponentVariation(handle, 0, outfit.HEAD.DrawableID, outfit.HEAD.TextureID, outfit.HEAD.PaletteID);
            API.SetPedComponentVariation(handle, 1, outfit.BERD.DrawableID, outfit.BERD.TextureID, outfit.BERD.PaletteID);
            API.SetPedComponentVariation(handle, 2, outfit.HAIR.DrawableID, outfit.HAIR.TextureID, outfit.HAIR.PaletteID);*/

            API.SetPedComponentVariation(handle, 3, outfit.UPPR.DrawableID, outfit.UPPR.TextureID, outfit.UPPR.PaletteID);
            API.SetPedComponentVariation(handle, 4, outfit.LOWR.DrawableID, outfit.LOWR.TextureID, outfit.LOWR.PaletteID);
            API.SetPedComponentVariation(handle, 5, outfit.HAND.DrawableID, outfit.HAND.TextureID, outfit.HAND.PaletteID);
            API.SetPedComponentVariation(handle, 6, outfit.FEET.DrawableID, outfit.FEET.TextureID, outfit.FEET.PaletteID);
            API.SetPedComponentVariation(handle, 7, outfit.TEEF.DrawableID, outfit.TEEF.TextureID, outfit.TEEF.PaletteID);
            API.SetPedComponentVariation(handle, 8, outfit.ACCS.DrawableID, outfit.ACCS.TextureID, outfit.ACCS.PaletteID);
            API.SetPedComponentVariation(handle, 9, outfit.TASK.DrawableID, outfit.TASK.TextureID, outfit.TASK.PaletteID);
            API.SetPedComponentVariation(handle, 10, outfit.DECL.DrawableID, outfit.DECL.TextureID, outfit.DECL.PaletteID);
            API.SetPedComponentVariation(handle, 11, outfit.JBIB.DrawableID, outfit.JBIB.TextureID, outfit.JBIB.PaletteID);

            //Set prop variaitons
            API.SetPedPropIndex(handle, 0, outfit.headProp.DrawableID, outfit.headProp.TextureID, true);
            API.SetPedPropIndex(handle, 1, outfit.EYES.DrawableID, outfit.EYES.TextureID, true);
            API.SetPedPropIndex(handle, 2, outfit.EARS.DrawableID, outfit.EARS.TextureID, true);
            API.SetPedPropIndex(handle, 3, outfit.MOUTH.DrawableID, outfit.MOUTH.TextureID, true);
        }

        private static Component GetCombination(Ped targetPed, int id)
        {
            int ped = targetPed.Handle;
            Component component = new Component()
            {
                DrawableID = API.GetPedDrawableVariation(ped, id),
                TextureID = API.GetPedTextureVariation(ped, id),
                PaletteID = API.GetPedPaletteVariation(ped, id)
            };
            return component;
        }

        private static Component GetPropCombination(Ped targetPed, int id)
        {
            int ped = targetPed.Handle;
            Component component = new Component()
            {
                DrawableID = API.GetPedPropIndex(ped, id),
                TextureID = API.GetPedPropTextureIndex(ped, id),
                PaletteID = 0
            };
            return component;
        }

        public static Ped GetClosestPed(this Ped otherPed)
        {
            var peds = World.GetAllPeds();
            Ped chosenPed = new Ped(0);

            float distance = float.MaxValue;
            for (int i = 0; i < peds.Length; i++)
            {
                var ped = peds[i];
                var testDistance = World.GetDistance(otherPed.Position, ped.Position);
                if (testDistance < distance)
                {
                    chosenPed = ped;
                    distance = testDistance;
                }
            }

            return chosenPed;
        }

        public static Player GetPlayer(this Ped ped)
        {
            return new Player(API.NetworkGetPlayerIndexFromPed(ped.Handle));
        }

        public static bool IsTaskActive(this Ped ped, TaskType task) => API.GetIsTaskActive(ped.Handle, (int)task);

        public static TaskType[] GetActiveTasks(this Ped ped)
        {
            List<TaskType> active = new List<TaskType>(1024);
            for (int i = 0; i < 1024; i++)
            {
                if (API.GetIsTaskActive(ped.Handle, i))
                {
                    active.Add((TaskType)i);
                }
            }

            return active.ToArray();
        }

        public static async Task NetworkFadeOut(this Ped ped, bool normal = true, bool slow = false)
        {
            API.NetworkFadeOutEntity(ped.Handle, normal, slow);
            while (API.NetworkIsEntityFading(ped.Handle))
            {
                await BaseScript.Delay(0);
            }
        }

        public static async Task NetworkFadeIn(this Ped ped, bool slow = false)
        {
            API.NetworkFadeInEntity(ped.Handle, slow);
            while (API.NetworkIsEntityFading(ped.Handle))
            {
                await BaseScript.Delay(0);
            }
        }
    }
}