using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsurancesController(
        InsuranceManager insuranceManager,
        InsuredPersonManager insuredPersonManager,
        ILogger<InsurancesController> logger) : Controller
    {
        private readonly InsuranceManager _insuranceManager = insuranceManager;
        private readonly InsuredPersonManager _insuredPersonManager = insuredPersonManager;
        private readonly ILogger<InsurancesController> _logger = logger;

        // GET: Insurances/Index
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Index(int? page)
        {
            _logger.LogInformation("User accessed the insurance list (page {Page}).", page ?? 1);

            var insurances = await _insuranceManager.GetAllInsurances();
            var onePageOfInsurances = insurances.ToPagedList(page ?? 1, 4);
            ViewBag.OnePageOfInsurances = onePageOfInsurances;

            return View(insurances);
        }

        // GET: Insurances/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Insurance details requested with null ID.");
                return NotFound();
            }

            var insurance = await _insuranceManager.GetInsurancesForLoggedUserAsync((int)id, User);
            if (insurance == null)
            {
                _logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("User viewed insurance details wtih ID: {InsuranceId}.", id);
            return View(insurance);
        }

        // GET: Insurances/Create + with specific insuredPersonId
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(int? insuredPersonId = null)
        {
            _logger.LogInformation("User accessed the insurance creation page.");

            var insurance = new InsuranceViewModel();
            ViewBag.InsuredPersonId = await _insuranceManager.GetInsuredPersonSelectListAsync();

            if (insuredPersonId.HasValue)
            {
                var insuredPerson = await _insuredPersonManager.FindInsuredPersonById(insuredPersonId.Value);
                if (insuredPerson != null)
                {
                    _logger.LogInformation("Insurance creation page opened with preselected insured person with ID: {InsuredPersonId}.", insuredPersonId);
                    ViewBag.InsuredPersonId =
                        await _insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId.Value);
                    ViewBag.PersonName = await _insuredPersonManager.GetPersonNameById(insuredPersonId.Value) ?? string.Empty;
                    insurance.InsuredPersonId = insuredPersonId.Value;
                }
            }

            return View(insurance);
        }

        // POST: Insurances/Create
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? insuredPersonId,
            [Bind("Id,InsuredPersonId,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")]
            InsuranceViewModel insurance)
        {
            if (ModelState.IsValid)
            {
                await _insuranceManager.AddInsurance(insurance);
                _logger.LogInformation("Insurance created for insured person with ID: {InsuredPersonId}.", insuredPersonId);
                return RedirectToAction(nameof(Index));
            }
            
            _logger.LogWarning("Invalid model state when creating insurance for insured person with ID: {InsuredPersonId}.", insuredPersonId);
            ViewBag.InsuredPersonId = await _insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId);
            return View(insurance);
        }

        // GET: Insurances/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Insurance edit requested with null ID.");
                return NotFound();
            }

            var insurance = await _insuranceManager.FindInsuranceById((int)id);
            if (insurance == null)
            {
                _logger.LogWarning("Insurance with ID {InsuranceId} not found for editing.", id);
                return NotFound();
            }
            _logger.LogInformation("User accessed the edit page for insurance ID {InsuranceId}.", id);
            ViewBag.InsuredPersonId = await _insuranceManager.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // POST: Insurances/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,InsuredPersonId,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")]
            InsuranceViewModel insurance)
        {
            if (id != insurance.Id)
            {
                _logger.LogWarning("Edit request with mismatched insurance ID with {ProvidedId} != {ModelId}.", id, insurance.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedInsurance = await _insuranceManager.UpdateInsurance(insurance);
                _logger.LogInformation("Insurance with ID {InsuranceId} successfully updated.", id);
                return updatedInsurance is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Insurance edit failed due to invalid model state.");
            ViewBag.InsuredPersonId = await _insuranceManager.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // GET: Insurances/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Insurance delete requested with null ID.");
                return NotFound();
            }

            var insurance = await _insuranceManager.FindInsuranceById((int)id);

            if (insurance == null)
            {
                _logger.LogWarning("Insurance with ID {InsuranceId} not found for deletion.", id);
                return NotFound();
            }

            _logger.LogInformation("User accessed the delete page for insurance ID {InsuranceId}.", id);
            return View(insurance);
        }

        // POST: Insurances/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _insuranceManager.RemoveInsuranceWithId(id);
            _logger.LogInformation("Insurance with ID {InsuranceId} successfully deleted.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
