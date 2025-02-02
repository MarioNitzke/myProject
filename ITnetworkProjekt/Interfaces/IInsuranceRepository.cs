using System.Collections;
using System.Security.Claims;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Interfaces
{
    public interface IInsuranceRepository : IBaseRepository<Insurance>
    {
        Task<int> GetInsuredPersonIdOfCurrentUserAsync(string userId);
        Task<List<InsuredPerson>> GetInsuredPersonsAsync(int? selectedId = null);
    }
}


