using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Roatp");
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
