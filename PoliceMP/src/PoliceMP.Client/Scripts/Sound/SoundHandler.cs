using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Services.Interfaces;
using PoliceMP.Core.Client;
using PoliceMP.Core.Client.Commands.Interfaces;
using PoliceMP.Core.Client.Communications.Interfaces;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Client.Scripts;
using PoliceMP.Core.Shared;
using PoliceMP.Shared.Constants;

namespace PoliceMP.Client.Scripts.Sound
{
    public interface ISoundHandler
    {
        void ToClient(string soundFile, float soundVolume);
        void ToAll(string soundFile, float soundVolume);
        void ToRadius(int networkId, float soundRadius, string soundFile, float soundVolume);
        void ToCoords(float positionX, float positionY, float positionZ, float soundRadius, string soundFile, float soundVolume);
        bool IsStarted { get; }
        Task StartAsync();
    }

    public class SoundHandler : Script, ISoundHandler
    {
        private readonly ILegacyClientCommunicationsManager _comms;
        private readonly INotificationService _notification;
        private readonly ILogger<SoundHandler> _logger;
        private readonly ITickManager _ticks;
        private readonly ICommandManager _command;
        private readonly ISoundService _sound;

        public SoundHandler(ILegacyClientCommunicationsManager comms, INotificationService notification,
            ILogger<SoundHandler> logger, ITickManager ticks, ICommandManager command, ISoundService sound)
        {
            _comms = comms;
            _notification = notification;
            _logger = logger;
            _ticks = ticks;
            _command = command;
            _sound = sound;
        }

        protected override async Task OnStartAsync()
        {
            _comms.On<string, float>(ClientEvents.SoundToClient, ToClient);
            _comms.On<string, float>(ClientEvents.SoundToAll, ToAll);
            _comms.On<int, float, string, float>(ClientEvents.SoundToRadius, ToRadius);
            _comms.On<float, float, float, float, string, float>(ClientEvents.SoundToCoords, ToCoords);
        }
        
        public void ToClient(string soundFile, float soundVolume)
        {
            _sound.Play(soundFile, soundVolume);
        }

        public void ToAll(string soundFile, float soundVolume)
        {
            _sound.Play(soundFile, soundVolume);
        }

        public void ToRadius(int networkId, float soundRadius, string soundFile, float soundVolume)
        {
            _logger.Debug($"[{networkId}] {soundFile} {soundVolume}");
            var playerCoords = Game.Player.Character.Position;

            Vector3 targetCoords;
            targetCoords = !API.NetworkDoesEntityExistWithNetworkId(networkId) ? playerCoords : API.GetEntityCoords(API.NetworkGetEntityFromNetworkId(networkId), true);
            
            var distance = API.Vdist(playerCoords.X, playerCoords.Y, playerCoords.Z, targetCoords.X, targetCoords.Y, targetCoords.Z);
            _logger.Debug($"Distance: {distance}");
            var distanceVolumeMultiplier = (soundVolume / soundRadius);
            var distanceVolume = soundVolume - (distance * distanceVolumeMultiplier);
            _logger.Debug($"Sound Radius: {soundRadius}");
            _logger.Debug($"Distance Volume: {distanceVolume}");
            if (distance <= soundRadius)
            {
                _sound.Play(soundFile, distanceVolume);
            }
        }

        public void ToCoords(float positionX, float positionY, float positionZ, float soundRadius, string soundFile, float soundVolume)
        {
            var playerCoords = Game.Player.Character.Position;
            var compare = API.Vdist(playerCoords.X, playerCoords.Y, playerCoords.Z, positionX, positionY, positionZ);
            var distanceVolumeMultiplier = (soundVolume / soundRadius);
            var distanceVolume = soundVolume - (compare * distanceVolumeMultiplier);
            
            if (compare <= soundRadius)
            {
                _sound.Play(soundFile, distanceVolume);
            }
        }
    }
}