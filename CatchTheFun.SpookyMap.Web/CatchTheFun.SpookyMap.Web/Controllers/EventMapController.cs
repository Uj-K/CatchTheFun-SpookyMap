using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class EventMapController : Controller
    {
        private readonly ILogger<EventMapController> _logger;
        private readonly GoogleMapsOptions _mapOptions;

        public EventMapController(ILogger<EventMapController> logger, IOptions<GoogleMapsOptions> mapOptions)
        {
            _logger = logger;
            _mapOptions = mapOptions.Value;
        }

        // Index action to render the main view of the event map
        public IActionResult Index()
        {
            ViewData["GoogleMapsApiKey"] = _mapOptions.ApiKey;
            return View();
        }

        // Error action to handle errors and display error view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
