using System.Web.Mvc;
using CaptchaLib;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.IsCaptchaValid = false;
            return View();
        }

        [HttpPost]
        public ActionResult Index(TestCaptchaModel model)
        {
            ViewBag.IsCaptchaValid = ModelState.IsValid;
            return View();
        }

        public ActionResult GetCaptcha()
        {
            return this.Captcha();
        }

    }
}
