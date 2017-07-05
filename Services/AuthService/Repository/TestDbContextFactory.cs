using System;
using AuthService.DbModels;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;

namespace AuthService.Repository
{
    public static class TestDbContextFactory
    {
        public static TestDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseMySQL(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
 
            var context = new TestDbContext(optionsBuilder.Options);
            context.Database.EnsureCreated();
 
            return context;
        }
    }
}