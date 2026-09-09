using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using PoliceMP.Data.Entities;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    [Obsolete("SessionService is deprecated", true)]
    public interface ISessionService
    {
        Task EndAllAsync(string reason);
        Task<Session> StartAsync(Player player, User user);
        Task<bool> EndAsync(Player player, string reason);
        Task<TimeSpan> GetTotalPlaytime(Player player);
    }
}