using System.Collections.Generic;
using System.Linq;
using AuthService.DbModels;

namespace AuthService.Repository.Users.Impl
{
    public class UserRepository : IUserRepository
    {
        public IDbUser GetDbModel(string userId)
        {
            var context = TestDbContextFactory.Create();
            
            return context.TBU_users.FirstOrDefault(i => i.id.Equals(userId));
        }

        public IEnumerable<IDbUser> GetAllUsers()
        {
            var context = TestDbContextFactory.Create();

            return context.TBU_users;
        }
    }
}