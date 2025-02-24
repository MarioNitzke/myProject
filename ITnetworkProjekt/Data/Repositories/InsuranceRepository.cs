using ITnetworkProjekt.Data;
using ITnetworkProjekt.Data.Entities;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Data.Repositories
{
    public class InsuranceRepository(
        IDbContextFactory<ApplicationDbContext> dbContext) : BaseRepository<Insurance>(dbContext), IInsuranceRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext = dbContext;

        // FindById Insurance with InsuredPerson
        public new async Task<Insurance?> FindById(int id)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var insurance = await dbContext.Insurance
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (insurance == null)
            {
                return null;
            }

            var insuredPerson = await dbContext.InsuredPerson
                .Where(i => i.Id == insurance.InsuredPersonId)
                .ToListAsync();

            insurance.InsuredPerson = insuredPerson.FirstOrDefault();
            return insurance;
        }

        // GetAll Insurance with InsuredPersons
        public new async Task<List<Insurance>> GetAll()
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();

            var insurances = await dbContext.Insurance.AsNoTracking().ToListAsync();
            var insuredPersons = await dbContext.InsuredPerson.AsNoTracking().ToListAsync();

            foreach (var insurance in insurances)
            {
                insurance.InsuredPerson = insuredPersons.FirstOrDefault(p => p.Id == insurance.InsuredPersonId);
            }

            return insurances;
        }

        public async Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var insurances = await dbContext.Insurance
                .AsNoTracking()
                .Where(i => insuranceIds.Contains(i.Id))
                .ToListAsync();

            return insurances;
        }

        public async Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();

            var insuredPerson = dbContext.InsuredPerson.AsQueryable();

            if (selectedId.HasValue)
            {
                insuredPerson = insuredPerson.Where(p => p.Id == selectedId.Value);
            }
            var result = await insuredPerson.ToListAsync();
            return result;
        }
    }
}
