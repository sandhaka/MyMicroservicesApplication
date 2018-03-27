using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthService.DbModels;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Security
{
    public class TokenProviderOptions : TokenOptions
    {
        /// <summary>
        ///  The Issuer (iss) claim for generated tokens.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The Audience (aud) claim for the generated tokens.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
        
        /// <summary>
        /// Generates a random value (nonce) for each generated token.
        /// </summary>
        /// <remarks>The default nonce is a random GUID.</remarks>
        public Func<Task<string>> NonceGenerator { get; set; }
            = () => Task.FromResult(Guid.NewGuid().ToString());
        
        /// <summary>
        /// Resolves a user identity given a username and password.
        /// </summary>
        public Func<IdentityContext, string, string, Task<ClaimsIdentity>> IdentityResolver { get; set; }
    }
}