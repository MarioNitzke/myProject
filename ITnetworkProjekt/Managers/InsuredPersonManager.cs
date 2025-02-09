using System.Security.Claims;
using AutoMapper;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace ITnetworkProjekt.Managers
{
    public class InsuredPersonManager(IInsuredPersonRepository insuredPersonRepository,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        ILogger<InsuredPersonManager> logger
        )
    {
        private readonly IInsuredPersonRepository insuredPersonRepository = insuredPersonRepository ?? throw new ArgumentNullException(nameof(insuredPersonRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly UserManager<IdentityUser> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<InsuredPersonManager> logger = logger ?? throw new ArgumentNullException(nameof(logger));


        public async Task<InsuredPersonViewModel?> FindInsuredPersonById(int id)
        {
            logger.LogInformation("Finding insured person with ID {InsuredPersonId}.", id);
            InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(id);
            if (insuredPerson is null)
            {
                logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return null;
            }

            logger.LogInformation("Found insured person with ID {InsuredPersonId}.", id);
            return mapper.Map<InsuredPersonViewModel?>(insuredPerson);
        }

        public async Task<List<InsuredPersonViewModel>> GetAllInsuredPersons()
        {
            logger.LogInformation("Getting all insured persons.");
            List<InsuredPerson> insuredPersons = await insuredPersonRepository.GetAll();
            return mapper.Map<List<InsuredPersonViewModel>>(insuredPersons);
        }

        public async Task<InsuredPersonViewModel?> AddInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            logger.LogInformation("Attempting to add a new insured person.");

            InsuredPerson insuredPerson = mapper.Map<InsuredPerson>(insuredPersonViewModel);
            InsuredPerson addedInsuredPerson = await insuredPersonRepository.Insert(insuredPerson);

            logger.LogInformation("Adding insured person with ID {InsuredPersonId}.", addedInsuredPerson.Id);
            return mapper.Map<InsuredPersonViewModel>(addedInsuredPerson);
        }

        public async Task<InsuredPersonViewModel?> UpdateInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            InsuredPerson insuredPerson = mapper.Map<InsuredPerson>(insuredPersonViewModel);

            try
            {
                InsuredPerson updatedInsuredPerson = await insuredPersonRepository.Update(insuredPerson);
                logger.LogInformation("Updating insured person with ID {InsuredPersonId}.", updatedInsuredPerson.Id);
                return mapper.Map<InsuredPersonViewModel>(updatedInsuredPerson);
            }
            catch (InvalidOperationException)
            {   
                if (!await insuredPersonRepository.ExistsWithId(insuredPerson.Id))
                {
                    logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", insuredPerson.Id);
                    return null;
                }
                logger.LogError("An unexpected error occurred while updating insured person with ID {InsuredPersonId}.", insuredPerson.Id);
                throw;
            }
        }

        public async Task RemoveInsuredPersonWithId(int id)
        {
            logger.LogInformation("Attempting to remove insured person with ID {InsuredPersonId}.", id);
            InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(id);

            if (insuredPerson is not null)
            {
                await insuredPersonRepository.Delete(insuredPerson);
                logger.LogInformation("Insured person with ID {InsuredPersonId} removed.", id);
            }
            else
            {
                logger.LogWarning("Remove failed: insured person with ID {InsuredPersonId} not found.", id);
            }
        }

        public async Task<string?> GetPersonNameById(int id)
        {
            logger.LogInformation("Getting insured person name by ID {InsuredPersonId}.", id);

            InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(id);
            if (insuredPerson != null)
            {
                logger.LogInformation("Successfully retrieved name for insured person with ID: {InsuredPersonId}", id);
                return $"{insuredPerson.FirstName} {insuredPerson.LastName}";
            }

            logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
            return null;
        }

        public async Task<InsuredPersonViewModel?> GetInsuredPersonForUserAsync(ClaimsPrincipal user)
        {
            logger.LogInformation("Getting insured person for current user.");

            int? currentUserId = await insuredPersonRepository.GetInsuredPersonIdOfCurrentUserAsync(userManager.GetUserId(user));
            if (currentUserId is not null)
            {
                InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(currentUserId.Value);
                logger.LogInformation("Sucessfuly found insured person for user with userid: {UserId}.", currentUserId);
                return mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }

            logger.LogWarning("Insured person not found for user with userid: {UserId}.", currentUserId);
            return null;
        }

        public async Task<InsuredPersonViewModel?> GetInsuredPersonByEmailAndSSNAsync(string email, string ssn)
        {
            logger.LogInformation("Getting insured person by email and SSN.");
            
            
            InsuredPerson? insuredPerson = await insuredPersonRepository.FindByEmailAndSSNAsync(email, ssn);
            if(insuredPerson is not null)
            {
                logger.LogInformation("Successfully found insured person with email: {Email} and ssn: {Snn}", email, ssn);
                return mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }
            logger.LogWarning("Insured person not found with email: {Email} and ssn: {Ssn}", email, ssn);
            return null;
        }
    }
}
