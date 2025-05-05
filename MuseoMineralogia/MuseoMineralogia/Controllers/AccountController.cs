using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MuseoMineralogia.Models;
using MuseoMineralogia.Services;
using MuseoMineralogia.ViewModels;
using System.Threading.Tasks;

namespace MuseoMineralogia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Utente> _userManager;
        private readonly SignInManager<Utente> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _env;

        public AccountController(
            UserManager<Utente> userManager,
            SignInManager<Utente> signInManager,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _env = env;
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
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Questa email è già registrata nel sistema.");
                    return View(model);
                }
                var user = new Utente
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    EmailConfirmed = true
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

        // Azione GET per la pagina "Password dimenticata"
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Azione POST per gestire la richiesta di reset password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordModel);

            // Controlla che l'email non sia null
            if (string.IsNullOrEmpty(forgotPasswordModel?.Email))
            {
                ModelState.AddModelError(string.Empty, "Email non valida");
                return View(forgotPasswordModel);
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);

            // Per sicurezza, reindirizza comunque alla pagina di conferma anche se l'utente non esiste
            if (user == null)
                return RedirectToAction("ForgotPasswordConfirmation");

            // Genera il token di reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Crea il link di callback
            var callback = Url.Action("ResetPassword", "Account",
                                     new { token, email = user.Email }, Request.Scheme);

            // Controlla che gli argomenti email e subject non siano null
            if (!string.IsNullOrEmpty(user.Email))
            {
                // Invia l'email con il link di reset
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset Password - Museo Mineralogia",
                    $"<h2>Reset della password</h2><p>Per reimpostare la tua password, <a href='{callback}'>clicca qui</a>.</p>");
            }

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // Pagina di conferma per il "Password dimenticata"
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Azione GET per la pagina di reset password (quando l'utente clicca sul link nell'email)
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null)
            {
                return BadRequest("È necessario un token per reimpostare la password.");
            }

            var model = new ResetPasswordModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        // Azione POST per elaborare il reset della password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
                return View(resetPasswordModel);

            // Controlla che l'email non sia null
            if (string.IsNullOrEmpty(resetPasswordModel?.Email))
            {
                ModelState.AddModelError(string.Empty, "Email non valida");
                return View(resetPasswordModel);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);

            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            // Controlla che il token non sia null
            if (string.IsNullOrEmpty(resetPasswordModel?.Token))
            {
                ModelState.AddModelError(string.Empty, "Token non valido");
                return View(resetPasswordModel);
            }

            // Controlla che la password non sia null
            if (string.IsNullOrEmpty(resetPasswordModel?.Password))
            {
                ModelState.AddModelError(string.Empty, "Password non valida");
                return View(resetPasswordModel);
            }

            // Resetta la password usando il token e la nuova password
            var resetPassResult = await _userManager.ResetPasswordAsync(user,
                                                                     resetPasswordModel.Token,
                                                                     resetPasswordModel.Password);

            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }

            return RedirectToAction("ResetPasswordConfirmation");
        }

        // Pagina di conferma per il reset password completato
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}