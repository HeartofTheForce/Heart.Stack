namespace TicTacToe.Api.Models.Auth
{
    public class LoginPasswordModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
