using System.Threading.Tasks;
using AuthService.DbModels;
using Microsoft.AspNetCore.Http;

namespace AuthService.Security
{
    public interface ITokenMiddleware
    {
        Task Invoke(HttpContext httpContext, IdentityContext context);
    }
}