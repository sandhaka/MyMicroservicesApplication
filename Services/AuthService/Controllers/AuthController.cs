using AuthService.Repository.Users;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(UserRepository.GetAdmins());
        }
    }
}