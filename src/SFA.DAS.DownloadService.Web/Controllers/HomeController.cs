using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Apar");
        }

        [Route("cookie-details")]
        public IActionResult CookieDetails()
        {
            return View();
        }

        [Route("cookie-settings")]
        public IActionResult CookieSettings()
        {
            return View();
        }

        public ActionResult Api()
        {
            return View();
        }

        [Route("terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [Route("accessibility-statement")]
        public IActionResult Accessibility()
        {
            return View();
        }

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
