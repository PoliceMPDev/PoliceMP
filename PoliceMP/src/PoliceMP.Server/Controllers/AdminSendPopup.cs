using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers.AdminSendPopup
{
    public class AdminSendPopup : Controller
    {
        private readonly ILogger<AdminSendPopup> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly PlayerList _players;

        public AdminSendPopup(ILogger<AdminSendPopup> logger,
            ILegacyServerCommunicationsManager comms, ICommandManager commands,
            PlayerList players) 
        {
            _logger = logger;
            _comms = comms;
            _players = players;
            _commands = commands;

        }

        public override Task Started()
        {
            _comms.On(ServerEvents.AdminSendPopupToServer, (string playerName) =>
            {
                SendPopup(playerName);
            });


            API.RegisterCommand(":smpu", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                string playerName = args[0].ToString();
                SendPopup(playerName);

            }), false);

            return Task.FromResult(0);
        }

        private async void SendPopup(string playerName)
        {
            try
            {
                foreach (var ped in _players.ToArray())
                {
                    await Delay(100);
                    if (ped == null || ped.Character == null) continue;
                    var name = API.GetPlayerName(ped.Handle);
                    if (name == playerName)
                    {
                        _comms.ToClient(ped, ClientEvents.AdminSendPopupClient, playerName);
                    }
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