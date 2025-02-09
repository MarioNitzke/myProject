using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuranceRepository : BaseRepository<Insurance>, IInsuranceRepository
    {
        private readonly ILogger<InsuranceRepository> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext;

        public InsuranceRepository(
            IDbContextFactory<ApplicationDbContext> dbContext,
            ILogger<InsuranceRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // FindById Insurance with InsuredPerson
        public new async Task<Insurance?> FindById(int id)
        {
            _logger.LogInformation("Searching for insurance with ID {InsuranceId}.", id);
            using var dbContext = _dbContext.CreateDbContext();
            var insurance = await dbContext.Insurance
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (insurance == null)
            {
                _logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
                return null;
            }

            var insuredPerson = await dbContext.InsuredPerson
                .Where(i => i.Id == insurance.InsuredPersonId)
                .ToListAsync();

            insurance.InsuredPerson = insuredPerson.FirstOrDefault();
            _logger.LogInformation("Insurance with ID {InsuranceId} found and InsuredPerson with ID {InsuredPersonId} assigned to this insurance.", id, insurance.InsuredPersonId);

            return insurance;
        }

        // GetAll Insurance with InsuredPersons
        public new async Task<List<Insurance>> GetAll()
        {
            _logger.LogInformation("Fetching all insurances.");
            using var dbContext = _dbContext.CreateDbContext();

            var insurances = await dbContext.Insurance.AsNoTracking().ToListAsync();
            var insuredPersons = await dbContext.InsuredPerson.AsNoTracking().ToListAsync();

            foreach (var insurance in insurances)
            {
                insurance.InsuredPerson = insuredPersons.FirstOrDefault(p => p.Id == insurance.InsuredPersonId);
            }

            _logger.LogInformation("Fetched {InsuranceCount} insurances.", insurances.Count);
            return insurances;
        }

        public async Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            _logger.LogInformation("Fetching insurances for IDs: {InsuranceIds}.", string.Join(", ", insuranceIds));

            using var dbContext = _dbContext.CreateDbContext();
            var insurances = await dbContext.Insurance
                .AsNoTracking()
                .Where(i => insuranceIds.Contains(i.Id))
                .ToListAsync();
            if (insurances.Count == 0)
            {
                _logger.LogWarning("No insurances found for given IDs.");
            }
            else
            {
                _logger.LogInformation("Fetched {InsuranceCount} insurances for given IDs.", insurances.Count);
            }

            return insurances;
        }

        public async Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null)
        {
            _logger.LogInformation("Fetching insured persons. Filter by ID: {SelectedId}.", selectedId);
            using var dbContext = _dbContext.CreateDbContext();

            var insuredPerson = dbContext.InsuredPerson.AsQueryable();

            if (selectedId.HasValue)
            {
                insuredPerson = insuredPerson.Where(p => p.Id == selectedId.Value);
            }
            var result = await insuredPerson.ToListAsync();
            _logger.LogInformation("Fetched {InsuredPersonCount} insured persons.", result.Count);

            return result;
        }
    }
}
