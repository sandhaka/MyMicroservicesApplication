using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AuthService.Security
{
    public interface ITokenMiddleware
    {
        Task Invoke(HttpContext httpContext);
    }
}