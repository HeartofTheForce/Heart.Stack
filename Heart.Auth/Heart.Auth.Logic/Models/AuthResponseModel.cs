using Microsoft.AspNetCore.Identity;

namespace Heart.Auth.Logic.Models
{
    public class AuthResponseModel
    {
        public string AccessToken { get; }

        public AuthResponseModel(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}