using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuredPersonRepository(IDbContextFactory<ApplicationDbContext> dbContext, 
        ILogger<InsuredPersonRepository> logger) : BaseRepository<InsuredPerson>(dbContext, logger),
        IInsuredPersonRepository
    {
        private readonly ILogger<InsuredPersonRepository> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<InsuredPerson?> FindByEmailAndSSNAsync(string email, string ssn)
        {
            logger.LogInformation("Finding insured person with email {Email} and SSN {SSN}.", email, ssn);
            using var dbContext = this.dbContext.CreateDbContext();
            var insuredPerson = await dbContext.InsuredPerson
                .FirstOrDefaultAsync(m =>
                    (m.Email == email && m.SocialSecurityNumber == ssn));
            if(insuredPerson is null)
            {
                logger.LogWarning("Insured person with email {Email} and SSN {SSN} not found.", email, ssn);
                return null;
            }
            
            logger.LogInformation("Found insured person with email {Email} and SSN {SSN}.", email, ssn);
            return insuredPerson;
        }

        //FindById InsuredPerson with his InsuranceIds
        public new async Task<InsuredPerson?> FindById(int id)
        {
            logger.LogInformation("Finding insured person with ID {InsuredPersonId}.", id);

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

            if(insuredPerson is null)
            {
                logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return null;
            }

            logger.LogInformation("Found insured person with ID {InsuredPersonId}.", id);
            return insuredPerson;
        }
    }
}
