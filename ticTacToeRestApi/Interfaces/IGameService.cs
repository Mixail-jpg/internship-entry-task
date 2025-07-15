using ticTacToeRestApi.Data.Entities;

namespace ticTacToeRestApi.Interfaces
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(Guid playerXid, Guid playerOid, int boardSize, int winLength);
        Task<Move> MakeMoveAsync(Guid gameId, Guid playerId, int row, int column);
        Task<Game?> GetGameAsync(Guid gameId);
    }
}
