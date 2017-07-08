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
        public static TestDbContext CreateTestDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
 
            var context = new TestDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
 
            return context;
        }
    }
}