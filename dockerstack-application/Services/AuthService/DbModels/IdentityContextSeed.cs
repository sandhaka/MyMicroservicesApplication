using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AuthService.DbModels
{
    /// <summary>
    /// Init database with default values
    /// </summary>
    public class UserDbContextSeed
    {
        public async Task SeedAsync(IServiceProvider services, ILoggerFactory loggerFactory, int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                var context =
                    (IdentityContext) services.GetService(typeof(IdentityContext));

                if (!context.ApplicationUsers.Any())
                {
                    context.ApplicationUsers.AddRange(GetDefaultsUser());
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                var log = loggerFactory.CreateLogger("orders seed");
                log.LogError(exception.Message);
                
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    await SeedAsync(services, loggerFactory, retryForAvailability);
                }
            }
        }

        private AppUser GetDefaultsUser()
        {
            var md5Hascher = MD5.Create();
            var passwordHash = md5Hascher.ComputeHash(Encoding.UTF8.GetBytes("admin"));
            var sBuilder = new StringBuilder();
                        
            foreach (var t in passwordHash)
            {
                sBuilder.Append(t.ToString("x2"));
            }
            
            return new AppUser
            {
                id = Guid.NewGuid().ToString(),
                level = 0, // Admin permissions as default
                name_full = "Developer",
                username = "admin",
                password = sBuilder.ToString()
            };
        }
    }
}