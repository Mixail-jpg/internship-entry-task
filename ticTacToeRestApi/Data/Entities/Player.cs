namespace ticTacToeRestApi.Data.Entities
{
    public class Player
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public List<Game> GamesAsX { get; set; } = new();
        public List<Game> GamesAsO { get; set; } = new();

        public List<Move>? Moves { get; set; }
    }
}
