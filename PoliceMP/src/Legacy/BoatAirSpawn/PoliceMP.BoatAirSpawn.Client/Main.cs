using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace PoliceMP.BoatAirSpawn.Client
{
    public class Main : BaseScript
    {
        private bool NPASStatus = false;
        private bool MPUStatus = false;
        private List<MPUData> mpudata = new List<MPUData>();
        private List<NPASData> npasdata = new List<NPASData>();

        private bool recievedWhitelist = false;
        private bool spawnLocker = false;

        private float scale = 0.1F * API.GetGameplayCamFov();
        private uint boatuint = 1033245328;
        private uint heliuint = 353883353;

        public Main()
        {
            //Docs at heliport
            MPUData mpu1 = new MPUData(-759, -1486, 4, -797, -1502, 1);
            mpudata.Add(mpu1);

            //Add NPAS at Police Station
            NPASData npas1 = new NPASData(459, -983, 43, 448, -981, 43);
            npasdata.Add(npas1);

            //Heliport
            NPASData npas2 = new NPASData(-695, -1451, 4, -702, -1445, 5);
            npasdata.Add(npas2);
        }

        [EventHandler("PoliceMP:recieveSpecWhitelisting")]
        private void recieveSpecWhitelisting(bool afo, bool rpu, bool cid, bool npas, bool mpu, bool admin, bool dog, bool nhs, bool fire, bool mod, bool whitelisted)
        {
            NPASStatus = npas;
            MPUStatus = mpu;

            if (admin)
            {
                NPASStatus = true;
                MPUStatus = true;
            }

            recievedWhitelist = true;
            Blips();
        }

        private void Blips()
        {
            if (MPUStatus)
            {
                foreach(MPUData md in mpudata)
                {
                    int bid = API.AddBlipForCoord(md.xmarker, md.ymarker, md.zmarker);
                    API.SetBlipSprite(bid, 356);
                    API.SetBlipColour(bid, 3);
                    API.BeginTextCommandSetBlipName("STRING");
                    API.AddTextComponentString("MPU Base");
                    API.EndTextCommandSetBlipName(bid);
                    API.SetBlipAsShortRange(bid, true);
                }
            }

            if (NPASStatus)
            {
                foreach (NPASData nd in npasdata)
                {
                    int bid = API.AddBlipForCoord(nd.xmarker, nd.ymarker, nd.zmarker);
                    API.SetBlipSprite(bid, 360);
                    API.BeginTextCommandSetBlipName("STRING");
                    API.AddTextComponentString("NPAS Base");
                    API.EndTextCommandSetBlipName(bid);
                    API.SetBlipColour(bid, 3);
                    API.SetBlipAsShortRange(bid, true);
                }
            }
        }

        [Tick]
        private async Task MPU()
        {
            if (!recievedWhitelist) { return; }
            if (!MPUStatus) { return; }
            if (mpudata.Count == 0) { return; }
            if (spawnLocker) { return; }

            foreach(MPUData md in mpudata)
            {
                float distance = API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, md.xmarker, md.ymarker, md.zmarker, true);

                if(distance < 20.0f)
                {
                    API.DrawMarker(1, md.xmarker, md.ymarker, md.zmarker, 0, 0, 0, 0, 0, 0, 2F, 2F, 3F, 0, 255, 255, 100, false, true, 2, false, null, null, false);
                }

                if (distance < 10.0F)
                {
                    API.SetTextScale(0.1F * scale, 0.1F * scale);
                    API.SetTextFont(4);
                    API.SetTextProportional(true);
                    API.SetTextColour(250, 250, 250, 255);
                    API.SetTextDropshadow(1, 1, 1, 1, 255);
                    API.SetTextEdge(2, 0, 0, 0, 255);
                    API.SetTextDropShadow();
                    API.SetTextOutline();
                    API.SetTextEntry("STRING");
                    API.SetTextCentre(true);
                    API.AddTextComponentString("Spawn Boat");
                    API.SetDrawOrigin(md.xmarker, md.ymarker, md.zmarker + 2F, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();
                }

                if (distance < 3.0f)
                {
                    Screen.DisplayHelpTextThisFrame("Press ~y~E ~w~to spawn a boat.");
                    if (Game.IsControlPressed(0, Control.Pickup))
                    {
                        //Dinghy
                        await LoadModel(boatuint);
                        Vehicle vehObj = new Vehicle(API.CreateVehicle(boatuint, md.xspawn, md.yspawn, md.zspawn, 0, true, true))
                        {
                            IsPersistent = true,
                            IsStolen = false,
                            IsWanted = false,
                            NeedsToBeHotwired = false,
                            PreviouslyOwnedByPlayer = true
                        };
                        API.PlaceObjectOnGroundProperly(vehObj.Handle);
                        spawnLocker = true;
                    }
                }
            }
        }

        [Tick]
        private async Task NPAS()
        {
            if (!recievedWhitelist) { return; }
            if (!NPASStatus) { return; }
            if (npasdata.Count == 0) { return; }
            if (spawnLocker) { return; }

            foreach (NPASData nd in npasdata)
            {
                float distance = API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, nd.xmarker, nd.ymarker, nd.zmarker, true);

                if (distance < 20.0f)
                {
                    API.DrawMarker(1, nd.xmarker, nd.ymarker, nd.zmarker, 0, 0, 0, 0, 0, 0, 2F, 2F, 3F, 0, 255, 255, 100, false, true, 2, false, null, null, false);
                }

                if (distance < 10.0F)
                {
                    API.SetTextScale(0.1F * scale, 0.1F * scale);
                    API.SetTextFont(4);
                    API.SetTextProportional(true);
                    API.SetTextColour(250, 250, 250, 255);
                    API.SetTextDropshadow(1, 1, 1, 1, 255);
                    API.SetTextEdge(2, 0, 0, 0, 255);
                    API.SetTextDropShadow();
                    API.SetTextOutline();
                    API.SetTextEntry("STRING");
                    API.SetTextCentre(true);
                    API.AddTextComponentString("Spawn Helicopter");
                    API.SetDrawOrigin(nd.xmarker, nd.ymarker, nd.zmarker + 2F, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();
                }

                if (distance < 3.0f)
                {
                    Screen.DisplayHelpTextThisFrame("Press ~y~E ~w~to spawn a helicopter.");
                    if (Game.IsControlPressed(0, Control.Pickup))
                    {
                        //Dinghy
                        await LoadModel(heliuint);
                        Vehicle vehObj = new Vehicle(API.CreateVehicle(heliuint, nd.xspawn, nd.yspawn, nd.zspawn, 0, true, true))
                        {
                            IsPersistent = true,
                            IsStolen = false,
                            IsWanted = false,
                            NeedsToBeHotwired = false,
                            PreviouslyOwnedByPlayer = true
                        };
                        API.PlaceObjectOnGroundProperly(vehObj.Handle);
                        spawnLocker = true;
                    }
                }
            }
        }

        [Tick]
        private async Task SpawnLockManager()
        {
            if (!spawnLocker) { return; }
            await Delay(5000);
            spawnLocker = false;
        }

        public static async Task<bool> LoadModel(uint modeluint)
        {
            Debug.WriteLine($"Loading Model {modeluint}");
            // This won't trip-up on addon models, I checked
            if (!API.IsModelInCdimage(modeluint))
            {
                Debug.WriteLine($"ERROR! This model, {modeluint} is not valid! This code will never work.");
            }
            else
            {
                API.RequestModel(modeluint);
                while (!API.HasModelLoaded(modeluint))
                {
                    Debug.WriteLine($"Waiting for model {modeluint} to load...");
                    await BaseScript.Delay(100);
                }
            }
            return true;
        }
    }
}
