using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using ITnetworkProjekt.Managers;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsuredPersonsController(InsuredPersonManager insuredPersonManager, InsuranceManager insuranceManager)
        : Controller
    {
        private readonly InsuredPersonManager insuredPersonManager = insuredPersonManager;
        private readonly InsuranceManager insuranceManager = insuranceManager;

        // GET: InsuredPersons/Index with PagedList
        public async Task<IActionResult> Index(int? page)
        {
            if (User.IsInRole(UserRoles.Admin))
            {
                var insuredPersons = await insuredPersonManager.GetAllInsuredPersons();

                var pageNumber = page ?? 1;
                var onePageOfInsuredPersons = insuredPersons.ToPagedList(pageNumber, 4);

                ViewBag.OnePageOfInsuredPersons = onePageOfInsuredPersons;
                return View();
            }
            else
            {
                var insuredPerson = await insuredPersonManager.GetInsuredPersonForUserAsync(User);
                var insurances = await insuranceManager.GetInsurancesByIdsAsync(insuredPerson.InsuranceIds);
                ViewBag.Insurances = insurances;
                return View("Details", insuredPerson);
            }
        }

        // GET: InsuredPerson/Details
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredPerson = await insuredPersonManager.FindInsuredPersonById(id.Value);

            if (insuredPerson == null)
            {
                return NotFound();
            }

            var insurances = await insuranceManager.GetInsurancesByIdsAsync(insuredPerson.InsuranceIds);
            ViewBag.Insurances = insurances;

            return View(insuredPerson);
        }

        // GET: InsuredPerson/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
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
                await insuredPersonManager.AddInsuredPerson(insuredPerson);
                return RedirectToAction(nameof(Index));
            }

            return View(insuredPerson);
        }

        // GET: InsuredPerson/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredPerson = await insuredPersonManager.FindInsuredPersonById((int)id);
            if (insuredPerson == null)
            {
                return NotFound();
            }

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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var updatedInsuredPerson = await insuredPersonManager.UpdateInsuredPerson(insuredPerson);
                return updatedInsuredPerson is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            return View(insuredPerson);
        }

        // GET: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredPerson = await insuredPersonManager.FindInsuredPersonById((int)id);

            if (insuredPerson == null)
            {
                return NotFound();
            }

            return View(insuredPerson);
        }

        // POST: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await insuredPersonManager.RemoveInsuredPersonWithId(id);
            return RedirectToAction(nameof(Index));
        }
    }
}