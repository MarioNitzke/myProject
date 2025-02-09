using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ITnetworkProjekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly InsuredPersonManager _insuredPersonManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            InsuredPersonManager insuredPersonManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IStringLocalizer<AccountController> localizer,
            ILogger<AccountController> logger)
        {
            _insuredPersonManager = insuredPersonManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;
            _logger = logger;
        }
    

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult Login(string? returnUrl = null)
        {
            _logger.LogInformation("User opened login page.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _logger.LogInformation("User attempted to log in with email {Email}.", model.Email);

            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {Email} successfully logged in.", model.Email);
                    return RedirectToLocal(returnUrl);
                }

                _logger.LogWarning("Failed login attempt for email {Email}.", model.Email);
                ModelState.AddModelError("Login error", _localizer["InvalidLoginInformations"]);
                return View(model);
            }

            _logger.LogWarning("Login attempt failed due to invalid model state.");
            return View(model);
        }

        public IActionResult Register(string? returnUrl = null)
        {
            _logger.LogInformation("User opened registration page.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _logger.LogInformation("User attempted to register with email {Email} and socialsecretnumber {SocialSecurityNumber}.", model.Email, model.SocialSecurityNumber);

            if (ModelState.IsValid)
            {
                InsuredPersonViewModel? insuredPerson = await _insuredPersonManager.GetInsuredPersonByEmailAndSocialSecurityNumberAsync(model.Email, model.SocialSecurityNumber);

                if (insuredPerson == null)
                {
                    _logger.LogWarning("Registration failed: insured person with email {Email} and socialsecretnumber {SocialSecurityNumber} not found.", model.Email, model.SocialSecurityNumber);
                    ModelState.AddModelError("", _localizer["InsuredPersonNotFound"]);
                    return View(model);
                }

                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    insuredPerson.UserId = user.Id;
                    await _insuredPersonManager.UpdateInsuredPerson(insuredPerson);

                    _logger.LogInformation("User {Email} successfully registered and linked to insured person {InsuredPersonId}.", model.Email, insuredPerson.Id);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }

                foreach (IdentityError error in result.Errors)
                {
                    _logger.LogError("Error during registration for {Email}: {Error}", model.Email, error.Description);
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            else
            {
                _logger.LogWarning("Registration failed for {Email} due to invalid model state.", model.Email);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logged out.");
            await _signInManager.SignOutAsync();
            return RedirectToLocal(null);
        }

        public IActionResult AccessDenied(string? returnUrl = null)
        {
            _logger.LogWarning("Access denied. Redirecting user.");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
