namespace ticTacToeRestApi.Models
{
    public class CreateGameDto
    {
        public Guid PlayerXId { get; set; }
        public Guid PlayerOId { get; set; }
        public int boardSize {  get; set; }
        public int winLength { get; set; }
    }
}
