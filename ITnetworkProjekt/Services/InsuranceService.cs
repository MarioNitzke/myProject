using ITnetworkProjekt.Data;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITnetworkProjekt.Services
{
    public class InsuranceService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InsuranceService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // Načtení seznamu pojištěnců pro dropdown
        public async Task<SelectList> GetInsuredPersonSelectListAsync(int? selectedId = null)
        {
            var query = _context.InsuredPerson
                .Select(p => new
                {
                    Id = p.Id,
                    FullName = $"{p.SocialSecurityNumber} {p.LastName} {p.FirstName}"
                });

            if (selectedId.HasValue)
            {
                query = query.Where(x => x.Id == selectedId.Value);
            }

            var list = await query.ToListAsync();
            return new SelectList(list, "Id", "FullName", selectedId);
        }

        // Ověření existence pojistky
        public async Task<bool> InsuranceExistsAsync(int id)
        {
            return await _context.Insurance.AnyAsync(e => e.Id == id);
        }

        // Operace CRUD
        public async Task<List<Insurance>> GetAllInsurancesAsync()
        {
            return await _context.Insurance.Include(i => i.InsuredPerson).ToListAsync();
        }

        public async Task<Insurance?> GetInsuranceByIdAsync(int id)
        {
            return await _context.Insurance.Include(i => i.InsuredPerson).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateInsuranceAsync(Insurance insurance)
        {
            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInsuranceAsync(Insurance insurance)
        {
            _context.Insurance.Update(insurance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInsuranceAsync(int id)
        {
            var insurance = await GetInsuranceByIdAsync(id);
            if (insurance != null)
            {
                _context.Insurance.Remove(insurance);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<Insurance?> GetInsuranceForUserAsync(int id, ClaimsPrincipal user)
        {
            if (user.IsInRole(UserRoles.Admin))
            {
                // Administrátor může zobrazit libovolnou pojistku
                return await _context.Insurance
                    .Include(i => i.InsuredPerson)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }

            // Běžný uživatel může zobrazit pouze své pojistky
            var currentUserId = await _context.InsuredPerson
                .Where(m => m.UserId == _userManager.GetUserId(user))
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            return await _context.Insurance
                .Include(i => i.InsuredPerson)
                .FirstOrDefaultAsync(m => m.Id == id && m.InsuredPersonID == currentUserId);
        }

    }
}


