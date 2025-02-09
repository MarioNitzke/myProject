using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuranceRepository(IDbContextFactory<ApplicationDbContext> dbContext,
        ILogger<InsuranceRepository> logger) : BaseRepository<Insurance>(dbContext, logger), IInsuranceRepository
    {
        private readonly ILogger<InsuranceRepository> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        //FindById Insurance with InsuredPerson
        public new async Task<Insurance?> FindById(int id)
        {
            logger.LogInformation("Searching for insurance with ID {InsuranceId}.", id);
            using var dbContext = this.dbContext.CreateDbContext();
            var insurance = await dbContext.Insurance
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (insurance == null)
            {
                logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
                return null;
            }
            
            var insuredPerson = await dbContext.InsuredPerson
                .Where(i => i.Id == insurance.InsuredPersonID)
                .ToListAsync();
            
            insurance.InsuredPerson = insuredPerson.FirstOrDefault();
            logger.LogInformation("Insurance with ID {InsuranceId} found and InsuredPerson with Id {InsuredPersonId} assigned to this insurance", id, insurance.InsuredPersonID);

            return insurance;
        }

        //GetAll Insurance with InsuredPersons
        public new async Task<List<Insurance>> GetAll()
        {
            logger.LogInformation("Fetching all insurances.");
            using var dbContext = this.dbContext.CreateDbContext();

            var insurances = await dbContext.Insurance.AsNoTracking().ToListAsync();
            var insuredPersons = await dbContext.InsuredPerson.AsNoTracking().ToListAsync();

            foreach (var insurance in insurances)
            {
                insurance.InsuredPerson = insuredPersons.FirstOrDefault(p => p.Id == insurance.InsuredPersonID);
            }

            logger.LogInformation("Fetched {InsuranceCount} insurances.", insurances.Count);
            return insurances;
        }

        public async Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            logger.LogInformation("Fetching insurances for IDs: {InsuranceIds}.", string.Join(", ", insuranceIds));

            using var dbContext = this.dbContext.CreateDbContext();
            var insurances = await dbContext.Insurance
               .AsNoTracking()
               .Where(i => insuranceIds.Contains(i.Id))
               .ToListAsync();
            if (insurances.Count == 0)
            {
                logger.LogWarning("No insurances found for given IDs.");
            }
            else {
                logger.LogInformation("Fetched {InsuranceCount} insurances for given IDs.", insurances.Count);
            }

            return insurances;
        }

        public async Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null)
        {
            logger.LogInformation("Fetching insured persons. Filter by ID: {SelectedId}.", selectedId);
            using var dbContext = this.dbContext.CreateDbContext();

            var insuredPerson = dbContext.InsuredPerson.AsQueryable();

            if (selectedId.HasValue)
            {
                insuredPerson = insuredPerson.Where(p => p.Id == selectedId.Value);
            }
            var result = await insuredPerson.ToListAsync();
            logger.LogInformation("Fetched {InsuredPersonCount} insured persons.", result.Count);

            return result;
        }
 
    }
}
