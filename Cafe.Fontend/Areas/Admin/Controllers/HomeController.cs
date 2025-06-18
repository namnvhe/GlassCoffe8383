using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var token = GetTokenFromSession();
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserPhoto = HttpContext.Session.GetString("UserPhoto");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        private string GetTokenFromSession()
        {
            return HttpContext.Session.GetString("JWTToken");
        }

        private void ClearTokenFromSession()
        {
            HttpContext.Session.Remove("JWTToken");
        }
    }
}
