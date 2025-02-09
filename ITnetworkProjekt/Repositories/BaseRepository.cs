using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ILogger<BaseRepository<TEntity>> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext;

        protected BaseRepository(
            IDbContextFactory<ApplicationDbContext> dbContext,
            ILogger<BaseRepository<TEntity>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Delete(TEntity entity)
        {
            _logger.LogInformation("Deleting entity of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = _dbContext.CreateDbContext();
            try
            {
                dbContext.Set<TEntity>().Remove(entity);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Entity of type {EntityType} deleted.", typeof(TEntity).Name);
            }
            catch
            {
                _logger.LogWarning("Failed to delete entity of type {EntityType}.", typeof(TEntity).Name);
                dbContext.Entry(entity).State = EntityState.Unchanged;
                throw;
            }
        }

        public async Task<bool> ExistsWithId(int id)
        {
            _logger.LogInformation("Checking existence of entity with ID {EntityId} of type {EntityType}.", id, typeof(TEntity).Name);

            bool exists = await FindById(id) is not null;

            _logger.LogInformation("Entity with ID {EntityId} of type {EntityType} {ExistenceStatus}.", id, typeof(TEntity).Name, exists ? "exists" : "does not exist");
            return exists;
        }

        public async Task<TEntity?> FindById(int id)
        {
            _logger.LogInformation("Attempting to find entity with ID {EntityId} of type {EntityType}.", id, typeof(TEntity).Name);

            using var dbContext = _dbContext.CreateDbContext();
            var entity = await dbContext.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                _logger.LogWarning("Entity with ID {EntityId} of type {EntityType} not found.", id, typeof(TEntity).Name);
            }
            else
            {
                _logger.LogInformation("Entity with ID {EntityId} of type {EntityType} successfully found.", id, typeof(TEntity).Name);
            }

            return entity;
        }

        public async Task<List<TEntity>> GetAll()
        {
            _logger.LogInformation("Fetching all entities of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = _dbContext.CreateDbContext();
            var entities = await dbContext.Set<TEntity>().ToListAsync();

            _logger.LogInformation("Fetched {EntityCount} entities of type {EntityType}.", entities.Count, typeof(TEntity).Name);
            return entities;
        }

        public async Task<TEntity> Insert(TEntity entity)
        {
            _logger.LogInformation("Inserting entity of type {EntityType}.", typeof(TEntity).Name);

            using var dbContext = _dbContext.CreateDbContext();
            var entry = dbContext.Set<TEntity>().Add(entity);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Entity of type {EntityType} inserted.", typeof(TEntity).Name);
            return entry.Entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            _logger.LogInformation("Updating entity of type {EntityType}.", typeof(TEntity).Name);
            using var dbContext = _dbContext.CreateDbContext();
            try
            {
                var entry = dbContext.Set<TEntity>().Update(entity);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Entity of type {EntityType} updated.", typeof(TEntity).Name);
                return entry.Entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency exception occurred while updating entity of type {EntityType}.", typeof(TEntity).Name);
                throw new InvalidOperationException();
            }
        }

        public async Task<int?> GetInsuredPersonIdOfCurrentUserAsync(string? userId)
        {
            _logger.LogInformation("Getting InsuredPerson ID of current user with ID {UserId}.", userId);

            using var dbContext = _dbContext.CreateDbContext();
            var personId = await dbContext.InsuredPerson
                .Where(m => m.UserId == userId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
            if (personId == 0)
            {
                _logger.LogWarning("InsuredPerson ID not found for user with ID {UserId}.", userId);
            }
            else
            {
                _logger.LogInformation("InsuredPerson ID {InsuredPersonId} found for user with ID {UserId}.", personId, userId);
            }
            return personId;
        }
    }
}
