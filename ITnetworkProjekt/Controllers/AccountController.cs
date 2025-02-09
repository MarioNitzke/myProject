using System.Diagnostics;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace ITnetworkProjekt.Controllers
{
    public class AccountController(
        InsuredPersonManager insuredPersonManager,
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IStringLocalizer<AccountController> localizer,
        ILogger<AccountController> logger) : Controller
    {
        private readonly UserManager<IdentityUser> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        private readonly SignInManager<IdentityUser> signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        private readonly InsuredPersonManager insuredPersonManager = insuredPersonManager ?? throw new ArgumentNullException(nameof(insuredPersonManager));
        private readonly IStringLocalizer<AccountController> localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        private readonly ILogger<AccountController> logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
                Microsoft.AspNetCore.Identity.SignInResult result =
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
            logger.LogInformation("User attempted to register with email {Email} and socialsecretnumber {SocialSecurityNumber}.", model.Email, model.SocialSecurityNumber);

            if (ModelState.IsValid)
            {
                InsuredPersonViewModel? insuredPerson = await insuredPersonManager.GetInsuredPersonByEmailAndSSNAsync(model.Email, model.SocialSecurityNumber);

                if (insuredPerson == null)
                {
                    logger.LogWarning("Registration failed: insured person with email {Email} and socialsecretnumber {SocialSecurityNumber} not found.", model.Email, model.SocialSecurityNumber);
                    ModelState.AddModelError("", localizer["InsuredPersonNotFound"]);
                    return View(model);
                }

                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    insuredPerson.UserId = user.Id;
                    await insuredPersonManager.UpdateInsuredPerson(insuredPerson);

                    logger.LogInformation("User {Email} successfully registered and linked to insured person {InsuredPersonId}.", model.Email, insuredPerson.Id);
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

        public IActionResult AccessDenied(string returnUrl = null)
        {
            logger.LogWarning("Access denied. Redirecting user.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
