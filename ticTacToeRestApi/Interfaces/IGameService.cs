using ticTacToeRestApi.Data.Entities;

namespace ticTacToeRestApi.Interfaces
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(Guid playerXid, Guid playerOid, int boardSize, int winLength, CancellationToken cancellation = default);
        Task<Move> MakeMoveAsync(Guid gameId, Guid playerId, int row, int column, CancellationToken cancellation = default);
        Task<Game?> GetGameAsync(Guid gameId, CancellationToken cancellation = default);
    }
}
