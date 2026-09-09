using CitizenFX.Core;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IAnimationService
    {
        Task Breathalyser(Ped ped);
        Task HandsUp(Ped ped, bool standingOnly = false);
        Task HowYouDoing(Ped ped);
        Task ShowId(Ped ped);
        Task FacePalm(Ped ped);
        Task GesturePoint(Ped ped);
        void Clear(Ped ped);

        Task Play(Ped ped,
            string dict,
            string name,
            float blendInSpeed = 8f,
            float blendOutSpeed = -8f,
            int duration = 5000,
            int flag = 49,
            float playbackRate = 0,
            bool lockX = false,
            bool lockY = false,
            bool lockZ = false);
    }

}