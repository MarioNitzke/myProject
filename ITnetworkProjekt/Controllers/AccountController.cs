using ITnetworkProjekt.Data;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITnetworkProjekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IDbContextFactory<ApplicationDbContext> contextFactory;

        public AccountController
        (
            IDbContextFactory<ApplicationDbContext> contextFactory,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager
        )
        {
            this.contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            this.userManager = userManager;
            this.signInManager = signInManager;
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

                ModelState.AddModelError("Login error", "Neplatné přihlašovací údaje.");
                return View(model);
            }

            // Pokud byly odeslány neplatné údaje, vrátíme uživatele k přihlašovacímu formuláři
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
                using var _context = await contextFactory.CreateDbContextAsync();
                //Zkouším najít inusuredPerson pomoci email a RČ
                var insuredPerson = await _context.InsuredPerson
                .FirstOrDefaultAsync(m => (m.Email == model.Email && m.SocialSecurityNumber == model.SocialSecurityNumber));
                if (insuredPerson == null)
                {
                    ModelState.AddModelError("", "Pojištěnec s tímto e-mailem a rodným číslem nebyl nalezen.");
                    return View(model);
                }

                IdentityUser user = new IdentityUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    insuredPerson.UserId = user.Id;

                    // Update UserId v insuredPerson 
                    _context.InsuredPerson.Update(insuredPerson);
                    await _context.SaveChangesAsync();

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


