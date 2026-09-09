using System.Threading.Tasks;

namespace PoliceMP.Main.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Create(User user);
        User Get(int id);
        User Get(string steamId);
        void Update(User user);
        void Delete(User user);

        Task CreateAsync(User user);
        Task<User> GetAsync(int id);
        Task<User> GetAsync(string steamId);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
