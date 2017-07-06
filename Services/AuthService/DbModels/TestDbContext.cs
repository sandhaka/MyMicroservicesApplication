using AuthService.DbModels.Impl;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DbModels
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) {}
        
        public DbSet<DbUser> TBU_users {get; set; }
    }
}
