using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Communications.Interfaces;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers
{
    public class GrassBagController : Controller
    {
        struct DamageInfo
        {
            public int PlayerHandle { get; }
            public int EntityHandle { get; }
            public float Damage { get; }
        }

        private readonly ILogger<GrassBagController> _log;

        //private readonly ConcurrentBag<>

        public GrassBagController(ILogger<GrassBagController> log, ILegacyServerCommunicationsManager comms)
        {
            _log = log;
            //comms.On(ServerEvents.GrassBagVehicleDamagedByPlayer, );
            comms.On(ServerEvents.GrassBagGrass, new Action<Player, string>(HandleGrass));
        }

        private void HandleGrass(Player reportingPlayer, string message)
        {
            var printMessage = $"{message} <{reportingPlayer.Name}>";
            _log.Debug(printMessage);
        }
    }
}
