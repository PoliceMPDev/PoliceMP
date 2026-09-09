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

namespace PoliceMP.Server.Controllers.TwitterScript
{
    public class SpawnVehicleToPlayer : Controller
    {
        private readonly ILogger<SpawnVehicleToPlayer> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ICommandManager _commands;
        private readonly PlayerList _players;

        public SpawnVehicleToPlayer(ILogger<SpawnVehicleToPlayer> logger,
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
            _comms.On(ServerEvents.SendVehicleSpawnToServer, (string spawnCode, string targetPlayerName, bool elsEnabled) =>
            {
                VehicleFromServer(spawnCode, targetPlayerName, elsEnabled);
            });

            API.RegisterCommand("dscsvtp", new Action<int, List<object>, string>((source, args, rawCommand) =>
            {
                if (args.Count < 3)
                {
                    return;
                }

                var elsEnabled = false;
                string spawnCode = args[0].ToString();
                string targetPlayerName = string.Join(" ", args.GetRange(1, args.Count - 2).Select(arg => arg.ToString()));
                string elsAnswer = args[2].ToString();
                if (elsAnswer == "yes") elsEnabled = true;
                _comms.ToServer(ServerEvents.SendVehicleSpawnToServer, spawnCode, targetPlayerName, elsEnabled);
                Console.WriteLine($"SpawnVehicleToPlayer: Vehicle {spawnCode} has been sent to {targetPlayerName} via Console with els {elsEnabled}!");
            }), false);

            return Task.FromResult(0);
        }

        private void VehicleFromServer(string spawnCode, string targetPlayerName, bool elsEnabled)
        {
            try
            {
                foreach (var ped in _players.ToArray())
                {
                    if (ped == null || ped.Character == null) continue;
                    if (ped.Name != targetPlayerName) continue;
                    
                    _comms.ToClient(ped, ClientEvents.SendVehicleToClient, spawnCode, targetPlayerName, elsEnabled);
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