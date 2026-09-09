using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PoliceMP.Data.Entities;

namespace PoliceMP.Data
{
    public class GtaDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        
        public DbSet<UserActivity> UserActivities { get; set; }
        
        public DbSet<WhatThreeWords> WhatThreeWords { get; set; }

        public GtaDbContext(DbContextOptions<GtaDbContext> options)
        : base(options)
        { }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await base.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
                Console.WriteLine("^1[PoliceMP] [Error] [GtaDbContext] ^7Failed to save changes.");
                throw;
            }
        }
    }
}
