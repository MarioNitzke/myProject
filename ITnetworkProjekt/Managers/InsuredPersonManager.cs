using System.Security.Claims;
using AutoMapper;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace ITnetworkProjekt.Managers
{
    public class InsuredPersonManager(IInsuredPersonRepository insuredPersonRepository, IMapper mapper, UserManager<IdentityUser> userManager)
    {
        private readonly IInsuredPersonRepository insuredPersonRepository = insuredPersonRepository;
        private readonly IMapper mapper = mapper;

        public async Task<InsuredPersonViewModel?> FindInsuredPersonById(int id)
        {

            InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(id);
            return mapper.Map<InsuredPersonViewModel?>(insuredPerson);
        }

        public async Task<List<InsuredPersonViewModel>> GetAllInsuredPersons()
        {
            List<InsuredPerson> insuredPersons = await insuredPersonRepository.GetAll();
            return mapper.Map<List<InsuredPersonViewModel>>(insuredPersons);
        }

        public async Task<InsuredPersonViewModel?> AddInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            InsuredPerson insuredPerson = mapper.Map<InsuredPerson>(insuredPersonViewModel);
            InsuredPerson addedInsuredPerson = await insuredPersonRepository.Insert(insuredPerson);
            return mapper.Map<InsuredPersonViewModel>(addedInsuredPerson);
        }

        public async Task<InsuredPersonViewModel?> UpdateInsuredPerson(InsuredPersonViewModel insuredPersonViewModel)
        {
            InsuredPerson insuredPerson = mapper.Map<InsuredPerson>(insuredPersonViewModel);

            try
            {
                InsuredPerson updatedInsuredPerson = await insuredPersonRepository.Update(insuredPerson);
                return mapper.Map<InsuredPersonViewModel>(updatedInsuredPerson);
            }
            catch (InvalidOperationException)
            {
                if (!await insuredPersonRepository.ExistsWithId(insuredPerson.Id))
                    return null;

                throw;
            }
        }

        public async Task RemoveInsuredPersonWithId(int id)
        {
            InsuredPerson? insuredPerson = await insuredPersonRepository.FindById(id);

            if (insuredPerson is not null)
                await insuredPersonRepository.Delete(insuredPerson);
        }

        public async Task<string?> GetPersonNameById(int id)
        {
            var insuredPerson = await FindInsuredPersonById(id);
            if (insuredPerson != null)
            {
                return $"{insuredPerson.FirstName} {insuredPerson.LastName}";
            }
            return null;
        }

        public async Task<InsuredPersonViewModel?> GetInsuredPersonForUserAsync(ClaimsPrincipal user)
        {
            var currentUserId = await insuredPersonRepository.GetInsuredPersonIdOfCurrentUserAsync(userManager.GetUserId(user));
            if (currentUserId != null)
            {
                var insuredPerson = await insuredPersonRepository.FindById(currentUserId);
                return mapper.Map<InsuredPersonViewModel?>(insuredPerson);
            }
            return null;
        }
    }
}
