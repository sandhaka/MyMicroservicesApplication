using AuthService.Controllers.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    public class SecretsController : Controller
    {
        [HttpGet("public-key")]
        public string Get()
        {
            if(System.IO.File.Exists("/run/secrets/jwt-pub-key"))
                return System.IO.File.ReadAllText("/run/secrets/jwt-pub-key");
            return "File not found";
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            
        }
    }
}