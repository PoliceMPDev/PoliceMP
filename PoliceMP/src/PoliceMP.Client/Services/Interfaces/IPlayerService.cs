using System.Collections.Generic;
using System.Threading.Tasks;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<List<int>> FetchAllPlayerNetworkIds();
        string FetchPlayerNameFromNetworkId(int networkId);
        Task<List<PlayerInfo>> FetchAllRecentPlayerInfo();
        PlayerInfo FetchCahcedPlayerInfoFromNetworkId(int networkId);
        Task<PlayerInfo> FetchPlayerInfoFromNetworkId(int networkId);
    }
}