using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using AuthService.DbModels;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("BuildWebHost");
                
                try
                {
                    new UserDbContextSeed().SeedAsync(services, loggerFactory).Wait();
                }
                catch (Exception exception)
                {
                    
                    logger.LogError(exception, "An error occurred seeding the DB.");
                }
            }
            
            host.Run();
        }

        private static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.Listen(IPAddress.Loopback, 443, listenOptions =>
                {
                    listenOptions.UseHttps("certificate/certificate.pfx", "password");
                });
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseUrls("https://*:443")
            .UseStartup<Startup>()
            .Build();
    }
}