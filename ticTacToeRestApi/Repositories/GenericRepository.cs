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

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _entities.AddAsync(entity);
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _entities.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _entities.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _entities.FindAsync(id);
            return entity ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID '{id}' was not found.");
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _entities.Update(entity);
            return Task.CompletedTask;
        }

    }
}
