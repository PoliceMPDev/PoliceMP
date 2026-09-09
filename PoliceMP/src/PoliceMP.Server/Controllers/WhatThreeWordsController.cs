using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CitizenFX.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Data;
using PoliceMP.Data.Entities;
using PoliceMP.Server.Extensions;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class WhatThreeWordsController : Controller
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WhatThreeWordsController> _logger;
        private readonly INotificationService _notification;
        private readonly ILegacyServerCommunicationsManager _comms;

        public WhatThreeWordsController(IServiceScopeFactory scopeFactory, ICommandManager commandManager, ILogger<WhatThreeWordsController> logger, INotificationService notification, ILegacyServerCommunicationsManager comms)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _notification = notification;
            _comms = comms;
            commandManager.Register("w3p").WithHandler(WhatThreeWordCommand);
            _comms.On<Player, string>(ClientEvents.GoToWhatThreeWord, GoToWhatThreeWordCommand);
        }

        private async Task GoToWhatThreeWordCommand(Player player, string whatWord)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();
            var whatData = await context.WhatThreeWords.FirstOrDefaultAsync(s => s.Name.ToLower() == whatWord.ToLower());

            if (whatData == null)
            {
                _notification.Error(player, "WhatThreePhases", "Unable to find this location!");
                return;
            }

            var whatPosition = new Core.Shared.Models.PmpVector3(whatData.PosX, whatData.PosY, whatData.PosZ);

            _comms.ToClient(player, ServerEvents.SendWhatThreeWordPosToClient, whatPosition);

            _notification.Success(player, "WhatThreePhases", $"Sending the location to your GPS!");
        }

        private async void WhatThreeWordCommand(Player player)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();

                var playerPosition = player.Character.Position;

                var positionList = await context.WhatThreeWords.ToListAsync();

                var distance = 3.0f;

                var wordPosition = Vector3.Zero;

                var word = string.Empty;

                foreach (var whatPosition in positionList)
                {
                    var position = new Vector3(whatPosition.PosX, whatPosition.PosY, whatPosition.PosZ);

                    var distanceTo = playerPosition.Distance(position, true);

                    _logger.Debug($"WhatThreePhases Distance without Z: {distanceTo}");

                    if (!(distanceTo < distance)) continue;

                    distance = distanceTo;
                    wordPosition = position;
                    word = whatPosition.Name;
                }
                if (wordPosition == Vector3.Zero)
                {
                    var newWord = await FetchNewWordString();
                    var newWhatWordData = new WhatThreeWords
                    {
                        Name = newWord,
                        PosX = playerPosition.X,
                        PosY = playerPosition.Y,
                        PosZ = playerPosition.Z
                    };

                    await context.WhatThreeWords.AddAsync(newWhatWordData);
                    await context.SaveChangesAsync();

                    word = newWord;
                }

                if (word == string.Empty)
                {
                    _notification.Error(player, "WhatThreePhases", "Unable to get a WhatThreePhases!");
                    return;
                }

                _notification.Success(player, "WhatThreePhases", $"Your WhatThreePhases is <br> {word}");
                _comms.ToClient(player, ServerEvents.SendWhatThreeWordToClient, word);
            }
            catch (Exception ex)
            {
                _logger.Error("W3P", ex);
            }
        }

        private async Task<string> FetchNewWordString()
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();
            using var webClient = new HttpClient();

            var url = new Uri("http://random-word-api.herokuapp.com/word?number=3");
            var output = await webClient.GetStringAsync(url);

            _logger.Debug($"New Word Output: {output}");

            var stringList = JsonConvert.DeserializeObject<List<string>>(output);

            var whatThreeWords = string.Join(" ", stringList);

            var contains = await context.WhatThreeWords.AnyAsync(s => s.Name == whatThreeWords);
            while (contains)
            {
                output = await webClient.GetStringAsync(url);

                _logger.Debug($"New Word Output: {output}");
                stringList = JsonConvert.DeserializeObject<List<string>>(output);
                whatThreeWords = string.Join(" ", stringList);

                contains = await context.WhatThreeWords.AnyAsync(s => s.Name == whatThreeWords);
            }

            return whatThreeWords;
        }
    }
}