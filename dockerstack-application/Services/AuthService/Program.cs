using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using AuthService.DbModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().MigrateDbContext<IdentityContext>((context, services) =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("BuildWebHost");

                try
                {
                    UserDbContextSeed.SeedAsync(context, loggerFactory).Wait();
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "An error occurred seeding the DB.");
                }
            }).Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) => 
        WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseKestrel(options =>
        {
            options.Listen(IPAddress.Any, 443, listenOptions =>
            {
                listenOptions.UseHttps("certificate/dev.boltjwt.pfx", File.ReadAllText("certificate/dev.boltjwt.passphrase"));
            });
        });
    }
}