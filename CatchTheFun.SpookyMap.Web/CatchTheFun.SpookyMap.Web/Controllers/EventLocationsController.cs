using CatchTheFun.SpookyMap.Web.Data;
using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json; // for Geocoding
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http; // for Geocoding 
using System.Threading.Tasks;

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class EventLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EventLocationsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            _context = context;
            _configuration = configuration;
            _env = env;
        }


        // GET: EventLocations
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventLocations.ToListAsync());
        }

        // GET: EventLocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLocation = await _context.EventLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventLocation == null)
            {
                return NotFound();
            }

            return View(eventLocation);
        }

        // GET: EventLocations/Create
        public IActionResult Create()
        {
            ViewData["GoogleMapsApiKey"] = _configuration["GoogleMaps:ApiKey"];
            return View();
        }

        // POST: EventLocations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Address,Description,SomethingElse, PhotoUrl, StartTime, EndTime")] 
            EventLocation eventLocation, IFormFile? photo)
        {
            // 1. 업로드 검증
            if (photo is { Length: > 0 })
            {
                var ok = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
                if (!ok.Contains(photo.ContentType))
                    ModelState.AddModelError(nameof(eventLocation.PhotoUrl), "Only jpg/png/webp/gif allowed.");
                if (photo.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError(nameof(eventLocation.PhotoUrl), "Max 5MB.");
            }

            if (ModelState.IsValid)
            {
                // 1) Photo save first (keeps URL if provided)
                if (photo is { Length: > 0 })
                {
                    var y = DateTime.UtcNow.ToString("yyyy");
                    var m = DateTime.UtcNow.ToString("MM");
                    var relDir = Path.Combine("uploads", y, m);
                    var absDir = Path.Combine(_env.WebRootPath, relDir);
                    Directory.CreateDirectory(absDir);

                    var ext = Path.GetExtension(photo.FileName);
                    if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

                    var fileName = $"{Guid.NewGuid():N}{ext}";
                    var absPath = Path.Combine(absDir, fileName);

                    await using (var fs = new FileStream(absPath, FileMode.Create))
                        await photo.CopyToAsync(fs);

                    eventLocation.PhotoUrl = "/" + Path.Combine(relDir, fileName).Replace("\\", "/");
                }

                // 2) Geocode address
                var coords = await GetCoordinatesFromAddress(eventLocation.Address);
                if (coords != null)
                {
                    eventLocation.Lat = coords.Value.lat;
                    eventLocation.Lng = coords.Value.lng;
                    _context.Add(eventLocation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Address", "Invalid address. Could not fetch location data.");
                }
            }

            // If we got this far, something failed; return view for correction
            return View(eventLocation);
        }


        // GET: EventLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLocation = await _context.EventLocations.FindAsync(id);
            if (eventLocation == null)
            {
                return NotFound();
            }
            return View(eventLocation);
        }

        // POST: EventLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Description,SomethingElse, PhotoUrl, StartTime, EndTime")] EventLocation form, IFormFile? photo)
        {
            if (id != form.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var entity = await _context.EventLocations.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            // Photo upload validation (same as in Create)
            if (photo is { Length: > 0 })
            {
                var ok = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
                if (!ok.Contains(photo.ContentType))
                    ModelState.AddModelError(nameof(form.PhotoUrl), "Only jpg/png/webp/gif allowed.");
                if (photo.Length > 5 * 1024 * 1024)
                    ModelState.AddModelError(nameof(form.PhotoUrl), "Max 5MB.");

                if (!ModelState.IsValid)
                    return View(form);
            }

            // Update scalar fields
            entity.Name = form.Name;
            entity.Description = form.Description;
            entity.SomethingElse = form.SomethingElse;
            entity.StartTime = form.StartTime;
            entity.EndTime = form.EndTime;

            // Address change handling
            var oldAddress = entity.Address;
            if (!string.Equals(oldAddress, form.Address, StringComparison.OrdinalIgnoreCase))
            {
                // Geocode new address
                var coords = await GetCoordinatesFromAddress(form.Address);
                if (coords == null)
                {
                    ModelState.AddModelError("Address", "Invalid address. Could not fetch location data.");
                    return View(form);
                }
                entity.Address = form.Address;
                entity.Lat = coords.Value.lat;
                entity.Lng = coords.Value.lng;
            }
            else
            {
                entity.Address = form.Address; // unchanged
            }

            // Photo upload
            if (photo is { Length: > 0 })
            {
                var y = DateTime.UtcNow.ToString("yyyy");
                var m = DateTime.UtcNow.ToString("MM");
                var relDir = Path.Combine("uploads", y, m);
                var absDir = Path.Combine(_env.WebRootPath, relDir);
                Directory.CreateDirectory(absDir);

                var ext = Path.GetExtension(photo.FileName);
                if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var absPath = Path.Combine(absDir, fileName);

                await using (var fs = new FileStream(absPath, FileMode.Create))
                    await photo.CopyToAsync(fs);

                entity.PhotoUrl = "/" + Path.Combine(relDir, fileName).Replace("\\", "/");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: EventLocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventLocation = await _context.EventLocations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventLocation == null)
            {
                return NotFound();
            }

            return View(eventLocation);
        }

        // POST: EventLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventLocation = await _context.EventLocations.FindAsync(id);
            if (eventLocation != null)
            {
                _context.EventLocations.Remove(eventLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventLocationExists(int id)
        {
            return _context.EventLocations.Any(e => e.Id == id);
        }

        // Geocoding 
        private async Task<(double lat, double lng)?> GetCoordinatesFromAddress(string address)
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Google Maps API key not found in appsettings.json.");
            }

            var httpClient = new HttpClient();
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&language=en&key={apiKey}";

            var response = await httpClient.GetStringAsync(url);
            dynamic result = JsonConvert.DeserializeObject(response);

            if (result.status == "OK")
            {
                double lat = result.results[0].geometry.location.lat;
                double lng = result.results[0].geometry.location.lng;
                return (lat, lng);
            }

            return null;
        }


    }
}
