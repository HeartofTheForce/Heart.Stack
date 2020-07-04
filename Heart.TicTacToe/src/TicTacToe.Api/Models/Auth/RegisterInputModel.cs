namespace TicTacToe.Api.Models.Auth
{
    public class RegisterInputModel
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Alias { get; set; } = default!;
    }
}
