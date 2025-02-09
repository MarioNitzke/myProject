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
                .FirstOrDefaultAsync(m =>
                    m.Email == email && m.SocialSecurityNumber == socialSecurityNumber);
            if (insuredPerson is null)
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
                        .Where(i => i.InsuredPersonId == p.Id)
                        .Select(i => i.Id)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (insuredPerson is null)
            {
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return null;
            }

            _logger.LogInformation("Found insured person with ID {InsuredPersonId}.", id);
            return insuredPerson;
        }
    }
}
