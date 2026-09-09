using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IPedInfoCacheService
    {
        PedInfo GetByNetworkId(int networkId);
        PedInfo GetByName(string name);
        bool Cache(PedInfo pedInfo);
    }
}