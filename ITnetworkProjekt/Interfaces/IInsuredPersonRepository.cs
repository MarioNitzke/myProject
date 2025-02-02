using ITnetworkProjekt.Models;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuredPersonRepository : IBaseRepository<InsuredPerson>
    {
        Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId);
    }
}
