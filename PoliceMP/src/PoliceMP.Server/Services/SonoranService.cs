using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Server.Services.Interfaces;
using RestSharp;

namespace PoliceMP.Server.Services
{
    public class SonoranService : ISonoranService
    {
        private readonly string _communityId = "PoliceMP";
        private readonly string _apiKey = "4AE6BC6-C572-4775-86";
        private readonly string _baseUri = "https://api.sonorancad.com";

        private readonly ILogger<SonoranService> _logger;
        private readonly IFeatureService _featureService;
        
        public SonoranService(ILogger<SonoranService> logger, IFeatureService featureService)
        {
            _logger = logger;
            _featureService = featureService;
        }

        public async Task SendPanicEvent(Player player)
        {
            var steamId = player.Identifiers[Identifiers.Steam];

            var serverId = 1;

            if (_featureService.IsFeatureEnabled("sonoran_prodono"))
            {
                serverId = 2;
            }

            if (_featureService.IsFeatureEnabled("sonoran_dev"))
            {
                serverId = 3;
            }

            if (string.IsNullOrEmpty(steamId))
            {
                _logger.Debug("ERROR: SteamID Null!");
                return;
            }
            
            var data = new
            {
                apiId = $"{steamId}",
                isPanic = true,
            };

            var jsonData = JsonConvert.SerializeObject(data);
            
            var client = new RestClient($"{_baseUri}");
            var requestUri = $"/emergency/unit_panic";
            
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            var request = new RestRequest(requestUri, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.Timeout = 5000; // 5 second timeout
            request.AddJsonBody(new
            {
                id = _communityId,
                key = _apiKey,
                serverId = serverId,
                type = "UNIT_PANIC",
                data = new []
                {
                    new
                    {
                        apiId = steamId,
                        isPanic = true
                    }
                }
            });
            
            try
            {
                var response = client.Post(request);
                if (serverId == 3)
                {
                    // Dev Server
                    _logger.Debug($"Content: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception occurred sending panic to Sonoran: " + ex.Message);
            }
        }
    }
}