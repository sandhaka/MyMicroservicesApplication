using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Basket.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;
        
        public IdentityService(IHttpContextAccessor context)
        {
            _context = context;
        }
        
        public string GetUserIdentity()
        {
            var token = _context.HttpContext.User.FindFirst("access_token")?.Value;

            if (string.IsNullOrEmpty(token))
                return null;
            
            var tokenData = new JwtSecurityTokenHandler().ReadJwtToken(token);

            return tokenData.Claims.FirstOrDefault(i => i.Type.Equals("userId"))?.Value;
        }
    }
}