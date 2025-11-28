using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Apar");
        }

        [HttpGet]
        [Route("cookie-details")]
        public IActionResult CookieDetails()
        {
            return View();
        }

        [HttpGet]
        [Route("cookie-settings")]
        public IActionResult CookieSettings()
        {
            return View();
        }

        [HttpGet]
        [Route("terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet]
        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        [Route("accessibility-statement")]
        public IActionResult Accessibility()
        {
            return View();
        }
    }
}
