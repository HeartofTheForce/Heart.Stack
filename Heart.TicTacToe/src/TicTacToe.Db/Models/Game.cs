using System;

namespace TicTacToe.Db.Models
{
    public class Game
    {
        public int Id { get; set; }

        public string? PlayerOne { get; set; }
        public string? PlayerTwo { get; set; }

        public int BoardHistory { get; set; }

        public string? GameResult { get; set; }
        public string GameState { get; set; } = default!;

        public DateTimeOffset LastActionDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        internal Game() { }

        public Game(string playerOne, string playerTwo, string gameState, DateTime createdDate)
        {
            PlayerOne = playerOne;
            PlayerTwo = playerTwo;
            GameState = gameState;
            LastActionDate = createdDate;
            CreatedDate = createdDate;
        }
    }

    public static class GameState
    {
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Aborted = "Aborted";
    }

    public static class GameResult
    {
        public const string WinPlayerOne = "Winner Player One";
        public const string WinPlayerTwo = "Winner Player Two";
        public const string Draw = "Draw";
    }
}
