using AuthService.Repository.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    /// <summary>
    /// Test class
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        
        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Unsecure content!");
        }

        [HttpGet("Secure")]
        [Authorize]
        public IActionResult GetSecure()
        {
            return Ok("Secure content!");
        }
    }
}