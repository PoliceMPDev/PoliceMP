using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Microsoft.Extensions.DependencyInjection;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;

namespace PoliceMP.Server.Controllers
{
    public class AntiCheatController : Controller
    {
        private readonly ILogger<AntiCheatController> _logger;
        
        public AntiCheatController(ILogger<AntiCheatController> logger, IFiveEventManager fiveEvent)
        {
            _logger = logger;

            //fiveEvent.On<int>("entityCreating", OnEntityCreating);
            fiveEvent.On<Player, string, dynamic, dynamic>("playerConnecting", OnPlayerConnecting);   
        }
        
        public override Task Started()
        {
            return Task.CompletedTask;
        }

        private string[] bannedThingsInName =
        {
            "<",
            ">",
            "~",
            "^"
        };

        private async Task OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();
            await Delay(0);

            foreach (var str in bannedThingsInName)
            {
                if (!playerName.Contains(str)) continue;
                deferrals.done($"Your name contains {str}, please change and rejoin.");
                API.DropPlayer(player.Handle, $"Your name contains {str}, please change and rejoin.");
                _logger.Debug($"DDS has just booted {playerName} for a bad name containing {str}");
                TriggerEvent("txaLogger:DebugMessage",$"DDS has just booted {playerName} for a bad name containing {str}");
                return;
            }

            deferrals.done();
            return;
        }
    }
}