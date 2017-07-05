using System.Collections.Generic;
using System.Linq;
using AuthService.DbModels;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AuthService.Repository.Users
{
    public class UserRepository
    {
        public static DbUser GetDbModel(string userId)
        {
            var context = TestDbContextFactory.Create();
            
            return context.TBU_users.FirstOrDefault(i => i.id.Equals(userId));
        }
        
        public static IEnumerable<DbUser> GetAdmins()
        {
            var context = TestDbContextFactory.Create();
            
            return context.TBU_users.Where(i => i.level == 0);
        }
    }
}