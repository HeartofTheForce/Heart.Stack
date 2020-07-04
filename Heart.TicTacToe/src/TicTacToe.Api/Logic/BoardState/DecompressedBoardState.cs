using System;
using System.Text;

namespace TicTacToe.Api.Logic.BoardState
{
    public readonly struct DecompressedBoardState
    {
        public int BoardHistory { get; }
        public int CurrentTurn { get; }
        public int PlayerOneBoardSnapshot { get; }
        public int PlayerTwoBoardSnapshot { get; }
        public BoardResult BoardResult { get; }
        public BoardPlayer CurrentPlayer => (BoardPlayer)(CurrentTurn % 2);

        public DecompressedBoardState(int boardHistory)
        {
            BoardHistoryUtils.ValidateBoardHistory(
                boardHistory,
                out int currentTurn,
                out int playerOneBoardSnapshot,
                out int playerTwoBoardSnapshot,
                out var boardResult);

            BoardHistory = boardHistory;
            CurrentTurn = currentTurn;
            PlayerOneBoardSnapshot = playerOneBoardSnapshot;
            PlayerTwoBoardSnapshot = playerTwoBoardSnapshot;
            BoardResult = boardResult;
        }

        public TakeTurnResult TryTakeTurn(int x, int y, BoardPlayer player, out DecompressedBoardState newDecompressedState)
        {
            if (CurrentTurn == 9 || BoardResult != BoardResult.InProgress)
            {
                newDecompressedState = this;
                return TakeTurnResult.NotInProgress;
            }

            if (player != CurrentPlayer)
            {
                newDecompressedState = this;
                return TakeTurnResult.WrongPlayer;
            }

            int unsetIndex = -1;
            int targetIndex = y * 3 + x;
            int combinedBoardSnapshot = PlayerOneBoardSnapshot | PlayerTwoBoardSnapshot;
            for (int i = 0; i < 9; i++)
            {
                int snapshotIndex = 1 << i;

                if ((combinedBoardSnapshot & snapshotIndex) == 0)
                    unsetIndex++;

                if (targetIndex == i)
                {
                    if ((combinedBoardSnapshot & snapshotIndex) == snapshotIndex)
                    {
                        newDecompressedState = this;
                        return TakeTurnResult.TileAlreadySet;
                    }
                    else
                    {
                        int newState = BoardHistoryUtils.SetTurn(BoardHistory, CurrentTurn, unsetIndex);
                        newDecompressedState = new DecompressedBoardState(newState);
                        return TakeTurnResult.Success;
                    }
                }
            }

            newDecompressedState = this;
            return TakeTurnResult.UnknownFailure;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i = 0; i < 9; i++)
            {
                int snapshotIndex = 1 << i;

                bool playerOneSet = (PlayerOneBoardSnapshot & snapshotIndex) == snapshotIndex;
                bool playerTwoSet = (PlayerTwoBoardSnapshot & snapshotIndex) == snapshotIndex;

                if (playerOneSet && playerTwoSet)
                    throw new ArgumentException("Snapshot is corrupted");
                else if (playerOneSet)
                    sb.Append('X');
                else if (playerTwoSet)
                    sb.Append('O');
                else
                    sb.Append('-');

                if (i < 8 && i % 3 == 2)
                    sb.Append('\n');
            }

            return sb.ToString();
        }
    }

    public enum BoardResult
    {
        InProgress = 0,
        WinPlayerOne = 1,
        WinPlayerTwo = 2,
        Draw = 3,
    }

    public enum TakeTurnResult
    {
        Success = 0,
        NotInProgress = 1,
        TileAlreadySet = 2,
        WrongPlayer = 3,
        UnknownFailure = 4,
    }

    public enum BoardPlayer
    {
        PlayerOne = 0,
        PlayerTwo = 1
    }
}
