using Microsoft.EntityFrameworkCore;
using ticTacToeRestApi.Data;
using ticTacToeRestApi.Data.Entities;
using ticTacToeRestApi.Interfaces;  

namespace ticTacToeRestApi.Repositories
{
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository(GameDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Game?> GetGameWithMovesAsync(Guid gameId, CancellationToken cancellationToken = default)
        {
            return await _context.Games
                .Include(g => g.Moves)
                .Include(g => g.PlayerX)
                .Include(g => g.PlayerO)
                .Include(g => g.Winner)
                .FirstOrDefaultAsync(g => g.Id == gameId);
        }
    }
}
