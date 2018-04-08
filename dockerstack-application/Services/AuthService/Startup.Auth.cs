using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using AuthService.Repository.Users;
using AuthService.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService
{
    public partial class Startup
    {
        private void ConfigureTokenProviderMiddleware(IApplicationBuilder app, IServiceProvider services)
        {
            var userRepository = services.GetRequiredService<IUserRepository>();

            var certName = Environment.GetEnvironmentVariable("CERT_NAME");
            var certPwdName = Environment.GetEnvironmentVariable("CERT_PWD_NAME");

            var certPath = File.Exists(certName)
                ? certName
                : "/run/secrets/cert";

            var certPwdPath = File.Exists(certPwdName)
                ? certPwdName
                : "/run/secrets/cert_pwd";

            var prvtKeyPassphrase = File.ReadAllText(certPwdPath);
            var privateKey = new X509Certificate2(certPath, prvtKeyPassphrase).GetRSAPrivateKey();

            // Setup Token provider
            var tokenProviderOptions = new TokenProviderOptions()
            {
                Path = "/api/token",
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256),
                IdentityResolver = UserRepository.GetIdentityAsync
            };
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(tokenProviderOptions));
            
            // Setup Token renew options
            var tokenRenewOptions = new TokenRenewOptions()
            {
                Path = "/api/tokenrenew"
            };
            app.UseMiddleware<TokenRenewMiddleware>(Options.Create(tokenRenewOptions));
        }
    }
}