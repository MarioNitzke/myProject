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
    public class InsuranceManager(IInsuranceRepository insuranceRepository, IMapper mapper, UserManager<IdentityUser> userManager)
    {
        private readonly IInsuranceRepository insuranceRepository = insuranceRepository;
        private readonly IMapper mapper = mapper;

        public async Task<InsuranceViewModel?> FindInsuranceById(int id)
        {
            Insurance? insurance = await insuranceRepository.FindById(id);
            return mapper.Map<InsuranceViewModel?>(insurance);
        }

        public async Task<List<InsuranceViewModel>> GetAllInsurances()
        {
            List<Insurance> insurances = await insuranceRepository.GetAll();
            return mapper.Map<List<InsuranceViewModel>>(insurances);
        }

        public async Task<InsuranceViewModel?> AddInsurance(InsuranceViewModel insuranceViewModel)
        {
            Insurance insurance = mapper.Map<Insurance>(insuranceViewModel);
            Insurance addedInsurance = await insuranceRepository.Insert(insurance);
            return mapper.Map<InsuranceViewModel>(addedInsurance);
        }

        public async Task<InsuranceViewModel?> UpdateInsurance(InsuranceViewModel insuranceViewModel)
        {
            Insurance insurance = mapper.Map<Insurance>(insuranceViewModel);

            try
            {
                Insurance updatedInsurance = await insuranceRepository.Update(insurance);
                return mapper.Map<InsuranceViewModel>(updatedInsurance);
            }
            catch (InvalidOperationException)
            {
                if (!await insuranceRepository.ExistsWithId(insurance.Id))
                    return null;

                throw;
            }
        }

        public async Task RemoveInsuranceWithId(int id)
        {
            Insurance? insurance = await insuranceRepository.FindById(id);

            if (insurance is not null)
                await insuranceRepository.Delete(insurance);
        }

        public async Task<InsuranceViewModel?> GetInsurancesForLoggedUserAsync(int id, ClaimsPrincipal user)
        {
            Insurance? insurance;

            if (user.IsInRole(UserRoles.Admin))
            {
                insurance = await insuranceRepository.FindById(id);
                return mapper.Map<InsuranceViewModel?>(insurance);
            }

            int? currentUserId = await insuranceRepository.GetInsuredPersonIdOfCurrentUserAsync(userManager.GetUserId(user));
            if (currentUserId is null) return null;

            insurance = await insuranceRepository.FindById(id);
            if (insurance is not null && insurance.InsuredPersonID == currentUserId)
            {
                return mapper.Map<InsuranceViewModel?>(insurance);
            }else
            {
                return null;
            }
        }

        public async Task<SelectList> GetInsuredPersonSelectListAsync(int? selectedId = null)
        {
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
            List<Insurance>? insurances = await insuranceRepository.GetInsurancesByIdsAsync(insuranceIds);
            return mapper.Map<List<InsuranceViewModel>>(insurances);
        }

    }
}
