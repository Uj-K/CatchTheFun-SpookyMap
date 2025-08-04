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

        public EventLocationsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
            return View();
        }

        // POST: EventLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description,SomethingElse")] EventLocation eventLocation)
        {
            if (ModelState.IsValid)
            {
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Description,SomethingElse")] EventLocation eventLocation)
        {
            if (id != eventLocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventLocationExists(eventLocation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(eventLocation);
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
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

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
