using ITnetworkProjekt.Models;
using ITnetworkProjekt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsurancesController(
        InsuranceService insuranceService,
        InsuredPersonService insuredPersonService
        ) : Controller
    {
        private readonly InsuranceService _insuranceService = insuranceService;
        private readonly InsuredPersonService _insuredPersonService = insuredPersonService;


        // GET: Insurances/Index
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Index()
        {
            var insurances = await _insuranceService.GetAllInsurancesAsync();
            return View(insurances);
        }

        // GET: Insurances/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _insuranceService.GetInsuranceForUserAsync(id.Value, User);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // GET: Insurances/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
            ViewData["InsuredPersonID"] = _insuranceService.GetInsuredPersonSelectListAsync();
            return View();
        }

        // POST: Insurances/Create with insuredPersonId - konkrétnímu uživateli
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(int? insuredPersonId = null)
        {
            var insuredPersonSelectList = await _insuranceService.GetInsuredPersonSelectListAsync(insuredPersonId);
            ViewData["InsuredPersonID"] = insuredPersonSelectList;

            var insurance = new Insurance();
            if (insuredPersonId.HasValue)
            {
                var insuredPerson = await _insuredPersonService.GetInsuredPersonByIdAsync(insuredPersonId.Value);
                if (insuredPerson != null)
                {
                    ViewBag.PersonName = $"{insuredPerson.FirstName} {insuredPerson.LastName}";
                    insurance.InsuredPersonID = insuredPersonId.Value;
                }
            }

            return View(insurance);
        }

        // POST: Insurances/Create
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")] Insurance insurance)
        {
            if (ModelState.IsValid)
            {
                await _insuranceService.CreateInsuranceAsync(insurance);
                return RedirectToAction(nameof(Index));
            }

            ViewData["InsuredPersonID"] = await _insuranceService.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // GET: Insurances/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _insuranceService.GetInsuranceByIdAsync(id.Value);
            if (insurance == null)
            {
                return NotFound();
            }

            ViewData["InsuredPersonID"] = await _insuranceService.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // POST: Insurances/Edit
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")] Insurance insurance)
        {
            if (id != insurance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _insuranceService.UpdateInsuranceAsync(insurance);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _insuranceService.InsuranceExistsAsync(insurance.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewData["InsuredPersonID"] = await _insuranceService.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // GET: Insurances/Delete
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _insuranceService.GetInsuranceByIdAsync(id.Value);
            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // POST: Insurances/Delete
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _insuranceService.DeleteInsuranceAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> InsuranceExistsAsync(int id)
        {
            return await _insuranceService.InsuranceExistsAsync(id);
        }
    }
}


