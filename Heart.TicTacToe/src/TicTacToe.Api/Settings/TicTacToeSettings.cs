namespace TicTacToe.Api.Settings
{
    public class TicTacToeSettings
    {
        public string ConnectionString { get; set; } = default!;
        public int StaleInMinutes { get; set; }
        public int MatchmakingIntervalInSeconds { get; set; }
        public int MatchmakingTimeoutInMinutes { get; set; }
    }
}
