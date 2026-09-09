using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;


namespace PoliceMP.Server.Controllers
{
    class ClothingLockerController : Controller
    {
        private const string LOCKER_POSITIONS_FILE = "Lockers.json";
        private const string LOCKER_OUTFITS_FILE = "Outfits.json";
        private const string LOCKER_CLOTHING_FILE = "Clothing.json";

        private readonly ILogger<ClothingLockerController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;

        private List<Locker> Lockers;
        private List<PedOutfit> Outfits;
        private List<LockerItem> LockerItems;

        public ClothingLockerController(ILogger<ClothingLockerController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms)
        {
            _logger = logger;
            _commands = commands;
            _comms = comms;

            Lockers = JsonConvert.DeserializeObject<List<Locker>>(TextFromFile(LOCKER_POSITIONS_FILE));
            Outfits = JsonConvert.DeserializeObject<List<PedOutfit>>(TextFromFile(LOCKER_OUTFITS_FILE));
            LockerItems = JsonConvert.DeserializeObject<List<LockerItem>>(TextFromFile(LOCKER_CLOTHING_FILE));
        }

        public override Task Started()
        {
            _comms.OnRequest<List<Locker>>(ServerEvents.GetLockerPoints, OnRequestLockerPoints);
            _comms.OnRequest<List<PedOutfit>>(ServerEvents.GetLockerOutfits, OnRequestLockerOutfits);
            _comms.OnRequest<List<LockerItem>>(ServerEvents.GetLockerClothes, OnRequestLockerItems);

            return Task.FromResult(0);
        }

        private Task<List<Locker>> OnRequestLockerPoints(Player player)
        {
            return Task.FromResult(Lockers);
        }

        private Task<List<PedOutfit>> OnRequestLockerOutfits(Player player)
        {
            List<PedOutfit> AllowedOutfits = new List<PedOutfit>();
            foreach (PedOutfit po in Outfits)
            {
                if (po.AceGroupsRequired.Length == 0)
                {
                    AllowedOutfits.Add(po);
                    continue;
                }

                if (po.AceGroupsRequired.All(rg => API.IsPlayerAceAllowed(player.Handle, rg)))
                {
                    AllowedOutfits.Add(po);
                }
            }
            return Task.FromResult(AllowedOutfits);
        }

        private Task<List<LockerItem>> OnRequestLockerItems(Player player)
        {
            List<LockerItem> AllowedClothing = new List<LockerItem>();
            foreach (LockerItem li in LockerItems)
            {
                if (li.AceGroupsRequired.Length == 0)
                {
                    AllowedClothing.Add(li);
                    continue;
                }

                if (li.AceGroupsRequired.All(rg => API.IsPlayerAceAllowed(player.Handle, rg)))
                {
                    AllowedClothing.Add(li);
                }
            }
            return Task.FromResult(AllowedClothing);
        }

        private string TextFromFile(string _file)
        {
            try
            {
                using (var file = File.OpenText(_file))
                {
                    return file.ReadToEnd();
                }
            }
            catch (Exception) { }
            return null;
        }
    }
}
