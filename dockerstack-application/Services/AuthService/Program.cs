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
            BuildWebHost(args).MigrateDbContext<IdentityContext>((context, services) =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("BuildWebHost");

                try
                {
                    new UserDbContextSeed().SeedAsync(context, loggerFactory).Wait();
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "An error occurred seeding the DB.");
                }
            }).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => 
        WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseKestrel(options =>
        {
            var certName = Environment.GetEnvironmentVariable("CERT_NAME");
            var certPwdName = Environment.GetEnvironmentVariable("CERT_PWD_NAME");

            var certPath = File.Exists(certName)
                ? certName
                : "/run/secrets/cert";

            var certPwdPath = File.Exists(certPwdName)
                ? certPwdName
                : "/run/secrets/cert_pwd";

            options.Listen(IPAddress.Any, 443, listenOptions =>
            {
                listenOptions.UseHttps(certPath, File.ReadAllText(certPwdPath));
            });
        }).Build();
    }
}