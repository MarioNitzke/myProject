using Microsoft.AspNetCore.Mvc;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using ITnetworkProjekt.Managers;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsuredPersonsController(
        InsuredPersonManager insuredPersonManager,
        InsuranceManager insuranceManager,
        ILogger<InsuredPersonsController> logger) : Controller
    {
        private readonly InsuredPersonManager _insuredPersonManager = insuredPersonManager;
        private readonly InsuranceManager _insuranceManager = insuranceManager;
        private readonly ILogger<InsuredPersonsController> _logger = logger;

        // GET: InsuredPersons/Index with PagedList
        public async Task<IActionResult> Index(int? page)
        {
            if (User.IsInRole(UserRoles.Admin))
            {
                var insuredPersons = await _insuredPersonManager.GetAllInsuredPersons();

                var pageNumber = page ?? 1;
                var onePageOfInsuredPersons = insuredPersons.ToPagedList(pageNumber, 4);
                ViewBag.OnePageOfInsuredPersons = onePageOfInsuredPersons;

                _logger.LogInformation("Admin displaying page {PageNumber} of insured persons.", pageNumber);
                return View();
            }
            else
            {
                var insuredPerson = await _insuredPersonManager.GetInsuredPersonForUserAsync(User);
                if (insuredPerson?.InsuranceIds != null)
                {
                    _logger.LogInformation("User has insurances.");
                    var insurances = await _insuranceManager.GetInsurancesByIdsAsync(insuredPerson.InsuranceIds);
                    ViewBag.Insurances = insurances;
                }
                else
                {
                    _logger.LogWarning("User has no insurances.");
                    ViewBag.Insurances = null;
                }

                _logger.LogInformation("Non-admin user accessed their own insured person details (ID: {InsuredPersonId}).", insuredPerson?.Id);
                return View("Details", insuredPerson);
            }
        }

        // GET: InsuredPerson/Details
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details requested with null ID.");
                return NotFound();
            }

            var insuredPerson = await _insuredPersonManager.FindInsuredPersonById(id.Value);

            if (insuredPerson == null)
            {
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return NotFound();
            }

            var insurances = await _insuranceManager.GetInsurancesByIdsAsync(insuredPerson.InsuranceIds);
            ViewBag.Insurances = insurances;

            _logger.LogInformation("Admin displaying details of insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
            _logger.LogInformation("Admin accessed insured person creating page.");
            return View();
        }

        // POST: InsuredPerson/Create
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")]
            InsuredPersonViewModel insuredPerson)
        {
            if (ModelState.IsValid)
            {
                await _insuredPersonManager.AddInsuredPerson(insuredPerson);
                _logger.LogInformation("New insured person created with ID: {InsuredPersonId}.", insuredPerson.Id);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Insured person creation failed due to invalid model state.");
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogInformation("Edit requested received with null ID.");
                return NotFound();
            }

            var insuredPerson = await _insuredPersonManager.FindInsuredPersonById((int)id);
            if (insuredPerson == null)
            {
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Admin accessed edit page for insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // POST: InsuredPerson/Edit
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")]
            InsuredPersonViewModel insuredPerson)
        {
            if (id != insuredPerson.Id)
            {
                _logger.LogWarning("Mismatch. Edit requested for ID {InsuredPersonId} but model ID is {ModelId}.", id, insuredPerson.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedInsuredPerson = await _insuredPersonManager.UpdateInsuredPerson(insuredPerson);
                _logger.LogInformation("Insured person with ID {InsuredPersonId} updated.", id);
                return updatedInsuredPerson is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Insured person with ID {InsuredPersonId} update failed due to invalid model state.", id);
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete requested received with null ID.");
                return NotFound();
            }

            var insuredPerson = await _insuredPersonManager.FindInsuredPersonById((int)id);

            if (insuredPerson == null)
            {
                _logger.LogWarning("Insured person with ID {InsuredPersonId} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Admin accessed delete page for insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // POST: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _insuredPersonManager.RemoveInsuredPersonWithId(id);
            _logger.LogInformation("Insured person with ID {InsuredPersonId} deleted.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
