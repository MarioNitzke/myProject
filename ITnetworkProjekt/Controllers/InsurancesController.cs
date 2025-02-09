using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsurancesController(
        InsuranceManager insuranceManager,
        InsuredPersonManager insuredPersonManager,
        ILogger<InsurancesController> logger) : Controller
    {
        private readonly InsuranceManager insuranceManager = insuranceManager ?? throw new ArgumentNullException(nameof(insuranceManager));
        private readonly InsuredPersonManager insuredPersonManager = insuredPersonManager ?? throw new ArgumentNullException(nameof(insuredPersonManager));
        private readonly ILogger<InsurancesController> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // GET: Insurances/Index
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Index(int? page)
        {
            logger.LogInformation("User accessed the insurance list (page {Page}).", page ?? 1);

            var insurances = await insuranceManager.GetAllInsurances();
            var onePageOfInsurances = insurances.ToPagedList(page ?? 1, 4);
            ViewBag.OnePageOfInsurances = onePageOfInsurances;

            return View(insurances);
        }

        // GET: Insurances/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Insurance details requested with null ID.");
                return NotFound();
            }

            var insurance = await insuranceManager.GetInsurancesForLoggedUserAsync((int)id, User);
            if (insurance == null)
            {
                logger.LogWarning("Insurance with ID {InsuranceId} not found.", id);
                return NotFound();
            }

            logger.LogInformation("User viewed insurance details wtih ID: {InsuranceId}.", id);
            return View(insurance);
        }

        // GET: Insurances/Create + with specific insuredPersonId
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(int? insuredPersonId = null)
        {
            logger.LogInformation("User accessed the insurance creation page.");

            var insurance = new InsuranceViewModel();
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync();

            if (insuredPersonId.HasValue)
            {
                var insuredPerson = await insuredPersonManager.FindInsuredPersonById(insuredPersonId.Value);
                if (insuredPerson != null)
                {
                    logger.LogInformation("Insurance creation page opened with preselected insured person with ID: {InsuredPersonId}.", insuredPersonId);
                    ViewBag.InsuredPersonID =
                        await insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId.Value);
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
            [Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")]
            InsuranceViewModel insurance)
        {
            if (ModelState.IsValid)
            {
                await insuranceManager.AddInsurance(insurance);
                logger.LogInformation("Insurance created for insured person with ID: {InsuredPersonId}.", insuredPersonId);
                return RedirectToAction(nameof(Index));
            }
            
            logger.LogWarning("Invalid model state when creating insurance for insured person with ID: {InsuredPersonId}.", insuredPersonId);
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync(insuredPersonId);
            return View(insurance);
        }

        // GET: Insurances/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Insurance edit requested with null ID.");
                return NotFound();
            }

            var insurance = await insuranceManager.FindInsuranceById((int)id);
            if (insurance == null)
            {
                logger.LogWarning("Insurance with ID {InsuranceId} not found for editing.", id);
                return NotFound();
            }
            logger.LogInformation("User accessed the edit page for insurance ID {InsuranceId}.", id);
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // POST: Insurances/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,InsuredPersonID,PolicyType,StartDate,EndDate,PremiumAmount,CreatedDate")]
            InsuranceViewModel insurance)
        {
            if (id != insurance.Id)
            {

                logger.LogWarning("Edit request with mismatched insurance ID with {ProvidedId} != {ModelId}.", id, insurance.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedInsurance = await insuranceManager.UpdateInsurance(insurance);
                logger.LogInformation("Insurance with ID {InsuranceId} successfully updated.", id);
                return updatedInsurance is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            logger.LogWarning("Insurance edit failed due to invalid model state.");
            ViewBag.InsuredPersonID = await insuranceManager.GetInsuredPersonSelectListAsync();
            return View(insurance);
        }

        // GET: Insurances/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Insurance delete requested with null ID.");
                return NotFound();
            }

            var insurance = await insuranceManager.FindInsuranceById((int)id);

            if (insurance == null)
            {
                logger.LogWarning("Insurance with ID {InsuranceId} not found for deletion.", id);
                return NotFound();
            }

            logger.LogInformation("User accessed the delete page for insurance ID {InsuranceId}.", id);
            return View(insurance);
        }

        // POST: Insurances/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await insuranceManager.RemoveInsuranceWithId(id);
            logger.LogInformation("Insurance with ID {InsuranceId} successfully deleted.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}