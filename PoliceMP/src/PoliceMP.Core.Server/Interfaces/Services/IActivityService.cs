using System;
using System.Threading.Tasks;
using PoliceMP.Data.Entities;

namespace PoliceMP.Core.Server.Interfaces.Services
{
    [Obsolete("ActivityService is deprecated", true)]
    public interface IActivityService
    {
        Task<UserActivity> CreateAsync(string steamId, string steamName, bool isMod);
        Task<UserActivity> GetBySteamIdAsync(string steamId);
        Task UpdateSteamUserName(string steamId, string newSteamName);
        Task UpdateLastLogin(string steamId);
        Task IncrementPlayTime(string steamId);
        Task UpdateModStatus(string steamId, bool isMod);
    }
}