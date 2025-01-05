using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GSMTool.Data;
using GSMTool.Models;
using Microsoft.EntityFrameworkCore;

namespace GSMTool.Controllers
{
    [Authorize]
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(ApplicationDbContext context, ILogger<DeviceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var devices = _context.Devices.OrderByDescending(d => d.CreatedDate).ToList();
            return View(devices);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device device)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    device.CreatedDate = DateTime.Now;
                    device.Status = "Nowe";
                    device.ServiceNumber = await GenerateServiceNumber();

                    _context.Devices.Add(device);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Urządzenie zostało dodane pomyślnie.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Błąd podczas dodawania urządzenia");
                    ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania urządzenia.");
                }
            }

            return View(device);
        }

        private async Task<string> GenerateServiceNumber()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var random = new Random();
            string serviceNumber;
            do
            {
                var randomPart = random.Next(1000, 9999).ToString();
                serviceNumber = $"SRV{today}{randomPart}";
            }
            while (await _context.Devices.AnyAsync(d => d.ServiceNumber == serviceNumber));

            return serviceNumber;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var device = await _context.Devices.FindAsync(id);
                if (device != null)
                {
                    device.Status = status;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Zaktualizowano status urządzenia {id} na: {status}");
                    return Json(new { success = true });
                }
                _logger.LogWarning($"Nie znaleziono urządzenia o ID: {id}");
                return Json(new { success = false, message = "Nie znaleziono urządzenia" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas aktualizacji statusu: {ex.Message}");
                return Json(new { success = false, message = "Wystąpił błąd podczas aktualizacji" });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            try
            {
                var device = await _context.Devices.FindAsync(id);
                if (device == null)
                {
                    return Json(new { success = false, message = "Nie znaleziono urządzenia" });
                }

                _context.Devices.Remove(device);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Usunięto urządzenie o ID: {id}");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd podczas usuwania urządzenia: {ex.Message}");
                return Json(new { success = false, message = "Wystąpił błąd podczas usuwania urządzenia" });
            }
        }
        public async Task<IActionResult> PrintOrder(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }
    }
}