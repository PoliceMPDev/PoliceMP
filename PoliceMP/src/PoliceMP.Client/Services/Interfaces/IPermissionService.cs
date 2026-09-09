using System.Threading.Tasks;
using PoliceMP.Shared.Models;

namespace PoliceMP.Client.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<UserAces> GetUserAces();
        Task<UserAces> FetchUserAcesServer();
        UserRole CurrentUserRole { get; }
        void SetUserRole(UserRole userRole);
        UserRole GetUserRole(int targetNetId);
    }
}