using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SFA.DAS.DownloadService.Web.Controllers
{
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
        public ActionResult Api()
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

        [HttpGet]
        [ResponseCache(Duration = 86400)]
        public ContentResult RobotsText()
        {
            var builder = new StringBuilder();

            builder.AppendLine("User-agent: *");
            builder.AppendLine("Disallow: /");

            return Content(builder.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}
