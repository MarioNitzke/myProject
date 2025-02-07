using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ITnetworkProjekt.Repositories
{
    public abstract class BaseRepository<TEntity>(IDbContextFactory<ApplicationDbContext> dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IDbContextFactory<ApplicationDbContext> dbContext = dbContext;
 
        public async Task Delete(TEntity entity)
        {
            using var dbContext = this.dbContext.CreateDbContext();
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
            return await FindById(id) is not null;
        }

        public async Task<TEntity?> FindById(int id)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            return await dbContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<List<TEntity>> GetAll()
        {
            using var dbContext = this.dbContext.CreateDbContext();
            return await dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            var entry = dbContext.Set<TEntity>().Add(entity);
            await dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            try
            {
                var entry = dbContext.Set<TEntity>().Update(entity);
                await dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            return await dbContext.InsuredPerson
                .Where(m => m.UserId == userId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
        }
    }
}
