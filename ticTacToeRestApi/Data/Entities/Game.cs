namespace ticTacToeRestApi.Data.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public Guid PlayerXId { get; set; }
        public Player PlayerX { get; set; }

        public Guid PlayerOId { get; set; }
        public Player PlayerO { get; set; }

        public required int BoardSize { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime EndAt { get; set; }

        public required string CurrentTurn { get; set; } = "X"; // X or O

        public required int WinLength { get; set; } // Added this property because it seems right
        public Guid? WinnerId { get; set; } 
        public Player? Winner {  get; set; }

        public List<Move> Moves { get; set; } = new();
    }
}
