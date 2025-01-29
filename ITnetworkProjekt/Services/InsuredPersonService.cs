using ITnetworkProjekt.Data;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITnetworkProjekt.Services
{
    public class InsuredPersonService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public InsuredPersonService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }


        // Načtení všech pojištěnců
        public async Task<List<InsuredPerson>> GetAllInsuredPersonsAsync()
        {
            return await _context.InsuredPerson.ToListAsync();
        }

        //načtení profilu pro uživatele(klienta pojištovny)
        public async Task<InsuredPerson?> GetInsuredPersonForUserAsync(ClaimsPrincipal user)
        {
            var currentUserId = await _context.InsuredPerson
                .Where(m => m.UserId == _userManager.GetUserId(user))
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            if (currentUserId == 0)
            {
                return null;
            }

            return await _context.InsuredPerson
                .Include(m => m.Insurances)
                .FirstOrDefaultAsync(m => m.Id == currentUserId);
        }


        // Načtení pojištěnce podle ID
        public async Task<InsuredPerson?> GetInsuredPersonByIdAsync(int id)
        {
            return await _context.InsuredPerson.Include(i => i.Insurances).FirstOrDefaultAsync(m => m.Id == id);
        }


        // Vytvoření nového pojištěnce
        public async Task CreateInsuredPersonAsync(InsuredPerson insuredPerson)
        {
            _context.InsuredPerson.Add(insuredPerson);
            await _context.SaveChangesAsync();
        }

        // Aktualizace pojištěnce
        public async Task UpdateInsuredPersonAsync(InsuredPerson insuredPerson)
        {
            _context.InsuredPerson.Update(insuredPerson);
            await _context.SaveChangesAsync();
        }

        // Smazání pojištěnce podle ID
        public async Task DeleteInsuredPersonByIdAsync(int id)
        {
            var insuredPerson = await GetInsuredPersonByIdAsync(id);
            if (insuredPerson != null)
            {
                _context.InsuredPerson.Remove(insuredPerson);
                await _context.SaveChangesAsync();
            }
        }

        // Kontrola existence pojištěnce podle ID
        public async Task<bool> InsuredPersonExistsAsync(int id)
        {
            return await _context.InsuredPerson.AnyAsync(p => p.Id == id);
        }

    }
}
