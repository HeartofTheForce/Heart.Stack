using System;

namespace TicTacToe.Api.Logic.BoardState
{
    public static class BoardHistoryUtils
    {
        private static readonly int[] s_turnShifts = new int[]
        {
            0,  //Turn 0
            4,  //Turn 1
            8,  //Turn 2
            11, //Turn 3
            14, //Turn 4
            17, //Turn 5
            20, //Turn 6
            22, //Turn 7
            24, //Turn 8
        };

        private static readonly int[] s_turnBits = new int[] {
            0b00000000000000000000000000001111, //Turn 0
            0b00000000000000000000000011110000, //Turn 1
            0b00000000000000000000011100000000, //Turn 2
            0b00000000000000000011100000000000, //Turn 3
            0b00000000000000011100000000000000, //Turn 4
            0b00000000000011100000000000000000, //Turn 5
            0b00000000001100000000000000000000, //Turn 6
            0b00000000110000000000000000000000, //Turn 7
            0b00000001000000000000000000000000, //Turn 8
        };

        private static readonly int[] s_winningStates = new int[]
        {
            //XXX
            //---
            //---
            0b00000000000000000000000000000111,
            //---
            //XXX
            //---
            0b00000000000000000000000000111000,
            //---
            //---
            //XXX
            0b00000000000000000000000111000000,
            //X--
            //X--
            //X--
            0b00000000000000000000000001001001,
            //-X-
            //-X-
            //-X-
            0b00000000000000000000000010010010,
            //--X
            //--X
            //--X
            0b00000000000000000000000100100100,
            //X--
            //-X-
            //--X
            0b00000000000000000000000100010001,
            //--X
            //-X-
            //X--
            0b00000000000000000000000001010100,
        };



        public static void ValidateBoardHistory(
            int boardHistory,
            out int currentTurn,
            out int playerOneBoardSnapshot,
            out int playerTwoBoardSnapshot,
            out BoardResult boardResult)
        {
            currentTurn = 0;

            playerOneBoardSnapshot = 0;
            playerTwoBoardSnapshot = 0;

            boardResult = BoardResult.InProgress;

            bool checkInvalidSequence = false;
            for (int i = 0; i < 9; i++)
            {
                int deltaIndex = GetDeltaIndexByTurn(boardHistory, i);

                if (deltaIndex == -1)
                {
                    checkInvalidSequence = true;
                    continue;
                }

                if (boardResult != BoardResult.InProgress)
                    throw new ArgumentException("Turn history exists after a ending state was reached");

                if (checkInvalidSequence)
                    throw new ArgumentException("Sequence of turns is invalid");

                if (IsDeltaIndexOutOfBounds(i, deltaIndex))
                    throw new ArgumentException("Turn delta index is out of bounds");

                currentTurn++;

                int unsetIndex = -1;
                for (int j = 0; j < 9; j++)
                {
                    int snapshotIndex = 1 << j;

                    int combinedBoardSnapshot = playerOneBoardSnapshot | playerTwoBoardSnapshot;
                    if ((combinedBoardSnapshot & snapshotIndex) == 0)
                        unsetIndex++;

                    if (deltaIndex == unsetIndex)
                    {
                        if (i % 2 == 0)
                        {
                            playerOneBoardSnapshot |= snapshotIndex;
                            if (IsSnapshotInWinningState(playerOneBoardSnapshot))
                                boardResult = BoardResult.WinPlayerOne;
                        }
                        else
                        {
                            playerTwoBoardSnapshot |= snapshotIndex;
                            if (IsSnapshotInWinningState(playerTwoBoardSnapshot))
                                boardResult = BoardResult.WinPlayerTwo;
                        }

                        break;
                    }
                }
            }

            if (currentTurn == 9 && boardResult == BoardResult.InProgress)
                boardResult = BoardResult.Draw;
        }

        public static bool IsSnapshotInWinningState(int snapshot)
        {
            for (int i = 0; i < s_winningStates.Length; i++)
            {
                if ((snapshot & s_winningStates[i]) == s_winningStates[i])
                    return true;
            }

            return false;
        }

        public static int GetDeltaIndexByTurn(int boardHistory, int turn)
        {
            int trueValue = (boardHistory & s_turnBits[turn]) >> s_turnShifts[turn];
            return trueValue - 1;
        }

        public static int SetTurn(int boardHistory, int turn, int deltaIndex)
        {
            int trueValue = deltaIndex + 1;
            return (boardHistory & ~s_turnBits[turn]) | ((trueValue << s_turnShifts[turn]) & s_turnBits[turn]);
        }

        public static bool IsDeltaIndexOutOfBounds(int turn, int deltaIndex)
        {
            int trueValue = deltaIndex + 1;
            return turn + trueValue > 9 || trueValue < 0;
        }
    }
}
