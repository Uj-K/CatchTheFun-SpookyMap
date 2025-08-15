using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // For accessing configuration settings (for Google Maps API key) 1. using Microsoft Extensions
using System.Diagnostics;
using CatchTheFun.SpookyMap.Web.Data;

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class EventMapController : Controller
    {
        private readonly ILogger<EventMapController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // 2. Inject IConfiguration to access app settings

        public EventMapController(ILogger<EventMapController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration; // 3. Initialize IConfiguration in the constructor
        }


    public async Task<IActionResult> Index(int? highlightId)
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            ViewData["GoogleMapsApiKey"] = apiKey;
            ViewData["HighlightId"] = highlightId; // 전달하여 방금 추가한 집을 강조

            var locations = await _context.EventLocations
                .Where(e => e.Lat != null && e.Lng != null)
                .ToListAsync(); // 비동기

            return View(locations); // ★ Model을 넘겨야 함
        }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
