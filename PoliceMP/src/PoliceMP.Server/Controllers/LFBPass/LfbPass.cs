using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;
using PoliceMP.Server.Extensions;


namespace PoliceMP.Server.Controllers.LFBPass
{
    public class LfbPass : Controller
    {
        private readonly ILogger<LfbPass> _logger;
        private readonly ICommandManager _commands;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly INotificationService _notifications;
        private readonly PlayerList _players;
        

        private readonly float AlarmDistance = 30f;

        public LfbPass(ILogger<LfbPass> logger,
            ICommandManager commands,
            ILegacyServerCommunicationsManager comms,
            INotificationService notifications,
            PlayerList players)
        {
            _logger = logger;
            _commands = commands;
            _comms = comms;
            _notifications = notifications;
            _players = players;
        }

        public override Task Started()
        {
            _comms.On(ServerEvents.FirefighterPass, (Player player, int passAlarm) =>
            {
                // Console.WriteLine("FirefighterPass event received.");
                Alarm(player, passAlarm);
            });

            return Task.FromResult(0);
        }

        private async void Alarm(Player player, int PassAlarm)
        {
            try
            {
                // Console.WriteLine("Alarm triggered");
                Vector3 playerPosition = player.Character.Position;

                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;

                    Vector3 pedPosition = ped.Character.Position;

                    if (pedPosition.Distance(playerPosition) > AlarmDistance) continue;
                    
                    _comms.ToClient(ped, ClientEvents.FirefighterPass, PassAlarm);
                }

                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }
    }
}