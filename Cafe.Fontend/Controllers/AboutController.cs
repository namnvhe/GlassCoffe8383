using Microsoft.AspNetCore.Mvc;

namespace Cafe.Fontend.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
