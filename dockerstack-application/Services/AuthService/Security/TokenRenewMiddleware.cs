using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthService.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthService.Security
{
    /// <summary>
    /// Middleware JWT token renew - Renew a valid token that is about to expire
    /// </summary>
    public class TokenRenewMiddleware : TokenMiddleware
    {
        public TokenRenewMiddleware(RequestDelegate next, IOptions<TokenOptions> options)
            : base(next, options)
        {
            ThrowIfInvalidOptions(Options);
        }

        /// <summary>
        /// Add to the base method the check about the user login status, 
        /// token renew is valid only for a authenticated user
        /// </summary>
        /// <param name="context">Http context</param>
        /// <param name="context">Db context</param>
        /// <returns>Next Task</returns>
        public override Task Invoke(HttpContext context, IdentityContext dbContext)
        {
            if (!context.Request.Path.Equals(Options.Path, StringComparison.Ordinal))
            {
                return Next(context);
            }
            
            // Token renew require an authenticated user
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return context.Response.WriteAsync("Unauthorized.");
            }
                
            return GenerateTokenAsync(context, dbContext);
        }

        /// <summary>
        /// Generate a new token from a valid one
        /// </summary>
        /// <param name="httpContext">Http context</param>
        /// <returns>Next Task</returns>
        protected override async Task GenerateTokenAsync(HttpContext httpContext, IdentityContext context)
        {
            // Retrieve the access token, skip 'Bearer ' string
            var encodedToken = httpContext.Request.Headers["Authorization"].ToString().Substring(7);
           
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var token = tokenHandler.ReadJwtToken(encodedToken);

            // Update token duration (+ 1 week)
            var jwt = new JwtSecurityToken(
                issuer: token.Issuer,
                audience: token.Audiences.First(),
                claims: token.Claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.Add(Options.Expiration),
                signingCredentials: token.SigningCredentials
            );

            var encodedJwt = tokenHandler.WriteToken(jwt);
            
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)Options.Expiration.TotalSeconds
            };
            
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response, SerializerSettings));
        }

        /// <summary>
        /// Check the options content
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private void ThrowIfInvalidOptions(TokenOptions options)
        {
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenOptions.Path));
            }
            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenOptions.Expiration));
            }
        }
    }
}