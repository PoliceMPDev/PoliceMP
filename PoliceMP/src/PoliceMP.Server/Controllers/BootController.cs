using Newtonsoft.Json;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Server.Extensions;
using PoliceMP.Shared.Constants.States;

namespace PoliceMP.Server.Controllers
{
    public class BootController : Controller
    {
        private const string BOOT_ITEMS_FILE = "BootItems.json";

        private readonly ILogger<BootController> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;

        public BootController(ILogger<BootController> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms)
        {
            _logger = logger;
            _commands = commands;
            _comms = comms;
        }

        public override Task Started()
        {
            GlobalState.Set(GlobalStates.BootItems, JsonConvert.DeserializeObject<List<BootItem>>(TextFromFile(BOOT_ITEMS_FILE)));

            return Task.FromResult(0);
        }

        private static string TextFromFile(string fileName)
        {
            try
            {
                using var file = File.OpenText(fileName);

                return file.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            
            return null;
        }

    }
}
