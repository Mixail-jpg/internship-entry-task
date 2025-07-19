using System.ComponentModel.DataAnnotations;

namespace ticTacToeRestApi.Data.Entities
{
    public class Move
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }

        public Game Game { get; set; } = null!;

        public Guid PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public required string Symbol { get; set; }

        public required int Row { get; set; }
        public required int Column { get; set; }        

        public DateTime DateTimeOfMove { get; set; } = DateTime.UtcNow;
    }
}
