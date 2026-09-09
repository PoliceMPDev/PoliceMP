using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface ISoundService
    {
        void Play(string soundFileName, float soundVolume = 0.3f);
        void PlayFromLocation(string fileName, float volume, Vector3 listenerPosition, Vector3 soundPosition, Vector3 listenerRotation);
    }
}