using Microsoft.AspNetCore.Mvc;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList.Extensions;
using ITnetworkProjekt.Features.Common.Queries;
using MediatR;
using ITnetworkProjekt.Features.Common.Commands;
using ITnetworkProjekt.Features.Insurances.Queries;
using ITnetworkProjekt.Features.InsuredPersons.Queries;
using System.Security.Claims;
using ITnetworkProjekt.Data.Entities;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsuredPersonsController(
        ILogger<InsuredPersonsController> logger,
        IMediator mediator) : Controller
    {
        // GET: InsuredPersons/Index with PagedList
        public async Task<IActionResult> Index(int? page)
        {
            if (User.IsInRole(UserRoles.Admin))
            {
                var query = new GetAllEntitiesQuery<InsuredPerson, InsuredPersonViewModel>();

                var insuredPersons = await mediator.Send(query);


                var pageNumber = page ?? 1;
                var onePageOfInsuredPersons = insuredPersons.ToPagedList(pageNumber, 4);
                ViewBag.OnePageOfInsuredPersons = onePageOfInsuredPersons;

                logger.LogInformation("Admin displaying page {PageNumber} of insured persons.", pageNumber);
                return View();
            }
            else
            {
                var insuredPerson =
                    await mediator.Send(
                        new GetInsuredPersonForLoggedUserQuery(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
                if (insuredPerson?.InsuranceIds != null)
                {
                    logger.LogInformation("User has insurances.");
                    var insurances = await mediator.Send(new GetInsurancesByIdsQuery(insuredPerson.InsuranceIds));
                    if (insurances != null) ViewBag.Insurances = insurances;
                }
                else
                {
                    logger.LogWarning("User has no insurances.");
                    ViewBag.Insurances = null!;
                }

                logger.LogInformation(
                    "Non-admin user accessed their own insured person details (ID: {InsuredPersonId}).",
                    insuredPerson?.Id);
                return View("Details", insuredPerson);
            }
        }

        // GET: InsuredPerson/Details
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Details requested with null ID.");
                return NotFound();
            }


            var query = new FindEntityByIdQuery<InsuredPerson, InsuredPersonViewModel> { Id = id.Value };

            var insuredPerson = await mediator.Send(query);

            if (insuredPerson.InsuranceIds != null)
            {
                var insurances = await mediator.Send(new GetInsurancesByIdsQuery(insuredPerson.InsuranceIds));
                if (insurances != null) ViewBag.Insurances = insurances;
            }

            logger.LogInformation("Admin displaying details of insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Create
        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Create()
        {
            logger.LogInformation("Admin accessed insured person creating page.");
            return View();
        }

        // POST: InsuredPerson/Create   GENERIC DONE
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")]
            InsuredPersonViewModel insuredPerson)
        {
            if (ModelState.IsValid)
            {
                var command = new AddEntityCommand<InsuredPerson, InsuredPersonViewModel>(insuredPerson);
                await mediator.Send(command);

                //await insuredPersonManager.AddInsuredPerson(insuredPerson);
                logger.LogInformation("New insured person created with ID: {InsuredPersonId}.", insuredPerson.Id);
                return RedirectToAction(nameof(Index));
            }

            logger.LogInformation("Insured person creation failed due to invalid model state.");
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Edit   GENERIC DONE
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                logger.LogInformation("Edit requested received with null ID.");
                return NotFound();
            }


            var query = new FindEntityByIdQuery<InsuredPerson, InsuredPersonViewModel> { Id = id.Value };

            var insuredPerson = await mediator.Send(query);

            logger.LogInformation("Admin accessed edit page for insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // POST: InsuredPerson/Edit   GENERIC DONE
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,FirstName,LastName,DateOfBirth,PhoneNumber,Email,Address,CreatedDate,SocialSecurityNumber")]
            InsuredPersonViewModel insuredPerson)
        {
            if (id != insuredPerson.Id)
            {
                logger.LogWarning("Mismatch. Edit requested for ID {InsuredPersonId} but model ID is {ModelId}.", id,
                    insuredPerson.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var command = new UpdateEntityCommand<InsuredPerson, InsuredPersonViewModel>(insuredPerson);
                var updatedInsuredPerson = await mediator.Send(command);


                //var updatedInsuredPerson = await insuredPersonManager.UpdateInsuredPerson(insuredPerson);
                logger.LogInformation("Insured person with ID {InsuredPersonId} updated.", id);
                return updatedInsuredPerson is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            logger.LogInformation("Insured person with ID {InsuredPersonId} update failed due to invalid model state.",
                id);
            return View(insuredPerson);
        }

        // GET: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                logger.LogWarning("Delete requested received with null ID.");
                return NotFound();
            }

            var query = new FindEntityByIdQuery<InsuredPerson, InsuredPersonViewModel> { Id = id.Value };
            var insuredPerson = await mediator.Send(query);

            logger.LogInformation("Admin accessed delete page for insured person with ID {InsuredPersonId}.", id);
            return View(insuredPerson);
        }

        // POST: InsuredPerson/Delete
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = new RemoveEntityWithIdCommand<InsuredPerson, InsuredPersonViewModel> { Id = id };
            await mediator.Send(query);

            logger.LogInformation("Insured person with ID {InsuredPersonId} deleted.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}