using ITnetworkProjekt.Data.Entities;
using ITnetworkProjekt.Features.Common.Commands;
using ITnetworkProjekt.Features.InsuredPersons.Queries;
using ITnetworkProjekt.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ITnetworkProjekt.Controllers
{
    public class AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IStringLocalizer<AccountController> localizer,
        IMediator mediator,
        ILogger<AccountController> logger) : Controller
    {
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult Login(string? returnUrl = null)
        {
            logger.LogInformation("User opened login page.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            logger.LogInformation("User attempted to log in with email {Email}.", model.Email);

            if (ModelState.IsValid)
            {
                var result =
                    await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    logger.LogInformation("User {Email} successfully logged in.", model.Email);
                    return RedirectToLocal(returnUrl);
                }

                logger.LogWarning("Failed login attempt for email {Email}.", model.Email);
                ModelState.AddModelError("Login error", localizer["InvalidLoginInformations"]);
                return View(model);
            }

            logger.LogWarning("Login attempt failed due to invalid model state.");
            return View(model);
        }

        public IActionResult Register(string? returnUrl = null)
        {
            logger.LogInformation("User opened registration page.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            logger.LogInformation(
                "User attempted to register with email {Email} and socialsecretnumber {SocialSecurityNumber}.",
                model.Email, model.SocialSecurityNumber);

            if (ModelState.IsValid)
            {
                //InsuredPersonViewModel? insuredPerson = await insuredPersonManager.GetInsuredPersonByEmailAndSocialSecurityNumberAsync(model.Email, model.SocialSecurityNumber);

                var query = new GetInsuredPersonByEmailAndSocialSecurityNumberQuery(model.Email,
                    model.SocialSecurityNumber);
                InsuredPersonViewModel? insuredPerson = await mediator.Send(query);

                if (insuredPerson == null)
                {
                    logger.LogWarning(
                        "Registration failed: insured person with email {Email} and socialsecretnumber {SocialSecurityNumber} not found.",
                        model.Email, model.SocialSecurityNumber);
                    ModelState.AddModelError("", localizer["InsuredPersonNotFound"]);
                    return View(model);
                }

                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    insuredPerson.UserId = user.Id;

                    var command = new UpdateEntityCommand<InsuredPerson, InsuredPersonViewModel>(insuredPerson);
                    await mediator.Send(command);

                    logger.LogInformation(
                        "User {Email} successfully registered and linked to insured person {InsuredPersonId}.",
                        model.Email, insuredPerson.Id);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }

                foreach (IdentityError error in result.Errors)
                {
                    logger.LogError("Error during registration for {Email}: {Error}", model.Email, error.Description);
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            else
            {
                logger.LogWarning("Registration failed for {Email} due to invalid model state.", model.Email);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            logger.LogInformation("User logged out.");
            await signInManager.SignOutAsync();
            return RedirectToLocal(null);
        }

        public IActionResult AccessDenied(string? returnUrl = null)
        {
            logger.LogWarning("Access denied. Redirecting user.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult TestError()
        {
            throw new Exception("Test exception");
        }
    }
}