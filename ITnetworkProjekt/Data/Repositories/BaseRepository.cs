using ITnetworkProjekt.Data;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Data.Repositories
{
    public abstract class BaseRepository<TEntity>(
        IDbContextFactory<ApplicationDbContext> dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext = dbContext;

        public async Task Delete(TEntity entity)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            try
            {
                dbContext.Set<TEntity>().Remove(entity);
                await dbContext.SaveChangesAsync();
            }
            catch
            {
                dbContext.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<bool> ExistsWithId(int id)
        {
            bool exists = await FindById(id) is not null;
            return exists;
        }

        public async Task<TEntity?> FindById(int id)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var entity = await dbContext.Set<TEntity>().FindAsync(id);
            return entity;
        }

        public async Task<List<TEntity>> GetAll()
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var entities = await dbContext.Set<TEntity>().ToListAsync();
            return entities;
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var entry = dbContext.Set<TEntity>().Add(entity);
            await dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            try
            {
                var entry = dbContext.Set<TEntity>().Update(entity);
                await dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<int?> GetInsuredPersonIdOfCurrentUserAsync(string? userId)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var personId = await dbContext.InsuredPerson
                .Where(person => person.UserId == userId)
                .Select(person => person.Id)
                .FirstOrDefaultAsync();
            return personId;
        }
    }
}
