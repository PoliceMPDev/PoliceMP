using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Org.BouncyCastle.Asn1.Anssi;
using PoliceMP.Core.Server.Commands.Interfaces;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Server.Interfaces.Services;
using PoliceMP.Core.Server.Networking;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Services;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Controllers.AnprPings
{
    public class AnprPings : Controller
    {
        private readonly ILogger<AnprPings> _logger;
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly PlayerList _players;
        private readonly INotificationService _notification;

        public AnprPings(ILogger<AnprPings> logger,
            ILegacyServerCommunicationsManager comms,
            PlayerList players,
            INotificationService notification)
        {
            _logger = logger;
            _comms = comms;
            _players = players;
            _notification = notification;
        }

        public override Task Started()
        {
            /*
            _comms.On(ServerEvents.AnprPingRequest, (Player player, string targetPlate, string requesterName) =>
            {
                AnprPingRequest(player, targetPlate, requesterName);
            });

            _comms.On(ServerEvents.AnprPingSendCoords, (Player player, Vector3 targetCoords, string targetPlate, string requesterName) =>
            {
                AnprPingSendCoords(player, targetCoords, targetPlate, requesterName);
            });
            */

            return Task.FromResult(0);
        }

        //private DateTime _lastRequest = DateTime.Now;
        //private string _lastPlateChecked;

        private async Task AnprPingRequest(Player player, string targetPlate, string requesterName)
        {
            try
            {
                foreach (var ped in _players)
                {
                    _comms.ToClient(ped, ClientEvents.CheckForPlateOnClient, targetPlate, requesterName);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }

        private async Task AnprPingSendCoords(Player player, Vector3 targetCoords, string targetPlate, string requesterName)
        {
            try
            {
                foreach (var ped in _players)
                {
                    _comms.ToClient(ped, ClientEvents.AnprPingResponse, targetCoords, targetPlate, requesterName);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
            }
        }
    }
}