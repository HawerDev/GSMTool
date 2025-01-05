using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GSMTool.Data;
using GSMTool.Models;
using Microsoft.EntityFrameworkCore;

namespace GSMTool.Controllers
{
    [Authorize] 
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PartsController> _logger;

        public PartsController(ApplicationDbContext context, ILogger<PartsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var parts = await _context.Parts.OrderBy(p => p.Name).ToListAsync();
            return View(parts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Part part)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    part.QRCode = Guid.NewGuid().ToString();

                    _context.Parts.Add(part);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Część została dodana pomyślnie.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Błąd podczas dodawania części: {ex.Message}");
                    ModelState.AddModelError("", "Wystąpił błąd podczas dodawania części.");
                }
            }
            return View(part);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            try
            {
                var part = await _context.Parts.FindAsync(id);
                if (part != null)
                {
                    part.Quantity = quantity;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Nie znaleziono części" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas aktualizacji ilości: {ex.Message}");
                return Json(new { success = false, message = "Wystąpił błąd podczas aktualizacji" });
            }
       
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var part = await _context.Parts.FindAsync(id);
                if (part != null)
                {
                    _context.Parts.Remove(part);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Nie znaleziono części" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas usuwania części: {ex.Message}");
                return Json(new { success = false, message = "Wystąpił błąd podczas usuwania" });
            }
        }
    }
}