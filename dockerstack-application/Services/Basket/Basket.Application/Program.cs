using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Basket.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWehHost(args).Run();
        }

        private static IWebHost BuildWehHost(string[] args) => WebHost.CreateDefaultBuilder(args)
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