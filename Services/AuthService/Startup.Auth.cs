using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AuthService.Repository;
using AuthService.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
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
            var tokenProviderOptions = new TokenProviderOptions
            {
                Path = Configuration.GetSection("TokenAuthentication:TokenPath").Value,
                Audience = Configuration.GetSection("TokenAuthentication:Audience").Value,
                Issuer = Configuration.GetSection("TokenAuthentication:Issuer").Value,
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256),
                IdentityResolver = GetIdentity
            };
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(tokenProviderOptions));
        }
        
        /// <summary>
        /// This method check the user credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>User claims</returns>
        private static async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            ClaimsIdentity cIdentity = null;
            
            using (var context = TestDbContextFactory.Create())
            {
                var user = await context.TBU_users.FirstOrDefaultAsync(i => i.username == username);
                
                if (user != null)
                {
                    using (var md5Hash = MD5.Create())
                    {
                        var hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                        var sBuilder = new StringBuilder();
                        
                        foreach (var t in hash)
                        {
                            sBuilder.Append(t.ToString("x2"));
                        }
                        
                        if (user.password.Equals(sBuilder.ToString()))
                        {
                            cIdentity = new ClaimsIdentity(
                                new GenericIdentity(username, "Token"),
                                new Claim[]
                                {
                                    new Claim("Admin", user.level == 0 ? "true": "false")
                                });
                        }
                    }
                }
            }

            return cIdentity;
        }
    }
}