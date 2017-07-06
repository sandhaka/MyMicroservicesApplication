using System.Collections.Generic;
using AuthService.DbModels;

namespace AuthService.Repository.Users
{
    public interface IUserRepository
    {
        IDbUser GetDbModel(string userId);
        IEnumerable<IDbUser> GetAllUsers();
    }
}