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
                options.Listen(IPAddress.Any, 80);
            })
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();
    }
}