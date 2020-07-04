using System;

namespace TicTacToe.Db.Models
{
    public class MatchmakingEntry
    {
        public string PlayerId { get; set; } = default!;
        public DateTimeOffset LatestJoinDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        internal MatchmakingEntry() { }
        public MatchmakingEntry(string playerId, DateTimeOffset createdDate)
        {
            PlayerId = playerId;
            LatestJoinDate = createdDate;
            CreatedDate = createdDate;
        }
    }
}
