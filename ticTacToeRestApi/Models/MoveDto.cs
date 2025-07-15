namespace ticTacToeRestApi.Models
{
    public class MoveDto
    {
        public Guid PlayerId { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
