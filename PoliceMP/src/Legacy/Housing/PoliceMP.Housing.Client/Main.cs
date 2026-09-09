using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Newtonsoft.Json;
using PoliceMP.Main.Core.Client;

namespace PoliceMP.Housing.Client
{
    public class Main : BaseScript
    {
        public Main() { }

        //Lists
        private List<Interior> Interiors = new List<Interior>();
        private List<House> Houses = new List<House>();

        private List<int> PlayersInMyHouse = new List<int>();
        private List<int> EntitiesInMyHouseNetIDs = new List<int>();

        private List<int> HiddenPlayerIds = new List<int>();
        private List<int> HiddenEntities = new List<int>();

        //Globals
        private float scale = 0.1F * API.GetGameplayCamFov();
        private bool inHouse = false;
        private int currentHouseID, currentInteriorID;

        private int requestingToEnterHouse, requestingToEnterInterior;

        //Retrieve the json lists on first spawn
        private bool firstSpawn = true;
        [EventHandler("playerSpawned")]
        private void OnPlayerSpawned()
        {
            if (!firstSpawn) { return; }
            Debug.WriteLine("Client requesting json");
            TriggerServerEvent("PoliceMPHousing:ClientRequestingData");
        }

        //Retrieve interiors
        [EventHandler("PoliceMPHousing:SendingClientInteriors")]
        private void RecieveingInteriors(string _json)
        {
            lock (Interiors)
            {
                var obj = JsonConvert.DeserializeObject<List<Interior>>(_json);
                Interiors = obj;
                Debug.WriteLine("Total Interiors: " + Interiors.Count());
            }
        }

        //Retrieve houses
        [EventHandler("PoliceMPHousing:SendingClientHouses")]
        private void RecieveingHouses(string _json)
        {
            lock (Houses)
            {
                var obj = JsonConvert.DeserializeObject<List<House>>(_json);
                Houses = obj;
                Debug.WriteLine("Total Houses: " + Houses.Count());
            }
        }

        //Handle exterior markers
        [Tick]
        private Task ExteriorHandler()
        {
            if (inHouse) { return Task.FromResult(0); }
            if (Houses == null) { return Task.FromResult(0); }
            if (Houses.Count == 0) { return Task.FromResult(0); }

            foreach (House h in Houses)
            {
                float distance = API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, h.FrontDoorX, h.FrontDoorY, h.FrontDoorZ, true);

                if (distance < 3.0f)
                {
                    API.DrawMarker(1, h.FrontDoorX, h.FrontDoorY, h.FrontDoorZ - 1, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 20, 20, 200, 50, false, true, 2, false, null, null, false);

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
                    API.AddTextComponentString(h.HouseNameOrNumber + "\n" + h.HouseStreet);
                    API.SetDrawOrigin(h.FrontDoorX, h.FrontDoorY, h.FrontDoorZ + 1F, 0);
                    API.DrawText(0, 0);
                    API.ClearDrawOrigin();

                    Screen.DisplayHelpTextThisFrame("Press ~y~Y ~w~to enter this address.");

                    if (API.IsInputDisabled(2) && Game.IsControlJustPressed(0, (CitizenFX.Core.Control)246))
                    {
                        //TODO: Fire an event to check the house lock status


                        requestingToEnterHouse = h.HouseID;
                        requestingToEnterInterior = h.InteriorID;
                        RequestPlayerInHouse(h.HouseID);
                    }
                }
            }

            return Task.FromResult(0);
        }

        //Handle interior markers
        [Tick]
        private async Task InteriorHandler()
        {
            if (!inHouse) { return; }
            if (Interiors == null) { return; }
            if (Interiors.Count() == 0) { return; }

            foreach (Interior i in Interiors)
            {
                if (i.ID == currentInteriorID)
                {
                    float distance = API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, i.XFront, i.YFront, i.ZFront, true);

                    if (distance < 3.0f)
                    {
                        API.DrawMarker(1, i.XFront, i.YFront, i.ZFront - 1f, 0, 0, 0, 0, 0, 0, 1F, 1F, 2F, 20, 20, 200, 50, false, true, 2, false, null, null, false);

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
                        API.AddTextComponentString("Exit House");
                        API.SetDrawOrigin(i.XFront, i.YFront, i.ZFront + 1F, 0);
                        API.DrawText(0, 0);
                        API.ClearDrawOrigin();

                        Screen.DisplayHelpTextThisFrame("Press ~y~Y ~w~to exit this address.");

                        if (API.IsInputDisabled(2) && Game.IsControlJustPressed(0, (CitizenFX.Core.Control)246))
                        {
                            await ExitAddress();
                        }
                    }
                }
            }
            API.SetPedDensityMultiplierThisFrame(0f);
            API.SetScenarioPedDensityMultiplierThisFrame(0f, 0f);
            API.SetParkedVehicleDensityMultiplierThisFrame(0f);
            API.SetParkedVehicleDensityMultiplierThisFrame(0f);
            API.SetVehicleDensityMultiplierThisFrame(0f);
        }

        [EventHandler("PoliceMPHousing:GrantedToEnterHouse")]
        //Enter a provided address
        private void EnterAddress()
        {
            int _houseID = requestingToEnterHouse;
            int _InteriorID = requestingToEnterInterior;

            if (inHouse) { return; }

            foreach (Interior i in Interiors)
            {
                if (i.ID == _InteriorID)
                {
                    API.DoScreenFadeOut(1000);
                    API.NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);
                    API.SetEntityCoords(Game.PlayerPed.Handle, i.XFront, i.YFront, i.ZFront, false, false, false, false);
                    API.SetEntityRotation(Game.PlayerPed.Handle, 0, 0, i.HFront, 1, true);
                    API.NetworkFadeInEntity(Game.PlayerPed.Handle, true);
                    API.DoScreenFadeIn(1000);
                    inHouse = true;
                    currentHouseID = _houseID;
                    currentInteriorID = i.ID;
                    ShowNotification("Putting you in hoose mate");
                }
            }
            if (!inHouse) { ShowNotification("Could not find house, please screenshot location and provide following number to development team: " + _houseID + ":" + _InteriorID); }
        }

        //Exit provided address
        private async Task ExitAddress()
        {
            foreach (House h in Houses)
            {
                if (h.HouseID == currentHouseID)
                {
                    ShowNotification("Detected in house " + h.HouseID + ":" + currentHouseID + "Returning to X:" + h.FrontDoorX + " Y:" + h.FrontDoorY + " Z:" + h.FrontDoorZ);
                    API.DoScreenFadeOut(1000);
                    API.NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);
                    API.SetEntityCoords(Game.PlayerPed.Handle, h.FrontDoorX, h.FrontDoorY, h.FrontDoorZ, false, false, false, false);
                    API.NetworkFadeInEntity(Game.PlayerPed.Handle, true);
                    API.DoScreenFadeIn(1000);
                    RequestPlayerOutHouse();
                    RevealYourSecrets();
                    await HoldThenResetOutside();
                }
            }
        }

        [Command("stuckinhouse")]
        private async void StuckInHouse()
        {
            ShowNotification("Could not exit house properly, please screenshot location and provide following number to development team: " + currentHouseID + ":" + currentInteriorID);
            API.NetworkConcealPlayer(Game.Player.Handle, false, false);
            API.DoScreenFadeOut(1000);
            API.NetworkFadeOutEntity(Game.PlayerPed.Handle, true, false);
            API.SetEntityCoords(Game.PlayerPed.Handle, 427, -980, 30, false, false, false, false);
            API.NetworkFadeInEntity(Game.PlayerPed.Handle, true);
            API.DoScreenFadeIn(1000);
            RequestPlayerOutHouse();
            RevealYourSecrets();
            await HoldThenResetOutside();
        }

        private void RequestPlayerInHouse(int _HouseID)
        {
            TriggerServerEvent("PoliceMPHousing:PlayerIsEnteringHouse", _HouseID, Game.Player.Handle);
        }

        private void RequestPlayerOutHouse()
        {
            TriggerServerEvent("PoliceMPHousing:PlayerIsLeavingHouse", currentHouseID, Game.Player.Handle);
        }

        [EventHandler("PoliceMPHousing:CurrentHouseUpdate")]
        private void CurrentHouseUpdate(int _houseID, string _playersInHouse, string _entitesInHouseNetIDs)
        {
            lock (PlayersInMyHouse)
            {
                Debug.WriteLine("Housing update recieved");
                PlayersInMyHouse = JsonConvert.DeserializeObject<List<int>>(_playersInHouse);
                for (int i = 0; i <= 256; i++)
                {

                    if (!API.NetworkIsPlayerActive(i)) { continue; }
                    //Player is in the house, don't hide them
                    if (PlayersInMyHouse.Contains(i))
                    {
                        API.NetworkConcealPlayer(i, false, false);
                    }
                    else
                    {
                        //Player assumed not in house in the house
                        API.NetworkConcealPlayer(i, true, true);
                        HiddenPlayerIds.Add(i);
                    }

                }
            }
            lock (EntitiesInMyHouseNetIDs)
            {
                EntitiesInMyHouseNetIDs = JsonConvert.DeserializeObject<List<int>>(_entitesInHouseNetIDs);
                Ped[] peds = World.GetAllPeds();
                foreach (var ped in peds)
                {
                    if (ped.IsPlayer) { continue; }
                    var pedHandle = ped.Handle;
                    if (!EntitiesInMyHouseNetIDs.Contains(ped.NetworkId))
                    {
                        API.NetworkConcealEntity(pedHandle, true);
                        HiddenEntities.Add(pedHandle);
                    }
                    else
                    {
                        API.NetworkConcealEntity(pedHandle, false);
                    }
                }
            }
        }

        //Returns the handle of a players character from their player object
        private int RetrievePlayerCharacter(Player _pl)
        {
            foreach (Player p in Players)
            {
                if (p.Handle == API.GetPlayerFromServerId(_pl.Handle))
                {
                    return p.Character.Handle;
                }
            }
            return -1;
        }

        //Re enables everyone that was hidden now being visible on house leave
        private void RevealYourSecrets()
        {
            foreach (int plID in HiddenPlayerIds)
            {
                API.NetworkConcealPlayer(plID, false, false);
            }
            foreach (int eID in HiddenEntities)
            {
                API.NetworkConcealEntity(eID, false);
            }
            HiddenPlayerIds.Clear();
            HiddenEntities.Clear();
        }

        private async Task HoldThenResetOutside()
        {
            await Delay(3000);
            inHouse = false;
            currentHouseID = -1;
            currentInteriorID = -1;
        }


        [Command("showinterior")]
        private void ShowInterior(string[] args)
        {
            if (args.Length != 2) { return; }
            if (args[0] != "cock") { return; }

            foreach (Interior i in Interiors)
            {
                if (i.ID != Convert.ToInt32(args[1])) { continue; }
                API.SetEntityCoords(Game.PlayerPed.Handle, i.XFront, i.YFront, i.ZFront, false, false, false, false);
            }
        }

        [EventHandler("PoliceMPHousingClient:RequestPedExtractionLocation")]
        private void RequestPedExtractionLocation(int _entityId)
        {
            Debug.WriteLine("RequestPedExtractionLocation");
            if (inHouse)
            {
                foreach (var house in Houses)
                {
                    if (house.HouseID == currentHouseID)
                    {
                        TriggerEvent("PoliceMP:ExtractPedOutsideHouse", house.FrontDoorX, house.FrontDoorY, house.FrontDoorZ);

                        // Ensure ped is removed from house
                        TriggerServerEvent("PoliceMPHousing:DeregisterEntityInHouse", currentHouseID, _entityId);

                        return;
                    }
                }

            }
            else
            {
                ShowNotification("Must be inside a house to move a ped out of a house!");
            }

        }

        [Tick]
        private async Task LockDoors()
        {
            List<Vector3> doorPositions = new List<Vector3>();

            //Franklin, forum drive
            doorPositions.Add(new Vector3(-14.219466209412f, -1441.1086425781f, 31.101551055908f));


            //Michaels house
            doorPositions.Add(new Vector3(-816.38494873047f, 178.2991027832f, 72.226425170898f));
            doorPositions.Add(new Vector3(-815.23590087891f, 186.07308959961f, 72.478706359863f));
            doorPositions.Add(new Vector3(-795.53271484375f, 177.69175720215f, 72.834777832031f));
            doorPositions.Add(new Vector3(-793.82885742188f, 181.53060913086f, 72.834762573242f));

            //Garment Factory
            doorPositions.Add(new Vector3(717.94439697266f, -975.49859619141f, 24.91400718689f));

            //Coal Factory
            doorPositions.Add(new Vector3(1082.8779296875f, -1974.8763427734f, 31.473997116089f));
            doorPositions.Add(new Vector3(1065.6739501953f, -2005.3453369141f, 32.128944396973f));
            doorPositions.Add(new Vector3(1085.1104736328f, -2019.2902832031f, 41.522914886475f));

            //Slaughterhouse
            doorPositions.Add(new Vector3(961.87493896484f, -2184.8562011719f, 30.478820800781f));
            doorPositions.Add(new Vector3(961.82989501953f, -2186.4479980469f, 30.52195930481f));
            doorPositions.Add(new Vector3(962.85168457031f, -2105.8576660156f, 31.479602813721f));


            //Lesters house
            doorPositions.Add(new Vector3(1274.5766601563f, -1720.3275146484f, 54.771450042725f));

            //Flat
            doorPositions.Add(new Vector3(-107.2057800293f, -8.1637029647827f, 70.525115966797f));

            //Store Room
            doorPositions.Add(new Vector3(242.15371704102f, 360.66558837891f, 105.73797607422f));

            //Bar
            doorPositions.Add(new Vector3(-564.40631103516f, 276.4758605957f, 83.134239196777f));
            doorPositions.Add(new Vector3(-562.20440673828f, 293.91000366211f, 87.624465942383f));

            //Lifeinvader
            doorPositions.Add(new Vector3(-1045.79296875f, -230.60984802246f, 39.014621734619f));
            doorPositions.Add(new Vector3(-1082.3197021484f, -259.78182983398f, 37.796894073486f));

            //Trevor Flat
            doorPositions.Add(new Vector3(-1150.2248535156f, -1521.4382324219f, 10.632719039917f));

            //Franklin - Big house
            doorPositions.Add(new Vector3(8.1144571304321f, 539.01141357422f, 176.02821350098f));

            //24/7 - Market
            doorPositions.Add(new Vector3(1166.5545654297f, 2703.6145019531f, 38.179069519043f));

            //Bank
            doorPositions.Add(new Vector3(1175.2421875f, 2703.7197265625f, 38.172695159912f));

            //Low class Clothes
            doorPositions.Add(new Vector3(1198.0799560547f, 2703.3835449219f, 38.2340965271f));

            //ONiels Farmhouse
            doorPositions.Add(new Vector3(2452.5908203125f, 4969.8911132813f, 46.810550689697f));
            doorPositions.Add(new Vector3(2449.0825195313f, 4989.3833007813f, 46.811088562012f));
            doorPositions.Add(new Vector3(2440.3703613281f, 4982.212890625f, 46.808639526367f));
            doorPositions.Add(new Vector3(2435.8952636719f, 4975.642578125f, 46.809665679932f));

            //Blue Garage
            doorPositions.Add(new Vector3(2330.2785644531f, 2576.48828125f, 46.667694091797f));
            doorPositions.Add(new Vector3(2332.6513671875f, 2575.365234375f, 46.67529296875f));

            //Trevors Trailer
            doorPositions.Add(new Vector3(1973.3153076172f, 3815.6909179688f, 33.510341644287f));

            //Zancudo Atc
            doorPositions.Add(new Vector3(-2342.7062988281f, 3266.6965332031f, 32.827625274658f));

            //Trevors Methlab/Shop
            doorPositions.Add(new Vector3(1399.2657470703f, 3608.3029785156f, 38.999332427979f));
            doorPositions.Add(new Vector3(1387.8190917969f, 3614.5666503906f, 38.941898345947f));
            doorPositions.Add(new Vector3(1394.056640625f, 3599.8852539063f, 34.98095703125f));

            //Country Bar
            doorPositions.Add(new Vector3(1990.6750488281f, 3053.3930664063f, 47.215408325195f));

            //Humane Labs
            doorPositions.Add(new Vector3(3627.916015625f, 3746.5302734375f, 28.69010925293f));
            doorPositions.Add(new Vector3(3620.9008789063f, 3751.3952636719f, 28.690111160278f));

            bool close = false;
            foreach (Vector3 pos in doorPositions)
            {
                Vector3 p = Game.PlayerPed.Position;
                if (API.GetDistanceBetweenCoords(p.X, p.Y, p.Z, pos.X, pos.Y, pos.Z, true) <= 10f)
                {
                    close = true;
                }
            }

            if (!close)
            {
                return;
            }

            Prop[] props = World.GetAllProps();
            foreach (Prop p in props)
            {
                foreach (Vector3 pos in doorPositions)
                {
                    if (API.GetDistanceBetweenCoords(p.Position.X, p.Position.Y, p.Position.Z, pos.X, pos.Y, pos.Z, true) <= 2f)
                    {
                        API.FreezeEntityPosition(p.Handle, true);
                    }
                }

            }
            await Delay(1000);
        }

        private static void ShowNotification(string message)
            => ClientFunctions.ShowToast("Housing", message, "info");
    }
}
