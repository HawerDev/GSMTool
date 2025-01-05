using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using GSMTool.Models;
using GSMTool.ViewModels;
using GSMTool.Data;
using Microsoft.Extensions.Logging;

namespace GSMTool.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation($"Rozpoczęcie procesu logowania dla użytkownika: {model.Username}");

            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user != null)
                {
                    _logger.LogInformation($"Znaleziono użytkownika. Email: {user.Email}, Rola: {user.Role}");
                    _logger.LogInformation($"Hash w bazie: {user.PasswordHash}");
                    _logger.LogInformation($"Wprowadzone hasło: {model.Password}");

                    var isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                    _logger.LogInformation($"Wynik weryfikacji hasła: {isPasswordValid}");

                    if (isPasswordValid)
                    {
                        var claims = new List<Claim>
                       {
                           new Claim(ClaimTypes.Name, user.Username),
                           new Claim(ClaimTypes.Role, user.Role)
                       };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        _logger.LogInformation("Logowanie zakończone sukcesem");
                        return RedirectToAction("Index", "Home");
                    }
                    _logger.LogWarning("Niepoprawne hasło");
                }
                else
                {
                    _logger.LogWarning($"Nie znaleziono użytkownika o nazwie: {model.Username}");
                }

                ModelState.AddModelError(string.Empty, "Nieprawidłowa nazwa użytkownika lub hasło");
            }
            else
            {
                _logger.LogWarning("Model nie jest prawidłowy");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Błąd walidacji: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Ta nazwa użytkownika jest już zajęta");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = "Użytkownik"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
               {
                   new Claim(ClaimTypes.Name, user.Username),
                   new Claim(ClaimTypes.Role, user.Role)
               };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}