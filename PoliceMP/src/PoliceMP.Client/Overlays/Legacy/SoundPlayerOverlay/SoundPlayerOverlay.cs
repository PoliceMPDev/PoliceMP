using CitizenFX.Core;
using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Overlays.Legacy.SoundPlayerOverlay
{
    public class SoundPlayerOverlay : LegacyOverlay
    {
        public SoundPlayerOverlay(ILegacyNuiManager nuiManager, ILogger<LegacyOverlay> logger) : base(nuiManager, logger)
        {
        }

        public void Play(string fileName, float volume = 0.3f)
        {
            
            Emit("playSound", new
            {
                fileName,
                volume
            });
        }
        
        public void PlayFromLocation(string fileName, float volume, Vector3 listenerPosition, Vector3 soundPosition, Vector3 listenerRotation)
        {
            Emit("playSoundFromLocation", new
            {
                fileName,
                volume,
                listenerPosition = new { x = listenerPosition.X, y = listenerPosition.Y, z = listenerPosition.Z },
                soundPosition = new { x = soundPosition.X, y = soundPosition.Y, z = soundPosition.Z },
                listenerRotation = new { x = listenerRotation.X, y = listenerRotation.Y, z = listenerRotation.Z }
            });
        }
    }
}
