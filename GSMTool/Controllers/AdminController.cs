using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GSMTool.Data;
using GSMTool.Models;
using Microsoft.EntityFrameworkCore;

namespace GSMTool.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind("Username,Email,Role")] User user, string Password)
        {
            if (string.IsNullOrEmpty(Password))
            {
                ModelState.AddModelError("Password", "Hasło jest wymagane");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sprawdzenie czy użytkownik istnieje
                    if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    {
                        ModelState.AddModelError("Username", "Ta nazwa użytkownika jest już zajęta");
                        return View(user);
                    }

                    // Hashowanie hasła i przypisanie do modelu
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                    // Dodanie użytkownika
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Użytkownik został dodany pomyślnie";
                    return RedirectToAction(nameof(Users));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Błąd podczas dodawania użytkownika: {ex.Message}");
                    ModelState.AddModelError("", "Wystąpił błąd podczas tworzenia użytkownika");
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Użytkownik został usunięty pomyślnie";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, string role)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Role = role;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Nie znaleziono użytkownika" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas aktualizacji roli: {ex.Message}");
                return Json(new { success = false, message = "Wystąpił błąd podczas aktualizacji" });
            }
        }
    }
}