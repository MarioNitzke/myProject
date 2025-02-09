using System.Security.Claims;
using AutoMapper;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITnetworkProjekt.Managers
{
    public class InsuranceManager(
        IInsuranceRepository insuranceRepository,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        ILogger<InsuranceManager> logger)
    {
        private readonly IInsuranceRepository _insuranceRepository = insuranceRepository;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly ILogger<InsuranceManager> _logger = logger;

        public async Task<InsuranceViewModel?> FindInsuranceById(int id)
        {
            _logger.LogInformation("Searching for insurance with ID {InsuranceId}.", id);

            var insurance = await _insuranceRepository.FindById(id);
            if (insurance == null)
            {
                _logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
            }

            return _mapper.Map<InsuranceViewModel?>(insurance);
        }

        public async Task<List<InsuranceViewModel>> GetAllInsurances()
        {
            _logger.LogInformation("Fetching all insurances.");
            var insurances = await _insuranceRepository.GetAll();
            return _mapper.Map<List<InsuranceViewModel>>(insurances);
        }

        public async Task<InsuranceViewModel?> AddInsurance(InsuranceViewModel insuranceViewModel)
        {
            _logger.LogInformation("Adding new insurance for insured person ID {InsuredPersonId}.",
                insuranceViewModel.InsuredPersonId);

            var insurance = _mapper.Map<Insurance>(insuranceViewModel);
            var addedInsurance = await _insuranceRepository.Insert(insurance);

            _logger.LogInformation("Insurance successfully added with ID {InsuranceId}.", addedInsurance.Id);
            return _mapper.Map<InsuranceViewModel>(addedInsurance);
        }

        public async Task<InsuranceViewModel?> UpdateInsurance(InsuranceViewModel insuranceViewModel)
        {
            _logger.LogInformation("Updating insurance with ID {InsuranceId}.", insuranceViewModel.Id);

            var insurance = _mapper.Map<Insurance>(insuranceViewModel);

            try
            {
                var updatedInsurance = await _insuranceRepository.Update(insurance);
                _logger.LogInformation("Insurance with ID {InsuranceId} successfully updated.", updatedInsurance.Id);
                return _mapper.Map<InsuranceViewModel>(updatedInsurance);
            }
            catch (InvalidOperationException)
            {
                var result = await _insuranceRepository.ExistsWithId(insurance.Id);
                if (!result)
                {
                    _logger.LogWarning("Attempted to update non-existent insurance with ID {InsuranceId}.",
                        insurance.Id);
                    return null;
                }

                throw;
            }
        }

        public async Task RemoveInsuranceWithId(int id)
        {
            _logger.LogInformation("Attempting to remove insurance with ID {InsuranceId}.", id);
            var insurance = await _insuranceRepository.FindById(id);

            if (insurance is not null)
            {
                _logger.LogInformation("Insurance with ID {InsuranceId} deleted.", id);
                await _insuranceRepository.Delete(insurance);
            }
            else
            {
                _logger.LogWarning("Attempted to remove non-existent insurance with ID {InsuranceId}.", id);
            }
        }

        public async Task<InsuranceViewModel?> GetInsurancesForLoggedUserAsync(int id, ClaimsPrincipal user)
        {
            _logger.LogInformation("Try to fetch insurance with ID {InsuranceId} for logged user.", id);
            Insurance? insurance;

            if (user.IsInRole(UserRoles.Admin))
            {
                _logger.LogInformation("User is admin, fetching insurance with ID {InsuranceId}.", id);
                insurance = await _insuranceRepository.FindById(id);
                return _mapper.Map<InsuranceViewModel?>(insurance);
            }

            int? currentUserId =
                await _insuranceRepository.GetInsuredPersonIdOfCurrentUserAsync(_userManager.GetUserId(user));
            if (currentUserId == null)
            {
                _logger.LogWarning("Current user does not have a valid insured person ID. Insurance do not show.");
                return null;
            }

            insurance = await _insuranceRepository.FindById(id);
            if (insurance != null && insurance.InsuredPersonId == currentUserId)
            {
                _logger.LogInformation(
                    "Insurance with ID {InsuranceId} found for current user with userID {CurrentUserId}.", id,
                    currentUserId);
                return _mapper.Map<InsuranceViewModel?>(insurance);
            }
            else
            {
                return null;
            }
        }

        public async Task<SelectList> GetInsuredPersonSelectListAsync(int? selectedId = null)
        {
            _logger.LogInformation("Fetching insured persons list for selection.");

            var insuredPersons = await _insuranceRepository.GetInsuredPersonsAsync(selectedId);

            var list = insuredPersons
                .Select(p => new
                {
                    p.Id,
                    FullName = $"{p.SocialSecurityNumber} {p.LastName} {p.FirstName}"
                })
                .ToList();

            return new SelectList(list, "Id", "FullName", selectedId);
        }

        public async Task<List<InsuranceViewModel>?> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            _logger.LogInformation("Fetching insurances by list of IDs.");
            var insurances = await _insuranceRepository.GetInsurancesByIdsAsync(insuranceIds);
            return _mapper.Map<List<InsuranceViewModel>>(insurances);
        }
    }
}
