using System.Threading.Tasks;
using CitizenFX.Core;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IPersonalityService
    {
        Task<bool> WillPedResist(Ped ped);
    }
}
