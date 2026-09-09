using CitizenFX.Core;
using PoliceMPCallouts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoliceMPCallouts.Server
{
    public class ClientEntityChecker : BaseScript
    {
        private int _requestingNetworkId = -1;
        private bool _exists = false;
        private bool _received = false;

        [EventHandler(ServerEvents.RETURN_DOES_ENTITY_EXIST)]
        private async void ReceiveResultFromClient(int networkId, bool exists)
        {
            if (networkId != _requestingNetworkId) return;

            _received = true;
            _exists = exists;
        }

        public async Task<bool> DoesEntityExistOnAnyClient(int networkId)
        {
            foreach (var player in Players)
            {
                _requestingNetworkId = networkId;
                _exists = false;
                _received = false;
                player.TriggerEvent(ClientEvents.DOES_ENTITY_EXIST, networkId);

                while (!_received) await Delay(10);

                if (_exists) return true;
            }

            return false;
        }
    }
}
