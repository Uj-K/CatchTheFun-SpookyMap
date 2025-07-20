using System.Diagnostics;
using CatchTheFun.SpookyMap.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatchTheFun.SpookyMap.Web.Controllers
{
    public class EventMapController : Controller
    {
        private readonly ILogger<EventMapController> _logger;

        public EventMapController(ILogger<EventMapController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
