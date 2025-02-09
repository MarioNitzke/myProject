using ITnetworkProjekt.Models;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuranceRepository : IBaseRepository<Insurance>
    {
        Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds);
        Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null);
    }
}


