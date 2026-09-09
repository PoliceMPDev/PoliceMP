using CitizenFX.Core;
using CitizenFX.Core.Native;
using PoliceMP.Client.Overlays.Legacy.SoundPlayerOverlay;
using PoliceMP.Client.Services.Interfaces;

namespace PoliceMP.Client.Services
{
    public class SoundService : ISoundService
    {
        private readonly SoundPlayerOverlay _overlay;

        public SoundService(SoundPlayerOverlay overlay)
        {
            _overlay = overlay;
        }

        public void Play(string soundFileName, float soundVolume = 0.3f) => _overlay.Play(soundFileName, soundVolume);

        public void PlayFromLocation(string fileName, float volume, Vector3 listenerPosition, Vector3 soundPosition, Vector3 listenerRotation) => _overlay.PlayFromLocation(fileName, volume, listenerPosition, soundPosition, listenerRotation);
    }
}