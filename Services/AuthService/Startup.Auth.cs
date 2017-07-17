using System;
using System.Security.Cryptography;
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
        private RSA _publicKey;
        private RSA _privateKey;
        
        private void ConfigureAuth(IApplicationBuilder app)
        {
            var userRepository = app.ApplicationServices.GetRequiredService<IUserRepository>();
            
            // Get private and public keys
            _publicKey = new X509Certificate2("keys/saml.crt").GetRSAPublicKey();
            _privateKey = new X509Certificate2("keys/certificate.pfx").GetRSAPrivateKey();

            // Setup Token validation
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(_publicKey),
                ValidateIssuer = true,
                ValidIssuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                ValidateAudience = true,
                ValidAudience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            // Setup Token provider
            var tokenProviderOptions = new TokenProviderOptions()
            {
                Path = "/api/token",
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256),
                IdentityResolver = userRepository.GetIdentityAsync
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