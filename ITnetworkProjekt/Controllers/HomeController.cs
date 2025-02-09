using System.Diagnostics;
using ITnetworkProjekt.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITnetworkProjekt.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public IActionResult Index()
        {
            logger.LogInformation("User accessed the homepage.");
            return View();
        }

        public IActionResult Aboutapplication()
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
