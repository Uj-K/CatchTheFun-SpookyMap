using System.Diagnostics;
using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // For accessing configuration settings (for Google Maps API key) 1. using Microsoft Extensions

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class EventMapController : Controller
    {
        private readonly ILogger<EventMapController> _logger;
        private readonly IConfiguration _configuration; // 2. Inject IConfiguration to access app settings

        public EventMapController(ILogger<EventMapController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration; // 3. Initialize IConfiguration in the constructor
        }

        public IActionResult Index()
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            ViewData["GoogleMapsApiKey"] = apiKey;
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
