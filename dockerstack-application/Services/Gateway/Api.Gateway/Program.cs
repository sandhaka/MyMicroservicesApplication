using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Api.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    var certName = Environment.GetEnvironmentVariable("CERT_NAME");
                    var certPwdName = Environment.GetEnvironmentVariable("CERT_PWD_NAME");

                    var certPath = File.Exists(certName)
                        ? certName
                        : "/run/secrets/cert";

                    var certPwdPath = File.Exists(certPwdName)
                        ? certPwdName
                        : $"/run/secrets/cert_pwd";

                    options.Listen(IPAddress.Any, 443, listenOptions =>
                        {
                            listenOptions.UseHttps(certPath, File.ReadAllText(certPwdPath));
                        });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("https://*:443")
                .UseStartup<Startup>()
                .Build();
    }
}