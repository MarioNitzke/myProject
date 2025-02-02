using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuredPersonRepository(ApplicationDbContext dbContext) : BaseRepository<InsuredPerson>(dbContext), IInsuredPersonRepository
    {
        //FindById InsuredPerson with his insurances
        public new async Task<InsuredPerson?> FindById(int id)
        {
            var insuredPerson = await dbContext.InsuredPerson
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (insuredPerson == null)
                return null;

            var insurances = await dbContext.Insurance
                .AsNoTracking()
                .Where(i => i.InsuredPersonID == insuredPerson.Id)
                .ToListAsync();

            insuredPerson.Insurances = insurances;

            return insuredPerson;
        }

        public async Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId)
        {
            return await dbContext.InsuredPerson
                .Where(m => m.UserId == userId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
        }
    }
}
