using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;

namespace ITnetworkProjekt.Repositories
{
    public abstract class BaseRepository<TEntity>(IDbContextFactory<ApplicationDbContext> dbContext,
        ILogger<BaseRepository<TEntity>> logger) : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ILogger<BaseRepository<TEntity>> logger = logger ?? throw new ArgumentNullException(nameof(logger));
        protected readonly IDbContextFactory<ApplicationDbContext> dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task Delete(TEntity entity)
        {
            logger.LogInformation("Deleting entity of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = this.dbContext.CreateDbContext();
            try
            {
                dbContext.Set<TEntity>().Remove(entity);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Entity of type {EntityType} deleted.", typeof(TEntity).Name);
            }
            catch
            {
                logger.LogWarning("Failed to delete entity of type {EntityType}.", typeof(TEntity).Name);
                dbContext.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<bool> ExistsWithId(int id)
        {
            logger.LogInformation("Checking existence of entity with ID {EntityId} of type {EntityType}.", id, typeof(TEntity).Name);

            bool exists = await FindById(id) is not null;

            logger.LogInformation("Entity with ID {EntityId} of type {EntityType} {ExistenceStatus}.", id, typeof(TEntity).Name, exists ? "exists" : "does not exist");
            return exists;
        }

        public async Task<TEntity?> FindById(int id)
        {
            logger.LogInformation("Attempting to find entity with ID {EntityId} of type {EntityType}.", id, typeof(TEntity).Name);

            using var dbContext = this.dbContext.CreateDbContext();
            var entity = await dbContext.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                logger.LogWarning("Entity with ID {EntityId} of type {EntityType} not found.", id, typeof(TEntity).Name);
            }
            else
            {
                logger.LogInformation("Entity with ID {EntityId} of type {EntityType} successfully found.", id, typeof(TEntity).Name);
            }

            return entity;
        }


        public async Task<List<TEntity>> GetAll()
        {
            logger.LogInformation("Fetching all entities of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = this.dbContext.CreateDbContext();
            var entities = await dbContext.Set<TEntity>().ToListAsync();

            logger.LogInformation("Fetched {EntityCount} entities of type {EntityType}.", entities.Count, typeof(TEntity).Name);
            return entities;
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            logger.LogInformation("Inserting entity of type {EntityType}.", typeof(TEntity).Name);

            using var dbContext = this.dbContext.CreateDbContext();
            var entry = dbContext.Set<TEntity>().Add(entity);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Entity of type {EntityType} inserted.", typeof(TEntity).Name);
            return entry.Entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            logger.LogInformation("Updating entity of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = this.dbContext.CreateDbContext();
            try
            {
                var entry = dbContext.Set<TEntity>().Update(entity);
                await dbContext.SaveChangesAsync();

                logger.LogInformation("Entity of type {EntityType} updated.", typeof(TEntity).Name);
                return entry.Entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Concurrency exception occurred while updating entity of type {EntityType}.", typeof(TEntity).Name);
                throw new InvalidOperationException();
            }
        }

        public async Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId)
        {
            logger.LogInformation("Getting InsuredPerson ID of current user with ID {UserId}.", userId);

            using var dbContext = this.dbContext.CreateDbContext();
            var personId = await dbContext.InsuredPerson
                .Where(m => m.UserId == userId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
            if (personId == 0)
            {
                logger.LogWarning("InsuredPerson ID not found for user with ID {UserId}.", userId);
            }
            else
            {
                logger.LogInformation("InsuredPerson ID {InsuredPersonId} found for user with ID {UserId}.", personId, userId);
            }
            return personId;
        }
    }
}
