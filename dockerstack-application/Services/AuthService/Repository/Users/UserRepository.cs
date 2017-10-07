using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AuthService.DbModels;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityContext _identityContext;

        public UserRepository(IdentityContext context)
        {
            _identityContext = context;
        }

        /// <summary>
        /// Check the user credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>User claims</returns>
        public async Task<ClaimsIdentity> GetIdentityAsync(string username, string password)
        {
            ClaimsIdentity cIdentity = null;
            {
                var user = await _identityContext.ApplicationUsers.FirstOrDefaultAsync(i => i.username == username);

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
                                    new Claim("isAdmin", user.level == 0 ? "true" : "false"),
                                    new Claim("userId", user.id),
                                    new Claim("username", user.username) 
                                });
                        }
                    }
                }

                return cIdentity;
            }
        }
    }
}