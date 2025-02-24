using ITnetworkProjekt.Data.Entities;

namespace ITnetworkProjekt.Data.Repositories.Interfaces
{
    public interface IInsuranceRepository : IBaseRepository<Insurance>
    {
        Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds);
        Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null);
    }
}


