using System.Collections;
using System.Security.Claims;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuranceRepository(IDbContextFactory<ApplicationDbContext> dbContext) : BaseRepository<Insurance>(dbContext), IInsuranceRepository
    {
        //FindById Insurance with InsuredPerson
        public new async Task<Insurance?> FindById(int id)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            var insurance = await dbContext.Insurance
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (insurance == null)
            {
                return null;
            }

            var insuredPerson = await dbContext.InsuredPerson
                .Where(i => i.Id == insurance.InsuredPersonID)
                .ToListAsync();

            insurance.InsuredPerson = insuredPerson.FirstOrDefault();

            return insurance;
        }

        //GetAll Insurance with InsuredPersons
        public new async Task<List<Insurance>> GetAll()
        {
            using var dbContext = this.dbContext.CreateDbContext();
            var insurances = await dbContext.Insurance.AsNoTracking().ToListAsync();
            var insuredPersons = await dbContext.InsuredPerson.AsNoTracking().ToListAsync();

            foreach (var insurance in insurances)
            {
                insurance.InsuredPerson = insuredPersons.FirstOrDefault(p => p.Id == insurance.InsuredPersonID);
            }

            return insurances;
        }

        public async Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            return await dbContext.Insurance
                .AsNoTracking()
                .Where(i => insuranceIds.Contains(i.Id))
                .ToListAsync();
        }

        public async Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            var insuredPerson = dbContext.InsuredPerson.AsQueryable();

            if (selectedId.HasValue)
            {
                insuredPerson = insuredPerson.Where(p => p.Id == selectedId.Value);
            }

            return await insuredPerson.ToListAsync();
        }
 
    }
}
