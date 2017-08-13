using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Basket.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options => options.UseHttps("certificate/certificate.pfx", "password"))
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("https://*:443")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}