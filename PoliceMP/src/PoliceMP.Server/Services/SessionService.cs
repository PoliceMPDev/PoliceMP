using CitizenFX.Core;
using Microsoft.EntityFrameworkCore;
using PoliceMP.Core.Shared;
using PoliceMP.Core.Shared.Constants;
using PoliceMP.Data;
using PoliceMP.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PoliceMP.Core.Server.Interfaces.Services;
using static System.String;

namespace PoliceMP.Server.Services
{
    public class SessionService //: ISessionService
    {
        private readonly ConcurrentDictionary<string, Session> _activeSessions;
        private readonly ILogger<SessionService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionService(ILogger<SessionService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _activeSessions = new ConcurrentDictionary<string, Session>();
        }

        public async Task EndAllAsync(string reason)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();
            await context.Sessions.Where(s => s.DisconnectedAt <= DateTime.MinValue).ForEachAsync(s =>
            {
                s.DisconnectedAt = DateTime.UtcNow;
                s.DisconnectedReason = "Server restarted.";
            });

            await context.SaveChangesAsync();
        }

        public async Task<Session> StartAsync(Player player, User user)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();

            if (user == null || user.UserId < 1) return null;

            await EndAsync(player, "Starting new session.");

            var session = new Session
            {
                UserId = user.UserId,
                PlayerName = player.Name,
                ConnectedAt = DateTime.UtcNow,
                IpAddress = player.Identifiers[Identifiers.IpAddress],
                License = player.Identifiers[Identifiers.License],
                Discord = player.Identifiers[Identifiers.Discord],
                Xbl = player.Identifiers[Identifiers.Xbl],
                LiveId = player.Identifiers[Identifiers.LiveId]
            };

            await context.Sessions.AddAsync(session);
            int affectedRows = await context.SaveChangesAsync();
            if (affectedRows < 1) return null;

            if (_activeSessions.TryAdd(user.SteamId, session)) return session;

            context.Sessions.Remove(session);
            await context.SaveChangesAsync();
            return null;
        }

        public async Task<TimeSpan> GetTotalPlaytime(Player player)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();

            var totalMs = await context.Sessions
                .Where(x => x.User.SteamId == player.Identifiers[Identifiers.Steam])
                .Where(x => x.DisconnectedAt != DateTime.MinValue)
                .SumAsync(x => (x.DisconnectedAt - x.ConnectedAt).TotalMilliseconds);

            return TimeSpan.FromMilliseconds(totalMs);
        }

        public async Task<bool> EndAsync(Player player, string reason)
        {
            if (IsNullOrWhiteSpace(player.Identifiers[Identifiers.Steam]))
            {
                return false;
            }
            
            if (!_activeSessions.TryRemove(player.Identifiers[Identifiers.Steam], out var session))
                return false;

            return await EndAsync(session, reason);
        }

        private async Task<bool> EndAsync(Session session, string reason)
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<GtaDbContext>();

            session.DisconnectedAt = DateTime.UtcNow;
            session.DisconnectedReason = reason;

            context.Update(session);
            int rowsAffected = await context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}