using TicTacToe.Api.Logic.BoardState;
using NUnit.Framework;

namespace TicTacToe.UTests
{
    [TestFixture]
    public class DecompressedBoardStateTests
    {
        private static readonly (int x, int y, BoardPlayer player)[] s_drawTurns = new (int, int, BoardPlayer)[]
        {
            (0,0, BoardPlayer.PlayerOne),
            (1,0, BoardPlayer.PlayerTwo),
            (2,0, BoardPlayer.PlayerOne),
            (0,1, BoardPlayer.PlayerTwo),
            (1,1, BoardPlayer.PlayerOne),
            (2,2, BoardPlayer.PlayerTwo),
            (1,2, BoardPlayer.PlayerOne),
            (0,2, BoardPlayer.PlayerTwo),
            (2,1, BoardPlayer.PlayerOne),
        };

        private static readonly (int x, int y, BoardPlayer player)[] s_quickWinPlayerOneTurns = new (int, int, BoardPlayer)[]
        {
            (0,0, BoardPlayer.PlayerOne),
            (0,1, BoardPlayer.PlayerTwo),
            (1,0, BoardPlayer.PlayerOne),
            (1,1, BoardPlayer.PlayerTwo),
            (2,0, BoardPlayer.PlayerOne),
        };

        private static readonly (int x, int y, BoardPlayer player)[] s_quickWinPlayerTwoTurns = new (int, int, BoardPlayer)[]
        {
            (0,0, BoardPlayer.PlayerOne),
            (0,1, BoardPlayer.PlayerTwo),
            (1,0, BoardPlayer.PlayerOne),
            (1,1, BoardPlayer.PlayerTwo),
            (2,2, BoardPlayer.PlayerOne),
            (2,1, BoardPlayer.PlayerTwo),
        };

        [Test]
        public void TryTakeTurn_DrawTurns_Expect_Draw()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);

            for (int i = 0; i < s_drawTurns.Length; i++)
            {
                var (x, y, player) = s_drawTurns[i];
                decompressedBoardState.TryTakeTurn(x, y, player, out decompressedBoardState);
            }

            Assert.AreEqual(BoardResult.Draw, decompressedBoardState.BoardResult);
        }

        [Test]
        public void TryTakeTurn_QuickWinPlayerOneTurns_Expect_WinPlayerOne()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);

            for (int i = 0; i < s_quickWinPlayerOneTurns.Length; i++)
            {
                var (x, y, player) = s_quickWinPlayerOneTurns[i];
                decompressedBoardState.TryTakeTurn(x, y, player, out decompressedBoardState);
            }

            Assert.AreEqual(BoardResult.WinPlayerOne, decompressedBoardState.BoardResult);
        }

        [Test]
        public void TryTakeTurn_QuickWinPlayerTwoTurns_Expect_WinPlayerTwo()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);

            for (int i = 0; i < s_quickWinPlayerTwoTurns.Length; i++)
            {
                var (x, y, player) = s_quickWinPlayerTwoTurns[i];
                decompressedBoardState.TryTakeTurn(x, y, player, out decompressedBoardState);
            }

            Assert.AreEqual(BoardResult.WinPlayerTwo, decompressedBoardState.BoardResult);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        // [TestCase(8)] BoardResult is Draw on final turn
        public void TryTakeTurn_UnfinishedTurns_Expect_Draw(int upUntilTurnIndex)
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);

            for (int i = 0; i <= upUntilTurnIndex; i++)
            {
                var (x, y, player) = s_drawTurns[i];
                decompressedBoardState.TryTakeTurn(x, y, player, out decompressedBoardState);
            }

            Assert.AreEqual(BoardResult.InProgress, decompressedBoardState.BoardResult);
        }

        [Test]
        public void TryTakeTurn_OutOfOrderPlayerTwo_Expect_WrongPlayer()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);
            var takeTurnResult = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerTwo, out _);

            Assert.AreEqual(TakeTurnResult.WrongPlayer, takeTurnResult);
        }

        [Test]
        public void TryTakeTurn_OutOfOrderPlayerOne_Expect_WrongPlayer()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);
            var takeTurnResult1 = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerOne, out decompressedBoardState);
            var takeTurnResult2 = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerOne, out _);

            Assert.AreEqual(TakeTurnResult.Success, takeTurnResult1);
            Assert.AreEqual(TakeTurnResult.WrongPlayer, takeTurnResult2);
        }

        [Test]
        public void TryTakeTurn_InOrder_Expect_Success()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);
            var takeTurnResult1 = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerOne, out decompressedBoardState);
            var takeTurnResult2 = decompressedBoardState.TryTakeTurn(0, 1, BoardPlayer.PlayerTwo, out _);

            Assert.AreEqual(TakeTurnResult.Success, takeTurnResult1);
            Assert.AreEqual(TakeTurnResult.Success, takeTurnResult2);
        }

        [Test]
        public void TryTakeTurn_AlreadySetTile_Expect_TileAlreadySet()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);
            var takeTurnResult1 = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerOne, out decompressedBoardState);
            var takeTurnResult2 = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerTwo, out _);

            Assert.AreEqual(TakeTurnResult.Success, takeTurnResult1);
            Assert.AreEqual(TakeTurnResult.TileAlreadySet, takeTurnResult2);
        }

        [Test]
        public void TryTakeTurn_NotInProgress_Expect_NotInProgress()
        {
            int boardHistory = 0;
            var decompressedBoardState = new DecompressedBoardState(boardHistory);
            for (int i = 0; i < s_quickWinPlayerOneTurns.Length; i++)
            {
                var (x, y, player) = s_quickWinPlayerOneTurns[i];
                decompressedBoardState.TryTakeTurn(x, y, player, out decompressedBoardState);
            }

            var takeTurnResult = decompressedBoardState.TryTakeTurn(0, 0, BoardPlayer.PlayerTwo, out _);

            Assert.AreEqual(TakeTurnResult.NotInProgress, takeTurnResult);
        }
    }
}
