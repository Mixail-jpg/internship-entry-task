using Microsoft.EntityFrameworkCore;
using ticTacToeRestApi.Data;
using ticTacToeRestApi.Interfaces;

namespace ticTacToeRestApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly GameDbContext _context;
        private readonly DbSet<T> _entities;

        public GenericRepository(GameDbContext context)
        {
            _context = context;
            _entities = context.Set<T>()!;
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public Task DeleteAsync(T entity)
        {
            _entities.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _entities.FindAsync(id);
        }

        public Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            return Task.CompletedTask;
        }
    }
}
