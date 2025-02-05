using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuredPersonRepository(IDbContextFactory<ApplicationDbContext> dbContext) : BaseRepository<InsuredPerson>(dbContext), IInsuredPersonRepository
    {
        //FindById InsuredPerson with his InsuranceIds
        public new async Task<InsuredPerson?> FindById(int id)
        {
            using var dbContext = this.dbContext.CreateDbContext();
            var insuredPerson = await dbContext.InsuredPerson
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new InsuredPerson
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    UserId = p.UserId,
                    SocialSecurityNumber = p.SocialSecurityNumber,
                    DateOfBirth = p.DateOfBirth,
                    Address = p.Address,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    CreatedDate = p.CreatedDate,
                    InsuranceIds = dbContext.Insurance
                        .Where(i => i.InsuredPersonID == p.Id)
                        .Select(i => i.Id)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return insuredPerson;
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
