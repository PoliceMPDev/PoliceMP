using CitizenFX.Core;
using PoliceMP.Core.Server.Communications.Interfaces;
using PoliceMP.Core.Shared;
using PoliceMP.Server.Controllers;
using PoliceMP.Server.Services.Interfaces;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Server.Services
{
    public class SoundService : ISoundService
    {
        private readonly ILegacyServerCommunicationsManager _comms;
        private readonly ILogger<SoundService> _logger;
        private readonly PlayerList _players;

        public SoundService(ILegacyServerCommunicationsManager comms, ILogger<SoundService> logger, PlayerList players)
        {
            _comms = comms;
            _logger = logger;
            _players = players;
            
            _comms.On<int, string, float>(ServerEvents.SoundToClient, (_, netId, soundFile, soundVolume) =>
                {
                    _comms.ToClient(_players[netId], ClientEvents.SoundToClient, soundFile, soundVolume);
                });
            
            _comms.On<string, float>(ServerEvents.SoundToAll, (_, soundFile, soundVolume) =>
            {
                _comms.ToClient(ClientEvents.SoundToAll, soundFile, soundVolume);
            });
            
            _comms.On<int, float, string, float>(ServerEvents.SoundToRadius, (_, netId, soundRadius, soundFile, soundVolume) =>
            {
                _comms.ToClient(ClientEvents.SoundToRadius, netId, soundRadius, soundFile, soundVolume);
            });
            
            _comms.On<float, float, float, float, string, float>(ServerEvents.SoundToCoords, (_, positionX, positionY, positionZ, soundRadius, soundFile, soundVolume) =>
            {
                _comms.ToClient(ClientEvents.SoundToCoords, positionX, positionY, positionZ, soundRadius, soundFile, soundVolume);
            });
        }

        public void SoundToClient(Player player, string soundName, float soundVolume)
        {
            _comms.ToClient(player, ClientEvents.SoundToClient, soundName, soundVolume);
        }

        public void SoundToAll(string soundName, float soundVolume)
        {
            _comms.ToClient(ClientEvents.SoundToAll, soundName, soundVolume);
        }

        public void SoundToRadius(Entity entity, float radius, string soundName, float soundVolume)
        {
            _comms.ToClient(ClientEvents.SoundToRadius, entity.NetworkId, radius, soundName, soundVolume);
        }

        public void SoundToCoord(float posX, float posY, float posZ, float soundRadius, string soundName,
            float soundVolume)
        {
            _comms.ToClient(ClientEvents.SoundToCoords, posX, posY, posZ, soundRadius, soundName, soundVolume);
        }
    }
}