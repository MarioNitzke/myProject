using System.Diagnostics;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITnetworkProjekt.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        public IActionResult Index()
        {
            logger.LogInformation("User accessed the homepage.");
            return View();
        }

        public IActionResult AboutApplication()
        {
            logger.LogInformation("User accessed the About Application page.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            logger.LogError("An error occurred. Request ID: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}