using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using ITnetworkProjekt.Services;
using X.PagedList.Extensions;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsuredPersonsController(InsuredPersonService insuredPersonService) : Controller
    {
        private readonly InsuredPersonService _insuredPersonService = insuredPersonService;


        // GET: InsuredPersons
        public async Task<IActionResult> Index(int? page)
        {
            if (User.IsInRole(UserRoles.Admin))
            {
                var insuredPersons = await _insuredPersonService.GetAllInsuredPersonsAsync();

                var pageNumber = page ?? 1;
                var onePageOfInsuredPersons = insuredPersons.ToPagedList(pageNumber, 4);

                ViewBag.OnePageOfInsuredPersons = onePageOfInsuredPersons;
                return View();
            }
            else
            {
                var insuredPerson = await _insuredPersonService.GetInsuredPersonForUserAsync(User);
                return View("Details", insuredPerson);
            }
        }

        // GET: InsuredPersons/Details
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insurance = await _insuredPersonService.GetInsuredPersonByIdAsync(id.Value);

            if (insurance == null)
            {
                return NotFound();
            }

            return View(insurance);
        }


        // GET: InsuredPersons/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: InsuredPersons/Create
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")] InsuredPerson insuredPerson)
        {
            if (ModelState.IsValid)
            {
                await _insuredPersonService.CreateInsuredPersonAsync(insuredPerson);
                return RedirectToAction(nameof(Index));
            }

            return View(insuredPerson);
        }

        // GET: InsuredPersons/Edit
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredPerson = await _insuredPersonService.GetInsuredPersonByIdAsync(id.Value);
            if (insuredPerson == null)
            {
                return NotFound();
            }
            return View(insuredPerson);
        }

        // POST: InsuredPersons/Edit
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")] InsuredPerson insuredPerson)
        {
            if (id != insuredPerson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _insuredPersonService.UpdateInsuredPersonAsync(insuredPerson);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _insuredPersonService.InsuredPersonExistsAsync(insuredPerson.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(insuredPerson);
        }

        // GET: InsuredPersons/Delete
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuredPerson = await _insuredPersonService.GetInsuredPersonByIdAsync(id.Value);

            if (insuredPerson == null)
            {
                return NotFound();
            }

            return View(insuredPerson);
        }

        // POST: InsuredPersons/Delete
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _insuredPersonService.DeleteInsuredPersonByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> InsuredPersonExistsAsync(int id)
        {
            return await _insuredPersonService.InsuredPersonExistsAsync(id);
        }

    }
}
