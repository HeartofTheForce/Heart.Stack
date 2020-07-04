using System;

namespace TicTacToe.Api.Models.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public DateTimeOffset Expires { get; }

        public AuthResponse(
            string accessToken,
            string refreshToken,
            DateTimeOffset expires)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expires = expires;
        }
    }
}
