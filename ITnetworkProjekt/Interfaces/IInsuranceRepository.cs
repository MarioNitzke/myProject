using System.Collections;
using System.Security.Claims;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuranceRepository : IBaseRepository<Insurance>
    {
        Task<List<Insurance>> GetInsurancesByIdsAsync(List<int> insuranceIds);
        Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null);
    }
}


