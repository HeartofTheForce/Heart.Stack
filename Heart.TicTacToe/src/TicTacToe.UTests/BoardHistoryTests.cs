using System;
using TicTacToe.Api.Logic.BoardState;
using NUnit.Framework;

namespace TicTacToe.UTests
{
    [TestFixture]
    public class BoardHistoryTests
    {
        private static readonly int[] s_drawTurns = new int[] { 0, 0, 0, 0, 0, 3, 2, 1, 0 };
        private static readonly int[] s_quickWinPlayerOneTurns = new int[] { 0, 0, 1, 1, 2 };
        private static readonly int[] s_quickWinPlayerTwoTurns = new int[] { 0, 2, 0, 1, 4, 1 };

        [Test]
        public void ValidateBoardHistory_DrawTurns_Expect_Draw()
        {
            int boardHistory = 0;
            for (int i = 0; i < s_drawTurns.Length; i++)
            {
                boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_drawTurns[i]);
            }

            BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out var boardResult);

            Assert.AreEqual(BoardResult.Draw, boardResult);
        }

        [Test]
        public void ValidateBoardHistory_QuickWinPlayerOneTurns_Expect_WinPlayerOne()
        {
            int boardHistory = 0;
            for (int i = 0; i < s_quickWinPlayerOneTurns.Length; i++)
            {
                boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_quickWinPlayerOneTurns[i]);
            }

            BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out var boardResult);

            Assert.AreEqual(BoardResult.WinPlayerOne, boardResult);
        }

        [Test]
        public void ValidateBoardHistory_QuickWinPlayerTwoTurns_Expect_WinPlayerTwo()
        {
            int boardHistory = 0;
            for (int i = 0; i < s_quickWinPlayerTwoTurns.Length; i++)
            {
                boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_quickWinPlayerTwoTurns[i]);
            }

            BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out var boardResult);

            Assert.AreEqual(BoardResult.WinPlayerTwo, boardResult);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        // [TestCase(8)] Can't sequence break on final turn
        public void ValidateBoardHistory_SequenceBreakState_Expect_Error(int sequenceBreakTurnIndex)
        {
            int boardHistory = 0;
            for (int i = 0; i <= sequenceBreakTurnIndex + 1; i++)
            {
                if (i != sequenceBreakTurnIndex)
                {
                    boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_drawTurns[i]);
                }
            }

            var ex = Assert.Throws<ArgumentException>(() => BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out _));
            Assert.AreEqual("Sequence of turns is invalid", ex.Message);
        }

        [Test]
        public void ValidateBoardHistory_TurnsHistoryAfterEndingState_Expect_Error()
        {
            int boardHistory = 0;

            for (int i = 0; i < s_quickWinPlayerOneTurns.Length; i++)
            {
                boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_quickWinPlayerOneTurns[i]);
            }

            boardHistory = BoardHistoryUtils.SetTurn(boardHistory, s_quickWinPlayerOneTurns.Length + 1, 0);

            var ex = Assert.Throws<ArgumentException>(() => BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out _));
            Assert.AreEqual("Turn history exists after a ending state was reached", ex.Message);
        }

        [TestCase(0)]
        [TestCase(1)]
        // [TestCase(2)] Optimizations always truncate this turn into a valid range
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        // [TestCase(6)] Optimizations always truncate this turn into a valid range
        [TestCase(7)]
        // [TestCase(8)] Optimizations always truncate this turn into a valid range
        public void ValidateBoardHistory_DeltaIndexOutOfBoundsState_Expect_Error(int outOfBoundsTurnIndex)
        {
            int boardHistory = 0;
            for (int i = 0; i <= outOfBoundsTurnIndex; i++)
            {
                if (i == outOfBoundsTurnIndex)
                {
                    boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, 9 - i);
                }
                else
                {
                    boardHistory = BoardHistoryUtils.SetTurn(boardHistory, i, s_drawTurns[i]);
                }
            }

            var ex = Assert.Throws<ArgumentException>(() => BoardHistoryUtils.ValidateBoardHistory(boardHistory, out _, out _, out _, out _));
            Assert.AreEqual("Turn delta index is out of bounds", ex.Message);
        }

        [TestCase(0, 0b00000000000000000000000000001111)]
        [TestCase(1, 0b00000000000000000000000011110000)]
        [TestCase(2, 0b00000000000000000000011100000000)]
        [TestCase(3, 0b00000000000000000011100000000000)]
        [TestCase(4, 0b00000000000000011100000000000000)]
        [TestCase(5, 0b00000000000011100000000000000000)]
        [TestCase(6, 0b00000000001100000000000000000000)]
        [TestCase(7, 0b00000000110000000000000000000000)]
        [TestCase(8, 0b00000001000000000000000000000000)]
        public void SetTurn_MaxValue_Expect_Truncation(int turnIndex, int expectedBits)
        {
            int boardHistory = 0;
            boardHistory = BoardHistoryUtils.SetTurn(boardHistory, turnIndex, ~0 - 1);

            Assert.AreEqual(expectedBits, boardHistory);
        }
    }
}
