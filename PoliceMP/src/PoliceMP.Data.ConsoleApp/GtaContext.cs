using Microsoft.EntityFrameworkCore;
using PoliceMP.Data.Entities;

namespace PoliceMP.Data.ConsoleApp
{
    public class GtaContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=policemp.com;database=policempdev;user=FiveMServer;password=yUT5gU6c3RaEn5KxyUT5gU6c3RaEn5Kx");
        }
    }
}