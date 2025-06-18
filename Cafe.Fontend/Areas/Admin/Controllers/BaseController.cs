using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = RedirectToAction("Login", "Auth", new { area = "Admin" });
                return;
            }

            // Kiểm tra token có hết hạn không
            if (IsTokenExpired(token))
            {
                HttpContext.Session.Clear();
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                context.Result = RedirectToAction("Login", "Auth", new { area = "Admin" });
                return;
            }

            // Kiểm tra role
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                context.Result = RedirectToAction("AccessDenied", "Auth", new { area = "Admin" });
                return;
            }

            base.OnActionExecuting(context);
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return jwtToken.ValidTo <= DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
