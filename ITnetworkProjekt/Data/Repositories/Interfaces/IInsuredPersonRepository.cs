using ITnetworkProjekt.Data.Entities;

namespace ITnetworkProjekt.Data.Repositories.Interfaces
{
    public interface IInsuredPersonRepository : IBaseRepository<InsuredPerson>
    {
        Task<InsuredPerson?> FindByEmailAndSocialSecurityNumberAsync(string email, string socialSecurityNumber);
    }
}
