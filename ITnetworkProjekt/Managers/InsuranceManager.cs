using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using ITnetworkProjekt.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Managers
{
    public class InsuranceManager(IInsuranceRepository insuranceRepository,
        IMapper mapper,
        UserManager<IdentityUser> userManager,
        ILogger<InsuranceManager> logger)
    {
        private readonly IInsuranceRepository insuranceRepository = insuranceRepository ?? throw new ArgumentNullException(nameof(insuranceRepository));
        private readonly IMapper mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly UserManager<IdentityUser> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly ILogger<InsuranceManager> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<InsuranceViewModel?> FindInsuranceById(int id)
        {
            logger.LogInformation("Searching for insurance with ID {InsuranceId}.", id);

            Insurance? insurance = await insuranceRepository.FindById(id);
            if (insurance == null)
            {
                logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
            }

            return mapper.Map<InsuranceViewModel?>(insurance);
        }

        public async Task<List<InsuranceViewModel>> GetAllInsurances()
        {
            logger.LogInformation("Fetching all insurances.");
            List<Insurance> insurances = await insuranceRepository.GetAll();
            return mapper.Map<List<InsuranceViewModel>>(insurances);
        }
        
        public async Task<InsuranceViewModel?> AddInsurance(InsuranceViewModel insuranceViewModel)
        {
            logger.LogInformation("Adding new insurance for insured person ID {InsuredPersonId}.", insuranceViewModel.InsuredPersonID);

            Insurance insurance = mapper.Map<Insurance>(insuranceViewModel);
            Insurance addedInsurance = await insuranceRepository.Insert(insurance); 
            
            logger.LogInformation("Insurance successfully added with ID {InsuranceId}.", addedInsurance.Id);
            return mapper.Map<InsuranceViewModel>(addedInsurance);
        }

        public async Task<InsuranceViewModel?> UpdateInsurance(InsuranceViewModel insuranceViewModel)
        {
            logger.LogInformation("Updating insurance with ID {InsuranceId}.", insuranceViewModel.Id);

            Insurance insurance = mapper.Map<Insurance>(insuranceViewModel);

            try
            {
                Insurance updatedInsurance = await insuranceRepository.Update(insurance);
                logger.LogInformation("Insurance with ID {InsuranceId} successfully updated.", updatedInsurance.Id);
                return mapper.Map<InsuranceViewModel>(updatedInsurance);
            }
            catch (InvalidOperationException)
            {
                if (!await insuranceRepository.ExistsWithId(insurance.Id))
                {
                    logger.LogWarning("Attempted to update non-existent insurance with ID {InsuranceId}.", insurance.Id);
                    return null;
                }
                throw;
            }
        }

        public async Task RemoveInsuranceWithId(int id)
        {
            logger.LogInformation("Attempting to remove insurance with ID {InsuranceId}.", id);
            Insurance? insurance = await insuranceRepository.FindById(id);

            if (insurance is not null)
            {
                logger.LogInformation("Insurance with ID {InsuranceId} deleted.", id);
                await insuranceRepository.Delete(insurance);
            }
            else
            {
                logger.LogWarning("Attempted to remove non-existent insurance with ID {InsuranceId}.", id);
            }
        }

        public async Task<InsuranceViewModel?> GetInsurancesForLoggedUserAsync(int id, ClaimsPrincipal user)
        {
            logger.LogInformation("Try to fetch insurance with ID {InsuranceId} for logged user.", id);
            Insurance? insurance;

            if (user.IsInRole(UserRoles.Admin))
            {
                logger.LogInformation("User is admin, fetching insurance with ID {InsuranceId}.", id);
                insurance = await insuranceRepository.FindById(id);
                return mapper.Map<InsuranceViewModel?>(insurance);
            }

            int? currentUserId = await insuranceRepository.GetInsuredPersonIdOfCurrentUserAsync(userManager.GetUserId(user));
            if (currentUserId is null)
            {
                logger.LogWarning("Current user does not have a valid insured person ID. Insurance do not show.");
                return null;
            }

            insurance = await insuranceRepository.FindById(id);
            if (insurance is not null && insurance.InsuredPersonID == currentUserId)
            {
                logger.LogInformation("Insurance with ID {InsuranceId} found for current user with userID {CurrentUserId}.", id, currentUserId);
                return mapper.Map<InsuranceViewModel?>(insurance);
            }else
            {
                return null;
            }
        }

        public async Task<SelectList> GetInsuredPersonSelectListAsync(int? selectedId = null)
        {
            logger.LogInformation("Fetching insured persons list for selection.");

            List<InsuredPerson> insuredPersons = await insuranceRepository.GetInsuredPersonsAsync(selectedId);

            var list = insuredPersons
                .Select(p => new {
                    Id = p.Id,
                    FullName = $"{p.SocialSecurityNumber} {p.LastName} {p.FirstName}"
                })
                .ToList();

            return new SelectList(list, "Id", "FullName", selectedId);
        }

        public async Task<List<InsuranceViewModel>> GetInsurancesByIdsAsync(List<int> insuranceIds)
        {
            logger.LogInformation("Fetching insurances by list of IDs.");
            List<Insurance>? insurances = await insuranceRepository.GetInsurancesByIdsAsync(insuranceIds);
            return mapper.Map<List<InsuranceViewModel>>(insurances);
        }

    }
}
