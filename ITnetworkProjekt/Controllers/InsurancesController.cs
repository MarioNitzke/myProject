using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using ITnetworkProjekt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsurancesController(InsuranceManager insuranceManager, InsuredPersonManager insuredPersonManager) : Controller
    {
        private readonly InsuranceManager insuranceManager = insuranceManager;
        private readonly InsuredPersonManager insuredPersonManager = insuredPersonManager;

        // GET: Insurances/Index
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Index()
        {
            return View(await insuranceManager.GetAllInsurances());
        }

        // GET: Insurances/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await insuranceManager.GetInsurancesForLoggedUserAsync((int)id, User);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // GET: Insurances/Create + with specific insuredPersonId
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(int? insuredPersonId = null)
        {
            var insurance = new InsuranceViewModel();
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync();

            if (insuredPersonId.HasValue)
            {
                var insuredPerson = await insuredPersonManager.FindInsuredPersonById(insuredPersonId.Value);
                if (insuredPerson != null)
                {
                    ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId.Value);
                    ViewBag.PersonName = await insuredPersonManager.GetPersonNameById(insuredPersonId.Value);
                    insurance.InsuredPersonID = insuredPersonId.Value;
                }
            }

            return View(insurance);
        }

        // POST: Insurances/Create
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? insuredPersonId,
            [Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")] InsuranceViewModel insurance)
        {
            if (ModelState.IsValid)
            {
                await insuranceManager.AddInsurance(insurance);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId);
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

            var insurance = await insuranceManager.FindInsuranceById((int)id);
            if (insurance == null)
            {
                return NotFound();
            }
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync(id);
            return View(insurance);
        }

        // POST: Insurances/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")] InsuranceViewModel insurance)
        {
            if (id != insurance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedInsurance = await insuranceManager.UpdateInsurance(insurance);
                return updatedInsurance is null ? NotFound() : RedirectToAction(nameof(Index));
            }
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync(id);
            return View(insurance);
        }

        // GET: Insurances/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await insuranceManager.FindInsuranceById((int)id);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }

        // POST: Insurances/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await insuranceManager.RemoveInsuranceWithId(id);
            return RedirectToAction(nameof(Index));
        }
    }
}


