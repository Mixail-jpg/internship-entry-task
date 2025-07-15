using ticTacToeRestApi.Data.Entities;

namespace ticTacToeRestApi.Interfaces
{
    public interface IGameRepository : IGenericRepository<Game>
    {
        Task<Game?> GetGameWithMovesAsync(Guid gameId);

    }
}
