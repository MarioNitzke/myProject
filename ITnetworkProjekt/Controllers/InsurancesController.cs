using ITnetworkProjekt.Data.Entities;
using ITnetworkProjekt.Features.Common.Commands;
using ITnetworkProjekt.Features.Common.Queries;
using ITnetworkProjekt.Features.Insurances.Queries;
using ITnetworkProjekt.Features.InsuredPersons.Queries;
using ITnetworkProjekt.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace ITnetworkProjekt.Controllers
{
    [Authorize]
    public class InsurancesController(
        ILogger<InsurancesController> logger,
        IMediator mediator) : Controller
    {
        // GET: Insurances/Index
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Index(int? page)
        {
            logger.LogInformation("Admin accessed the insurance list (page {Page}).", page ?? 1);

            var query = new GetAllEntitiesQuery<Insurance, InsuranceViewModel>();
            var insurances = await mediator.Send(query);

            var onePageOfInsurances = insurances.ToPagedList(page ?? 1, 4);
            ViewBag.OnePageOfInsurances = onePageOfInsurances;

            return View(insurances);
        }

        // GET: Insurances/Details 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var query = new GetInsuranceForLoggedUserQuery
            {
                InsuranceId = id.Value,
                User = User
            };

            var insurance = await mediator.Send(query);
            if (insurance == null)
                return NotFound();

            return View(insurance);
        }
        
        // GET: Insurances/Create + with specific insuredPersonId
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> Create(int? insuredPersonId = null)
        {
            logger.LogInformation("User accessed the insurance creation page.");
            
            var insurance = new InsuranceViewModel();
            ViewBag.InsuredPersonId = await mediator.Send(new GetInsuredPersonSelectListQuery());

            if (insuredPersonId.HasValue)
            {
                var query = new FindEntityByIdQuery<InsuredPerson, InsuredPersonViewModel>
                    { Id = insuredPersonId.Value };
                var insuredPerson = await mediator.Send(query);
                if (insuredPerson != null)
                {
                    logger.LogInformation(
                        "Insurance creation page opened with preselected insured person with ID: {InsuredPersonId}.",
                        insuredPersonId);
                    ViewBag.InsuredPersonId =
                        await mediator.Send(new GetInsuredPersonSelectListQuery(insuredPersonId.Value));

                    ViewBag.PersonName = await mediator.Send(new GetPersonNameByIdQuery(insuredPersonId.Value)) ??
                                         string.Empty;
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
                var command = new AddEntityCommand<Insurance, InsuranceViewModel>(insurance);
                await mediator.Send(command);

                logger.LogInformation("Insurance created for insured person with ID: {InsuredPersonId}.",
                    insuredPersonId);
                return RedirectToAction(nameof(Index));
            }

            logger.LogWarning(
                "Invalid model state when creating insurance for insured person with ID: {InsuredPersonId}.",
                insuredPersonId);
            if (insuredPersonId != null)
                ViewBag.InsuredPersonId =
                    await mediator.Send(new GetInsuredPersonSelectListQuery(insuredPersonId.Value));
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

            var query = new FindEntityByIdQuery<Insurance, InsuranceViewModel> { Id = id.Value };
            var insurance = await mediator.Send(query);

            logger.LogInformation("User accessed the edit page for insurance ID {InsuranceId}.", id);

            ViewBag.InsuredPersonId = await mediator.Send(new GetInsuredPersonSelectListQuery());
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
                logger.LogWarning("Edit request with mismatched insurance ID with {ProvidedId} != {ModelId}.", id,
                    insurance.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var command = new UpdateEntityCommand<Insurance, InsuranceViewModel>(insurance);
                var updatedInsurance = await mediator.Send(command);
                
                logger.LogInformation("Insurance with ID {InsuranceId} successfully updated.", id);
                return updatedInsurance is null ? NotFound() : RedirectToAction(nameof(Index));
            }

            logger.LogWarning("Insurance edit failed due to invalid model state.");
            ViewBag.InsuredPersonId = await mediator.Send(new GetInsuredPersonSelectListQuery());
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

            var query = new FindEntityByIdQuery<Insurance, InsuranceViewModel> { Id = id.Value };

            var insurance = await mediator.Send(query);

            logger.LogInformation("User accessed the delete page for insurance ID {InsuranceId}.", id);
            return View(insurance);
        }

        // POST: Insurances/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = new RemoveEntityWithIdCommand<Insurance, InsuranceViewModel> { Id = id };
            await mediator.Send(query);

            logger.LogInformation("Insurance with ID {InsuranceId} successfully deleted.", id);
            return RedirectToAction(nameof(Index));
        }
    }
}