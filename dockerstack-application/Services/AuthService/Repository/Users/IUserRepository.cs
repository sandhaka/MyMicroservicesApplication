using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Repository.Users
{
    public interface IUserRepository
    {
        Task<ClaimsIdentity> GetIdentityAsync(string username, string password);
    }
}