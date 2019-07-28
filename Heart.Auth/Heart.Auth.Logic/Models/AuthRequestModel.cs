using Microsoft.AspNetCore.Identity;

namespace Heart.Auth.Logic.Models
{
    public class AuthRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}