using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Shared;
using System;
using System.Threading.Tasks;
using PoliceMP.Core.Client.Scripts;

namespace PoliceMP.Client.Services
{
    public class AnimationService : IAnimationService
    {
        public async Task Breathalyser(Ped ped)
        {
            var entityPos = API.GetEntityCoords(ped.Handle, true);

            var breathalyser = API.CreateObject(API.GetHashKey("prop_inhaler_01"), entityPos.X, entityPos.Y,
                entityPos.Z, true, true, true);

            var boneIndex = API.GetPedBoneIndex(ped.Handle, 64016);

            API.AttachEntityToEntity(breathalyser, ped.Handle, boneIndex, 0.08f, 0f, 0f, 180f, 115f, 10f, true, true,
                false, false, 1, true);

            await Play(ped, "weapons@pistol_1h@gang", "aim_med_loop", flag: 50);

            await Script.Delay(2000);

            API.DetachEntity(breathalyser, true, true);
            API.DeleteEntity(ref breathalyser);
        }

        public async Task HandsUp(Ped ped, bool standingOnly = false)
        {
            if (!standingOnly)
            {
                var randomNum = AppRandom.Next(100);

                if (randomNum < 25)
                    await Play(ped, "misscarsteal2chad_garage",
                        "chad_parking_garage_handsuploop_chad",
                        duration: -1, flag: 50);
                else if (randomNum < 50)
                    await Play(ped, "busted", "idle_b", duration: -1, flag: 50);
                else if (randomNum < 75)
                    await Play(ped, "random@mugging5", "ig_2_guy_handsup_loop", duration: -1,
                        flag: 50);
                else
                    await Play(ped, "mp_pol_bust_out", "guard_handsup_loop", duration: -1,
                        flag: 50);
            }
            else
            {
                await Play(ped, "busted", "idle_b", duration: -1, flag: 50);
            }

            ped.Task.StandStill(-1);
        }

        public async Task HowYouDoing(Ped ped)
        {
            await Play(ped, "special_ped@baygor@michael_2@michael_2c", "hey_how_you_doing2_2");
        }

        public async Task ShowId(Ped ped)
        {
            await Play(ped, "misslester1b", "michael_phone_detonate_press");
        }

        public async Task GesturePoint(Ped ped)
        {
            await Play(ped, "gestures@f@standing@casual", "gesture_point",
                duration: -1,
                flag: (int)AnimationFlags.None);
        }

        public async Task FacePalm(Ped ped)
        {
            await Play(ped, "anim@mp_player_intcelebrationfemale@face_palm", "face_palm");
        }

        public void Clear(Ped ped)
        {
            ped.Task.ClearAll();
            ped.Task.ClearAnimation("misscarsteal2chad_garage", "chad_parking_garage_handsuploop_chad");
            ped.Task.ClearAnimation("busted", "idle_b");
            ped.Task.ClearAnimation("random@mugging5", "ig_2_guy_handsup_loop");
            ped.Task.ClearAnimation("mp_pol_bust_out", "guard_handsup_loop");
        }

        public async Task Play(Ped ped,
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
                return;

            API.RequestAnimDict(dict);
            var now = DateTime.Now;
            while (!API.HasAnimDictLoaded(dict))
            {
                if ((DateTime.Now - now).TotalSeconds >= 2) break;
                await Script.Delay(1);
            }

            API.TaskPlayAnim(ped.Handle, dict, name, blendInSpeed, blendOutSpeed, duration, flag, playbackRate, lockX,
                lockY, lockZ);
        }
    }
}