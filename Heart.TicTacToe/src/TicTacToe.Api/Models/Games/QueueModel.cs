using System;

namespace TicTacToe.Api.Models.Games
{
    public class QueueModel
    {
        public int? GameId { get; set; }
        public DateTimeOffset? InQueueSince { get; set; }
    }
}
