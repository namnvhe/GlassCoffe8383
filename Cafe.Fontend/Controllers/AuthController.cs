using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.Controllers
{
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
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /*        [HttpPost]
                public async Task<IActionResult> Register(RegisterRequest model)
                {
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    try
                    {
                        // Tạo JSON content để gửi đến API
                        var jsonContent = JsonConvert.SerializeObject(model);
                        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                        var registerUrl = $"{_apiUrl}Auth/register";
                        var response = await _httpClient.PostAsync(registerUrl, content);

                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {responseContent}");
                        if (response.IsSuccessStatusCode)
                        {
                            var registerResult = JsonConvert.DeserializeObject<AuthResponse>(responseContent);

                            // Lưu thông tin user vào session
                            HttpContext.Session.SetString("JWTToken", registerResult.AccessToken);
                            HttpContext.Session.SetString("RefreshToken", registerResult.RefreshToken);
                            HttpContext.Session.SetString("UserRole", registerResult.User.Role);
                            HttpContext.Session.SetString("UserName", registerResult.User.FullName);
                            HttpContext.Session.SetString("UserEmail", registerResult.User.Email);
                            HttpContext.Session.SetString("UserPhoto", registerResult.User.PhotoPath ?? "");

                            TempData["SuccessMessage"] = "Đăng ký thành công!";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            var errorResult = JsonConvert.DeserializeObject<dynamic>(responseContent);
                            Console.WriteLine($"Error Message: {errorResult.message}");
                            ModelState.AddModelError("", errorResult.message?.ToString() ?? "Đăng ký thất bại");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                        ModelState.AddModelError("", "Có lỗi xảy ra khi kết nối đến server");
                    }

                    return View(model);
                }*/

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var registerUrl = $"{_apiUrl}Auth/register";
                var response = await _httpClient.PostAsync(registerUrl, content);

                var responseContent = await response.Content.ReadAsStringAsync();              

                if (response.IsSuccessStatusCode)
                {
                    var settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore // Bỏ qua các thuộc tính không khớp
                    };
                    var registerResult = JsonConvert.DeserializeObject<RegisterResponse>(responseContent, settings);

                    if (registerResult.IsSuccess)
                    {
                        // Không lưu token vì đăng ký không trả về token
                        HttpContext.Session.SetString("UserRole", registerResult.User?.Role ?? "Customer");
                        HttpContext.Session.SetString("UserName", registerResult.User?.FullName ?? "");
                        HttpContext.Session.SetString("UserEmail", registerResult.User?.Email ?? "");
                        HttpContext.Session.SetString("UserPhoto", registerResult.User?.PhotoPath ?? "");

                        TempData["SuccessMessage"] = registerResult.Message;
                        return RedirectToAction("Login", "Auth");
                    }
                    else
                    {
                        ModelState.AddModelError("", registerResult.Message);
                    }
                }
                else
                {
                    var errorResult = JsonConvert.DeserializeObject<RegisterResponse>(responseContent);
                    ModelState.AddModelError("", errorResult.Message ?? "Đăng ký thất bại");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi kết nối đến server");
            }

            return View(model);
        }
    }
}
