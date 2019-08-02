using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DownloadService.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Download");
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}




        public ActionResult Api()
        {
            return View();
        }

        [ResponseCache(Duration = 86400)]
        public ContentResult RobotsText()
        {
            var builder = new StringBuilder();

            builder.AppendLine("User-agent: *");

            // MFCMFC Need to replicate this in azure
            //if (!bool.Parse(CloudConfigurationManager.GetSetting("FeatureToggle.RobotsAllowFeature")??"false"))
            //{
            builder.AppendLine("Disallow: /");
            //}

            return Content(builder.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}
