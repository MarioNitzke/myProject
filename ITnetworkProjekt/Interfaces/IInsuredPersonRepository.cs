using ITnetworkProjekt.Models;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuredPersonRepository : IBaseRepository<InsuredPerson>
    {
        Task<InsuredPerson?> FindByEmailAndSSNAsync(string email, string ssn);
    }
}
