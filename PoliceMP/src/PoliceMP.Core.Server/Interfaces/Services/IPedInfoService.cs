using PoliceMP.Shared.Models;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    public interface IPedInfoService
    {
        PedInfo GetByNetworkId(int networkId);
        PedInfo GetByName(string name);
        bool AddOrUpdate(PedInfo pedInfo);
    }
}