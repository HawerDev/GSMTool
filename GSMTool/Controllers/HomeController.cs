using Microsoft.AspNetCore.Mvc;
using GSMTool.Data;
using GSMTool.Models;

namespace GSMTool.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckStatus(string serviceNumber)
        {
            if (string.IsNullOrEmpty(serviceNumber))
            {
                return Json(new { success = false, message = "Wprowadź numer serwisowy" });
            }

            var device = _context.Devices.FirstOrDefault(d => d.ServiceNumber == serviceNumber);
            if (device == null)
            {
                return Json(new { success = false, message = "Nie znaleziono urządzenia o podanym numerze" });
            }

            return Json(new
            {
                success = true,
                status = device.Status,
                model = device.Model,
                createDate = device.CreatedDate.ToString("dd.MM.yyyy")
            });
        }
    }
}