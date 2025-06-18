using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public AuthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7151/api/";

            if (!_apiUrl.EndsWith("/"))
            {
                _apiUrl += "/";
            }

            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Tạo form data để gửi đến API
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.Email), "Email");
                formData.Add(new StringContent(model.Password), "Password");

                var loginUrl = $"{_apiUrl}Auth/login";
                var response = await _httpClient.PostAsync(loginUrl, formData);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResult = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

                    // Lưu token và thông tin user vào session
                    HttpContext.Session.SetString("JWTToken", loginResult.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", loginResult.RefreshToken);
                    HttpContext.Session.SetString("UserRole", loginResult.User.Role);
                    HttpContext.Session.SetString("UserName", loginResult.User.FullName);
                    HttpContext.Session.SetString("UserEmail", loginResult.User.Email);
                    HttpContext.Session.SetString("UserPhoto", loginResult.User.PhotoPath ?? "");

                    TempData["SuccessMessage"] = loginResult.Message.ToString();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    ModelState.AddModelError("", errorResult.message?.ToString() ?? "Đăng nhập thất bại");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi kết nối đến server");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "Bạn đã đăng xuất thành công";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public IActionResult LockScreen()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWTToken")))
            {
                return RedirectToAction("Login");
            }

            var previousUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(previousUrl) && !previousUrl.Contains("LockScreen") && !previousUrl.Contains("Login"))
            {
                HttpContext.Session.SetString("PreviousUrl", previousUrl);
            }
            else
            {
                HttpContext.Session.SetString("PreviousUrl", Url.Action("Index", "Home", new { area = "Admin" })!);
            }

            var model = new LockScreenRequest();
            return View("LockScreen", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(LockScreenRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View("LockScreen", model);
            }

            try
            {
                var email = HttpContext.Session.GetString("UserEmail");
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(email!), "Email");
                formData.Add(new StringContent(model.Password), "Password");

                var loginUrl = $"{_apiUrl}Auth/login";
                var response = await _httpClient.PostAsync(loginUrl, formData);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var previousUrl = HttpContext.Session.GetString("PreviousUrl") ?? Url.Action("Index", "Home", new { area = "Admin" })!;
                    return Redirect(previousUrl);
                }
                else
                {
                    ViewData["ErrorMessage"] = "Incorrect password";
                    return View("LockScreen", model);
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = "An error occurred while connecting to the server";
                return View("LockScreen", model);
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
