using System.Security.Claims;
using AutoMapper;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace ITnetworkProjekt.Managers
{
    public class InsuredPersonManager
    {
        private readonly IInsuredPersonRepository _insuredPersonRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<InsuredPersonManager> _logger;

        public InsuredPersonManager(
            IInsuredPersonRepository insuredPersonRepository,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            ILogger<InsuredPersonManager> logger)
        {
            _insuredPersonRepository = insuredPersonRepository;
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<InsuredPersonViewModel?> FindInsuredPersonById(int id)
        {
            _logger.LogInformation("Finding insured person with ID {InsuredPersonId}.", id);
            var insuredPerson = await _insuredPersonRepository.FindById(id);
            if (insuredPerson is null)
            {
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return null;
            }

            _logger.LogInformation("Found insured person with ID {InsuredPersonId}.", id);
            return _mapper.Map<InsuredPersonViewModel?>(insuredPerson);
        }

        public async Task<List<InsuredPersonViewModel>> GetAllInsuredPersons()
        {
            _logger.LogInformation("Getting all insured persons.");
            var insuredPersons = await _insuredPersonRepository.GetAll();
            return _mapper.Map<List<InsuredPersonViewModel>>(insuredPersons);
        }

        public async Task<InsuredPersonViewModel?> AddInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            _logger.LogInformation("Attempting to add a new insured person.");

            var insuredPerson = _mapper.Map<InsuredPerson>(insuredPersonViewModel);
            var addedInsuredPerson = await _insuredPersonRepository.Insert(insuredPerson);

            _logger.LogInformation("Adding insured person with ID {InsuredPersonId}.", addedInsuredPerson.Id);
            return _mapper.Map<InsuredPersonViewModel>(addedInsuredPerson);
        }

        public async Task<InsuredPersonViewModel?> UpdateInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            var insuredPerson = _mapper.Map<InsuredPerson>(insuredPersonViewModel);

            try
            {
                var updatedInsuredPerson = await _insuredPersonRepository.Update(insuredPerson);
                _logger.LogInformation("Updating insured person with ID {InsuredPersonId}.", updatedInsuredPerson.Id);
                return _mapper.Map<InsuredPersonViewModel>(updatedInsuredPerson);
            }
            catch (InvalidOperationException)
            {
                if (!await _insuredPersonRepository.ExistsWithId(insuredPerson.Id))
                {
                    _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", insuredPerson.Id);
                    return null;
                }

                _logger.LogError("An unexpected error occurred while updating insured person with ID {InsuredPersonId}.",
                    insuredPerson.Id);
                throw;
            }
        }

        public async Task RemoveInsuredPersonWithId(int id)
        {
            _logger.LogInformation("Attempting to remove insured person with ID {InsuredPersonId}.", id);
            var insuredPerson = await _insuredPersonRepository.FindById(id);

            if (insuredPerson is not null)
            {
                await _insuredPersonRepository.Delete(insuredPerson);
                _logger.LogInformation("Insured person with ID {InsuredPersonId} removed.", id);
            }
            else
            {
                _logger.LogWarning("Remove failed: insured person with ID {InsuredPersonId} not found.", id);
            }
        }

        public async Task<string?> GetPersonNameById(int id)
        {
            _logger.LogInformation("Getting insured person name by ID {InsuredPersonId}.", id);

            var insuredPerson = await _insuredPersonRepository.FindById(id);
            if (insuredPerson != null)
            {
                _logger.LogInformation("Successfully retrieved name for insured person with ID: {InsuredPersonId}", id);
                return $"{insuredPerson.FirstName} {insuredPerson.LastName}";
            }

            _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
            return null;
        }

        public async Task<InsuredPersonViewModel?> GetInsuredPersonForUserAsync(ClaimsPrincipal user)
        {
            _logger.LogInformation("Getting insured person for current user.");

            var currentUserId =
                await _insuredPersonRepository.GetInsuredPersonIdOfCurrentUserAsync(_userManager.GetUserId(user));
            if (currentUserId != null)
            {
                var insuredPerson = await _insuredPersonRepository.FindById(currentUserId.Value);
                _logger.LogInformation("Successfully found insured person for user with userid: {UserId}.", currentUserId);
                return _mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }

            _logger.LogWarning("Insured person not found for user with userid: {UserId}.", currentUserId);
            return null;
        }

        public async Task<InsuredPersonViewModel?> GetInsuredPersonByEmailAndSocialSecurityNumberAsync(string email,
            string socialSecurityNumber)
        {
            _logger.LogInformation("Getting insured person by email and SSN.");

            var insuredPerson = await _insuredPersonRepository.FindByEmailAndSocialSecurityNumberAsync(email, socialSecurityNumber);
            if (insuredPerson is not null)
            {
                _logger.LogInformation("Successfully found insured person with email: {Email} and socialSecurityNumber: {Snn}", email,
                    socialSecurityNumber);
                return _mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }

            _logger.LogWarning("Insured person not found with email: {Email} and socialSecurityNumber: {Snn}", email, socialSecurityNumber);
            return null;
        }
    }
}
