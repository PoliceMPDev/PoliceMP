using System;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.NetworkMessages.XPSystem.Notifications;
namespace PoliceMP.Server.Services
{
    /// <summary>
    /// XP system that stores its data in a server-side JSON file.
    /// To be replaced later down the line by Dashboard.
    /// </summary>
    public class JsonXPService : IXPService
    {
        private const string jsonFile = "PlayerXP.json";

        private readonly ILogger<JsonXPService> _logger;
        private readonly IServerCommunicationsManager _comms;
        
        public JsonXPService(ILogger<JsonXPService> logger, IServerCommunicationsManager comms)
        {
            _logger = logger;
            _comms = comms;
        }

        public Task NotifyInitialXP(Player player)
        {
            int currentXp;
            try
            {
                // Grab XP from JSON for given player
                JObject jsonData = GetXPDataFromJson();
                currentXp = jsonData[player.Identifiers["discord"]]?.Value<int>() ?? 0;
            }
            catch (ArgumentNullException ex)
            {
                currentXp = 0;
            }

            // Fire notification to client to notify them of current XP
            _comms.PublishToClient(player, new SetXPEvent()
            {
                XpValue = currentXp
            });

            return Task.FromResult(0);
        }

        public Task IncreasePlayerXP(Player player, int xpIncrease, string reason)
        {
            // Store new XP in JSON
            JObject jsonData = GetXPDataFromJson();
            int currentXp = jsonData[player.Identifiers["discord"]]?.Value<int>() ?? 0;
            if (currentXp < 0) currentXp = 0;
            currentXp += xpIncrease;
            jsonData[player.Identifiers["discord"]] = currentXp;
            StoreXPDataToJson(jsonData);
            
            // Tell client that XP has increased
            _comms.PublishToClient(player, new XPIncreasedEvent()
            {
                XpIncrease = xpIncrease,
                Reason = reason
            });

            return Task.FromResult(0);
        }

        public Task DecreasePlayerXP(Player player, int xpDecrease, string reason)
        {
            // Store new XP in JSON
            JObject jsonData = GetXPDataFromJson();
            int currentXp = jsonData[player.Identifiers["discord"]]?.Value<int>() ?? 0;
            if (currentXp > 0)
            {
                currentXp -= xpDecrease;
                jsonData[player.Identifiers["discord"]] = currentXp;
                StoreXPDataToJson(jsonData);
            }

            // Tell client that XP has decreased
            _comms.PublishToClient(player, new XPDecreasedEvent()
            {
                XpDecrease = xpDecrease,
                Reason = reason
            });
            
            return Task.FromResult(0);
        }

        private JObject GetXPDataFromJson()
        {
            if (!File.Exists(jsonFile))
            {
                File.WriteAllText(jsonFile, "{}");
            }
            var dataText = File.ReadAllText(jsonFile);
            
            return (JObject)JsonConvert.DeserializeObject(dataText);
        }
        
        private void StoreXPDataToJson(JObject jsonData)
        {
            File.WriteAllText(jsonFile, jsonData.ToString());
        }
    }
}