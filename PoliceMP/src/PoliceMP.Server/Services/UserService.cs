using Microsoft.EntityFrameworkCore;
using PoliceMP.Data;
using PoliceMP.Data.Entities;
using System;
using System.Threading.Tasks;
using PoliceMP.Core.Server.Interfaces.Services;

namespace PoliceMP.Server.Services
{
    [Obsolete("UserService is deprecated", true)]
    public class UserService : IUserService
    {
        private readonly GtaDbContext _context;

        public UserService(GtaDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateAsync(string steamId)
        {
            if (await _context.Users.AnyAsync(u => u.SteamId == steamId))
                return null;

            var user = new User
            {
                SteamId = steamId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetBySteamIdAsync(string steamId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
        }
    }
}
