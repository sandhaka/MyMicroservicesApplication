using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Security
{
    /// <summary>
    /// Middleware JWT provider
    /// </summary>
    public class TokenProviderMiddleware : TokenMiddleware
    {
        public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options)
            : base(next, options)
        {
            ThrowIfInvalidOptions(Options);
        }

        /// <summary>
        /// Generate and return a new access token after user credentials verification
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Next task</returns>
        /// <exception cref="InvalidCastException"></exception>
        protected override async Task GenerateTokenAsync(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            var opts = (Options as TokenProviderOptions);
            
            if(opts == null)
                throw new InvalidCastException("TokenProviderOptions");
            
            var identity = await opts.IdentityResolver(username, password);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;
          
            // Add standard claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await opts.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };
            
            // Add identity claims
            claims.AddRange(identity.Claims);
            
            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: opts.Issuer,
                audience: opts.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(opts.Expiration),
                signingCredentials: opts.SigningCredentials);
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)opts.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, SerializerSettings));
        }

        /// <summary>
        /// Check the options content
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void ThrowIfInvalidOptions(TokenOptions options)
        {
            TokenProviderOptions currentTokenOptions = options as TokenProviderOptions;

            if (currentTokenOptions == null)
            {
                throw new NullReferenceException("Wrong token provider options");
            }
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenOptions.Path));
            }
            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenOptions.Expiration));
            }
            if (currentTokenOptions.IdentityResolver == null)
            {
                throw new ArgumentNullException(nameof(currentTokenOptions.IdentityResolver));
            }
            if (currentTokenOptions.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(currentTokenOptions.SigningCredentials));
            }
            if (currentTokenOptions.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(currentTokenOptions.NonceGenerator));
            }
            if (string.IsNullOrEmpty(currentTokenOptions.Issuer))
            {
                throw new ArgumentNullException(nameof(currentTokenOptions.Issuer));
            }
            if (string.IsNullOrEmpty(currentTokenOptions.Audience))
            {
                throw new ArgumentNullException(nameof(currentTokenOptions.Audience));
            }
        }
    }
}