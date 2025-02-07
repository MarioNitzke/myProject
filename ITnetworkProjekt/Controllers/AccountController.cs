using System.Diagnostics;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ITnetworkProjekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly InsuredPersonManager insuredPersonManager;
        private readonly IStringLocalizer<AccountController> localizer;

        public AccountController(
            InsuredPersonManager insuredPersonManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IStringLocalizer<AccountController> localizer)
        {
            this.insuredPersonManager = insuredPersonManager ?? throw new ArgumentNullException(nameof(insuredPersonManager));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
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
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result =
                    await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                    return RedirectToLocal(returnUrl);

                ModelState.AddModelError("Login error", localizer["InvalidLoginInformations"]);
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                InsuredPersonViewModel? insuredPerson = await insuredPersonManager.GetInsuredPersonByEmailAndSSNAsync(model.Email, model.SocialSecurityNumber);

                if (insuredPerson == null)
                {
                    ModelState.AddModelError("", localizer["InsuredPersonNotFound"]);
                    return View(model);
                }

                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    insuredPerson.UserId = user.Id;
                    await insuredPersonManager.UpdateInsuredPerson(insuredPerson);

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToLocal(null);
        }

        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}