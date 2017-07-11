using AuthService.DbModels.Impl;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DbModels
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) {}
        
        public DbSet<DbUser> TBU_users {get; set; }
    }
}
