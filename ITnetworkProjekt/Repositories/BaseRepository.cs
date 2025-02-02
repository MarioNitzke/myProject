using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITnetworkProjekt.Repositories
{
    public abstract class BaseRepository<TEntity>(ApplicationDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext dbContext = dbContext;
        private readonly DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

        public async Task Delete(TEntity entity)
        {
            try
            {
                dbSet.Remove(entity);
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
            return await FindById(id) is not null;
        }

        public async Task<TEntity?> FindById(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            EntityEntry<TEntity> entry = dbSet.Add(entity);
            await dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            try
            {
                EntityEntry<TEntity> entry = dbSet.Update(entity);
                await dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
