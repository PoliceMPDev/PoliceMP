using CitizenFX.Core;
using System.Threading.Tasks;

namespace PoliceMP.Client.Scripts.PedInteractionMenu
{
    public interface IPedInteractionMenuScript
    {
        public Task InteractWith(Ped ped);
        public bool IsMenuActive();
    }
}