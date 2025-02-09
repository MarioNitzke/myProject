using ITnetworkProjekt.Models;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuredPersonRepository : IBaseRepository<InsuredPerson>
    {
        Task<InsuredPerson?> FindByEmailAndSocialSecurityNumberAsync(string email, string socialSecurityNumber);
    }
}
