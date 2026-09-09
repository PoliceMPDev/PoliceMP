using PoliceMP.Shared.Models;
using System.Threading.Tasks;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IPedInfoService
    {
        Task<PedInfo> GetByNetworkId(int networkId);
        Task<PedInfo> GetByName(string name);
    }
}