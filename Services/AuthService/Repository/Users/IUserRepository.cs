using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthService.DbModels;

namespace AuthService.Repository.Users
{
    public interface IUserRepository
    {
        Task<ClaimsIdentity> GetIdentityAsync(string username, string password);
    }
}