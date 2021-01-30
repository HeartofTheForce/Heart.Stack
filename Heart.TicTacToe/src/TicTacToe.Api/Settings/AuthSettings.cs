namespace TicTacToe.Api.Settings
{
    public class AuthSettings
    {
        public const string RefreshTokenProvider = "TicTacToe.RefreshTokenProvider";
        public const string RefreshTokenName = "TicTacToe.RefreshToken";

        public string PrivateKey { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpireInMinutes { get; set; }
    }
}
