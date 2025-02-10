using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Repositories
{
    public class InsuredPersonRepository(
        IDbContextFactory<ApplicationDbContext> dbContext,
        ILogger<InsuredPersonRepository> logger) : BaseRepository<InsuredPerson>(dbContext, logger), IInsuredPersonRepository
    {
        private readonly ILogger<InsuredPersonRepository> _logger = logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContext = dbContext;

        public async Task<InsuredPerson?> FindByEmailAndSocialSecurityNumberAsync(string email,
            string socialSecurityNumber)
        {
            _logger.LogInformation("Finding insured person with email {Email} and SSN {SSN}.", email, socialSecurityNumber);
            using var dbContext = _dbContext.CreateDbContext();
            var insuredPerson = await dbContext.InsuredPerson
                .FirstOrDefaultAsync(person =>
                    person.Email == email && person.SocialSecurityNumber == socialSecurityNumber);
            if (insuredPerson == null)
            {
                _logger.LogWarning("Insured person with email {Email} and SSN {SSN} not found.", email, socialSecurityNumber);
                return null;
            }

            _logger.LogInformation("Found insured person with email {Email} and SSN {SSN}.", email, socialSecurityNumber);
            return insuredPerson;
        }

        // FindById InsuredPerson with his InsuranceIds
        public new async Task<InsuredPerson?> FindById(int id)
        {
            _logger.LogInformation("Finding insured person with ID {InsuredPersonId}.", id);

            using var dbContext = _dbContext.CreateDbContext();
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
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return null;
            }

            _logger.LogInformation("Found insured person with ID {InsuredPersonId}.", id);
            return insuredPerson;
        }
    }
}
