using System;
using System.Collections.Generic;
using System.Text;
using CitizenFX.Core;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers.Commands
{
    public class CivCommands : Controller
    {
        private readonly ILogger<CivCommands> _log;
        private readonly ILegacyServerCommunicationsManager _comms;

        public CivCommands(ILogger<CivCommands> log, ICommandManager commands, ILegacyServerCommunicationsManager comms)
        {
            _log = log;
            _comms = comms;
            commands.Register("blend").Restrict().WithHandler(CommandBlend);
        }

        private void CommandBlend(Player player)
        {
            _comms.ToClient(player, ClientEvents.CivBlend);
        }
    }
}
