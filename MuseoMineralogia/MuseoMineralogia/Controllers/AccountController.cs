using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MuseoMineralogia.Models;
using MuseoMineralogia.ViewModels;
using System.Threading.Tasks;

namespace MuseoMineralogia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;

        public AccountController(UserManager<Utente> userManager, SignInManager<Utente> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Utente
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Cognome = model.Cognome
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Utente");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "L'account è bloccato. Riprova più tardi.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Tentativo di accesso non valido.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}