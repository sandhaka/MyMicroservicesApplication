using System;
using AuthService.DbModels;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace AuthService.Repository
{
    /// <summary>
    /// Factory for contexts
    /// </summary>
    public static class DbContextFactory
    {
        /// <summary>
        /// Create a new context for 'TestDb'
        /// </summary>
        /// <returns>Db context</returns>
        public static UserDbContext CreateTestDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
            optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
 
            var context = new UserDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
 
            return context;
        }
    }
}