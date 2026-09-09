using System.Threading.Tasks;
using PoliceMP.Main.Data.Repositories.Interfaces;

namespace PoliceMP.Main.Data.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        public void Create(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public User Get(int id)
        {
            using (var context = new PoliceContext())
            {
                return context.Users.FirstOrDefault(u => u.UserId == id);
            }
        }

        public User Get(string steamId)
        {
            using (var context = new PoliceContext())
            {
                return context.Users.FirstOrDefault(u => u.SteamId == steamId);
            }
        }

        public void Update(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Update(user);
                context.SaveChanges();
            }
        }

        public void Delete(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public async Task CreateAsync(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task<User> GetAsync(int id)
        {
            using (var context = new PoliceContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            }
        }

        public async Task<User> GetAsync(string steamId)
        {
            using (var context = new PoliceContext())
            {
                return await context.Users.FirstOrDefaultAsync(u => u.SteamId == steamId);
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(User user)
        {
            using (var context = new PoliceContext())
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
