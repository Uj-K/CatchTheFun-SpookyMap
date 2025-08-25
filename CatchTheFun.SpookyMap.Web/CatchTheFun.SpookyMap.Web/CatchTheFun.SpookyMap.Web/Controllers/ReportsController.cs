using CatchTheFun.SpookyMap.Web.Data;
using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ReportsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: /Reports/Create?eventId=123
        [HttpGet]
        public async Task<IActionResult> Create(int? eventId)
        {
            EventLocation? loc = null;
            if (eventId.HasValue)
            {
                loc = await _db.EventLocations.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventId.Value);
                if (loc == null)
                {
                    return NotFound();
                }
            }

            var vm = new ReportCreateViewModel
            {
                EventLocationId = eventId,
                SelectedLocation = loc
            };
            return View(vm);
        }

        // POST: /Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportCreateViewModel vm)
        {
            EventLocation? loc = null;
            if (vm.EventLocationId.HasValue)
            {
                loc = await _db.EventLocations.AsNoTracking().FirstOrDefaultAsync(e => e.Id == vm.EventLocationId.Value);
                if (loc == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected location not found.");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.SelectedLocation = loc;
                return View(vm);
            }

            // Persist to a simple local file under ContentRoot/App_Data/reports
            var store = new StoredReport
            {
                EventLocationId = vm.EventLocationId,
                ReporterName = vm.ReporterName,
                ReporterEmail = vm.ReporterEmail,
                Message = vm.Message,
                SnapshotName = loc?.Name,
                SnapshotAddress = loc?.Address
            };

            try
            {
                var dir = Path.Combine(_env.ContentRootPath, "App_Data", "reports");
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, $"{DateTime.UtcNow:yyyyMMddHHmmss}-{store.Id}.json");
                await System.IO.File.WriteAllTextAsync(path, System.Text.Json.JsonSerializer.Serialize(store));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not store report: {ex.Message}");
                vm.SelectedLocation = loc;
                return View(vm);
            }

            TempData["ReportSent"] = "Thank you for your report. Our moderators will review it.";
            if (vm.EventLocationId.HasValue)
            {
                return RedirectToAction("Index", "EventMap", new { highlightId = vm.EventLocationId.Value });
            }
            return RedirectToAction("Index", "EventMap");
        }
    }
}
