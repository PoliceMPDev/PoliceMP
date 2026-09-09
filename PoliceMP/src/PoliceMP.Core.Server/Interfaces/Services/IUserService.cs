using System;
using System.Threading.Tasks;
using PoliceMP.Data.Entities;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    [Obsolete("UserService is deprecated", true)]
    public interface IUserService
    {
        Task<User> CreateAsync(string steamId);
        Task<User> GetBySteamIdAsync(string steamId);
    }
}
