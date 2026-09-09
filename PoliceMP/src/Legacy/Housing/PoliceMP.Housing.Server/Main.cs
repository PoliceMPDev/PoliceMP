using System;
using System.Collections.Generic;
using System.IO;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace PoliceMP.Housing.Server
{
    public class Main : BaseScript
    {
        private const string INTERIOR_FILE = "Interiors.json";
        private const string HOUSES_FILE = "Houses.json";

        private string jsonInteriors;
        private string jsonHouses;

        private List<House> Houses = new List<House>();

        public Main()
        {
            RetrieveJson();
        }

        [EventHandler("PoliceMPHousing:ClientRequestingData")]
        private void ClientRequestingLists([FromSource] Player _pl)
        {
            if (jsonInteriors == null || jsonHouses == null)
            {
                RetrieveJson();
            }
            _pl.TriggerEvent("PoliceMPHousing:SendingClientInteriors", jsonInteriors);
            _pl.TriggerEvent("PoliceMPHousing:SendingClientHouses", jsonHouses);
        }

        [EventHandler("PoliceMPHousing:RegisterEntityInHouse")]
        private void RegisterEntityInHouse(int _houseId, int _entityNetId)
        {
            if (!DoesHouseExist(_houseId, true, false)) { return; }
            lock (Houses)
            {
                foreach (var house in Houses)
                {
                    if (house.HouseID == _houseId)
                    {
                        house.EntitiesInHouseNetIDs.Add(_entityNetId);
                    }

                    //Send update to everyone in houses
                    foreach (House h in Houses)
                    {
                        List<int> playerHandles = new List<int>();

                        foreach (var p in h.PlayersInHouse)
                        {
                            playerHandles.Add(p.Item2);
                        }

                        foreach (var p in h.PlayersInHouse)
                        {
                            p.Item1.TriggerEvent("PoliceMPHousing:CurrentHouseUpdate", h.HouseID, JsonConvert.SerializeObject(playerHandles), JsonConvert.SerializeObject(h.EntitiesInHouseNetIDs));
                        }
                    }
                }
            }
        }

        [EventHandler("PoliceMPHousing:DeregisterEntityInHouse")]
        private void DeRegisterEntityInHouse(int _houseId, int _entityNetId)
        {
            lock (Houses)
            {
                if (!DoesHouseExist(_houseId, true, false)) { return; }
                foreach (var house in Houses)
                {
                    if (house.HouseID == _houseId)
                    {
                        house.EntitiesInHouseNetIDs.Remove(_entityNetId);
                    }

                    //Send update to everyone in houses
                    foreach (House h in Houses)
                    {
                        List<int> playerHandles = new List<int>();

                        foreach (var p in h.PlayersInHouse)
                        {
                            playerHandles.Add(p.Item2);
                        }

                        foreach (var p in h.PlayersInHouse)
                        {
                            p.Item1.TriggerEvent("PoliceMPHousing:CurrentHouseUpdate", h.HouseID, JsonConvert.SerializeObject(playerHandles), JsonConvert.SerializeObject(h.EntitiesInHouseNetIDs));
                        }
                    }
                }
            }
        }

        private void RetrieveJson()
        {
            try
            {
                using (var file = File.OpenText(INTERIOR_FILE))
                {
                    jsonInteriors = file.ReadToEnd();
                }

                using (var file = File.OpenText(HOUSES_FILE))
                {
                    jsonHouses = file.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Havin a wabbler mate " + ex);
            }
        }

        private int FindCurrentPlayerId(Player _player)
        {
            for (int i = 0; i < 256; ++i)
            {
                var playerHandle = API.GetPlayerFromIndex(i);
                if (playerHandle == _player.Handle)
                {
                    return i;
                }
            }
            return -1;
        }

        [EventHandler("PoliceMPHousing:PlayerIsEnteringHouse")]
        private async void EnteringHouse([FromSource] Player _player, int _houseID, int playerHandle)
        {
            if (!DoesHouseExist(_houseID, true, false)) { _player.TriggerEvent("PoliceMPHousing:HouseDoesNotExist"); return; }
            lock (Houses)
            {
                //Do the physical change of player entering
                foreach (House h in Houses)
                {
                    if (h.HouseID == _houseID)
                    {
                        if (h.Locked)
                        {
                            _player.TriggerEvent("PoliceMPHousing:HouseIsCurrentlyLocked");
                            return;
                        }
                        else
                        {
                            h.PlayersInHouse.Add(new Tuple<Player, int>(_player, playerHandle));
                            _player.TriggerEvent("PoliceMPHousing:GrantedToEnterHouse");
                        }
                    }
                }

                //Update everyone who is in a house of their house status
                foreach (House h in Houses)
                {
                    List<int> playerHandles = new List<int>();

                    foreach (var p in h.PlayersInHouse)
                    {
                        playerHandles.Add(p.Item2);
                    }

                    foreach (var p in h.PlayersInHouse)
                    {
                        p.Item1.TriggerEvent("PoliceMPHousing:CurrentHouseUpdate", h.HouseID, JsonConvert.SerializeObject(playerHandles), JsonConvert.SerializeObject(h.EntitiesInHouseNetIDs));
                    }
                }
            }
        }

        [EventHandler("PoliceMPHousing:PlayerIsLeavingHouse")]
        private void LeavingHouse([FromSource] Player _player, int _houseID, int playerHandle)
        {
            if (!DoesHouseExist(_houseID, false, false)) { _player.TriggerEvent("PoliceMPHousing:HouseDoesNotExist"); return; }

            lock (Houses)
            {
                //Do the change of the plater leaving the house
                foreach (House h in Houses)
                {
                    if (h.HouseID == _houseID)
                    {
                        h.PlayersInHouse.Remove(new Tuple<Player, int>(_player, playerHandle));
                    }
                }

                //Send out updates to all clients for info about their house
                foreach (House h in Houses)
                {
                    List<int> playerHandles = new List<int>();

                    foreach (var p in h.PlayersInHouse)
                    {
                        playerHandles.Add(p.Item2);
                    }

                    foreach (var p in h.PlayersInHouse)
                    {
                        p.Item1.TriggerEvent("PoliceMPHousing:CurrentHouseUpdate", h.HouseID, JsonConvert.SerializeObject(playerHandles), JsonConvert.SerializeObject(h.EntitiesInHouseNetIDs));
                    }
                }
            }
        }

        [EventHandler("PoliceMPHousing:UnlockHouse")]
        private void UnlockHouse([FromSource] Player _player, int _houseID)
        {
            if (!DoesHouseExist(_houseID, false, false)) { _player.TriggerEvent("PoliceMPHousing:HouseDoesNotExist"); return; }

            lock (Houses)
            {
                foreach (House h in Houses)
                {
                    if (h.HouseID != _houseID)
                    {
                        continue;
                    }
                    h.Locked = false;
                    _player.TriggerEvent("PoliceMPHousing:DoorHasBeenForced");
                }
            }
        }

        [EventHandler("PoliceMPHousing:LockHouse")]
        private void LockHouse([FromSource] Player _player, int _houseID)
        {
            if (!DoesHouseExist(_houseID, false, false)) { _player.TriggerEvent("PoliceMPHousing:HouseDoesNotExist"); return; }

            lock (Houses)
            {
                foreach (House h in Houses)
                {
                    if (h.HouseID != _houseID)
                    {
                        continue;
                    }
                    h.Locked = true;
                    _player.TriggerEvent("PoliceMPHousing:DoorHasBeenLocked");
                }
            }
        }


        [EventHandler("PoliceMPHousing:InstantiateHouse")]
        private void InstantiateHouse([FromSource] Player _player, int _houseID, bool _lockStatus)
        {
            DoesHouseExist(_houseID, true, _lockStatus);
            _player.TriggerEvent("PoliceMPHousing:HouseHasBeenEnstantiated");
        }

        private bool DoesHouseExist(int _houseID, bool _ShouldCreate, bool _lockStatus)
        {
            lock (Houses)
            {
                foreach (House h in Houses)
                {
                    if (h.HouseID != _houseID)
                    {
                        continue;
                    }
                    return true;
                }

                if (_ShouldCreate)
                {
                    House newHouse = new House
                    {
                        HouseID = _houseID,
                        Locked = _lockStatus,
                        PlayersInHouse = new List<Tuple<Player, int>>(),
                        EntitiesInHouseNetIDs = new List<int>()
                    };
                    Houses.Add(newHouse);
                    return true;
                }
                return false;
            }
        }
    }
}
