using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Heart.Auth.Api.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok("Alive");
        }

        [HttpGet("api/claims")]
        public IActionResult GetClaims()
        {
            var output = User.Claims.Select(x => new
            {
                Type = x.Type,
                Value = x.Value
            });
            
            return Ok(output);
        }
    }
}