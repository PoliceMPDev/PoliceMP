using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface ISpeechService
    {
        void Say(Ped ped, string speech, int durationMs = 5000, bool replicate = true);
        void SayRandomGreeting(Ped ped, int durationMs = 5000, bool replicate = true);
        void SayRandomFarewell(Ped ped, int durationMs = 5000, bool replicate = true);
        void SayRandomInsult(Ped ped, int durationMs = 5000, bool replicate = true);
        void Do(Ped ped, string action, int durationMs = 5000, bool replicate = true);
    }
}