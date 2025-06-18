using Cafe.BusinessObjects.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.Fontend.ViewComponents
{
    public class ForgotPasswordViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ForgotPasswordRequest();
            return View(model);
        }
    }
}
