using System;
using TicTacToe.Api.Logic.BoardState;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Models.Games
{
    public class GameModel
    {
        public int Id { get; }

        public string? PlayerOne { get; }
        public string? PlayerTwo { get; }

        public string? CurrentTurnPlayer { get; }
        public string BoardState { get; }

        public string? GameResult { get; }
        public string GameState { get; }

        public DateTimeOffset LastActionDate { get; }
        public DateTimeOffset CreatedDate { get; }

        public GameModel(Game source)
        {
            if (source == null)
            {
                throw new ArgumentException($"{nameof(source)} cannot be null");
            }

            var decompressedBoardState = new DecompressedBoardState(source.BoardHistory);

            Id = source.Id;
            PlayerOne = source.PlayerOne;
            PlayerTwo = source.PlayerTwo;
            if (source.GameState == Db.Models.GameState.InProgress)
            {
                switch (decompressedBoardState.CurrentPlayer)
                {
                    case BoardPlayer.PlayerOne: CurrentTurnPlayer = source.PlayerOne; break;
                    case BoardPlayer.PlayerTwo: CurrentTurnPlayer = source.PlayerTwo; break;
                }
            }
            BoardState = decompressedBoardState.ToString();
            GameResult = source.GameResult;
            GameState = source.GameState;
            LastActionDate = source.LastActionDate;
            CreatedDate = source.CreatedDate;
        }
    }
}
