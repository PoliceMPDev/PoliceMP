using CitizenFX.Core;

namespace PoliceMP.Server.Services.Interfaces
{
    public interface ISoundService
    {
        void SoundToClient(Player player, string soundName, float soundVolume);
        void SoundToAll(string soundName, float soundVolume);
        void SoundToRadius(Entity entity, float radius, string soundName, float soundVolume);

        void SoundToCoord(float posX, float posY, float posZ, float soundRadius, string soundName,
            float soundVolume);
    }
}