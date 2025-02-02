using System.Collections;
using System.Security.Claims;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuranceRepository(ApplicationDbContext dbContext) : BaseRepository<Insurance>(dbContext), IInsuranceRepository
    {
        //FindById Insurance with InsuredPerson
        public new async Task<Insurance?> FindById(int id)
        {
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
            var insurances = await dbContext.Insurance.AsNoTracking().ToListAsync();
            var insuredPersons = await dbContext.InsuredPerson.AsNoTracking().ToListAsync();

            foreach (var insurance in insurances)
            {
                insurance.InsuredPerson = insuredPersons.FirstOrDefault(p => p.Id == insurance.InsuredPersonID);
            }

            return insurances;
        }

        public async Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId)
        {
            return await dbContext.InsuredPerson
                .Where(m => m.UserId == userId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null)
        {
            var insuredPerson = dbContext.InsuredPerson.AsQueryable();

            if (selectedId.HasValue)
            {
                insuredPerson = insuredPerson.Where(p => p.Id == selectedId.Value);
            }

            return await insuredPerson.ToListAsync();
        }
 
    }
}
