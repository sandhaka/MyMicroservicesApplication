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
            
            var prvtKeyPassphrase = File.ReadAllText("certificate/dev.boltjwt.passphrase");
            var privateKey = new X509Certificate2("certificate/dev.boltjwt.pfx", prvtKeyPassphrase).GetRSAPrivateKey();

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