using ITnetworkProjekt.Data;
using ITnetworkProjekt.Data.Entities;
using ITnetworkProjekt.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Data.Repositories
{
    public class InsuredPersonRepository(
        IDbContextFactory<ApplicationDbContext> dbContext) : BaseRepository<InsuredPerson>(dbContext), IInsuredPersonRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext = dbContext;

        public async Task<InsuredPerson?> FindByEmailAndSocialSecurityNumberAsync(string email,
            string socialSecurityNumber)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var insuredPerson = await dbContext.InsuredPerson
                .FirstOrDefaultAsync(person =>
                    person.Email == email && person.SocialSecurityNumber == socialSecurityNumber);
            if (insuredPerson == null)
            {
                return null;
            }

            return insuredPerson;
        }

        // FindById InsuredPerson with his InsuranceIds
        public new async Task<InsuredPerson?> FindById(int id)
        {
            await using var dbContext = await _dbContext.CreateDbContextAsync();
            var insuredPerson = await dbContext.InsuredPerson
                .AsNoTracking()
                .Where(person => person.Id == id)
                .Select(person => new InsuredPerson
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    UserId = person.UserId,
                    SocialSecurityNumber = person.SocialSecurityNumber,
                    DateOfBirth = person.DateOfBirth,
                    Address = person.Address,
                    Email = person.Email,
                    PhoneNumber = person.PhoneNumber,
                    CreatedDate = person.CreatedDate,
                    InsuranceIds = dbContext.Insurance
                        .Where(insurance => insurance.InsuredPersonId == person.Id)
                        .Select(insurance => insurance.Id)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (insuredPerson == null)
            {
                return null;
            }

            return insuredPerson;
        }
    }
}
