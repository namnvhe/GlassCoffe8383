using Cafe.BusinessObjects.Models.Response;
using Cafe.Fontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Cafe.Fontend.Controllers
{
    public class ContactController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _googleAppsScriptUrl;
        private readonly string _apiUrl;

        public ContactController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _googleAppsScriptUrl = "https://script.google.com/macros/s/AKfycbyOOzWL5fSoEzCicSY4WmHSfEqMwFBivxVw16fkwMb4tGny9vbOI0vdu-8Y8Vx6GMOv/exec";
            _apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7151/api/";
            if (!_apiUrl.EndsWith("/")) _apiUrl += "/";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            var userInfo = await GetCurrentUserInfo();
            return Json(userInfo);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContact()
        {
            try
            {
                ContactModel model = null;

                // Kiểm tra Content-Type và xử lý tương ứng
                var contentType = Request.ContentType?.ToLower();

                if (contentType != null && contentType.Contains("application/json"))
                {
                    // Xử lý JSON request
                    using var reader = new StreamReader(Request.Body);
                    var json = await reader.ReadToEndAsync();
                    Console.WriteLine($"Raw JSON: {json}");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    model = System.Text.Json.JsonSerializer.Deserialize<ContactModel>(json, options);
                }
                else
                {
                    // Xử lý form-data request (fallback)
                    model = new ContactModel
                    {
                        Name = Request.Form["name"].ToString(),
                        Email = Request.Form["email"].ToString(),
                        Phone = Request.Form["phone"].ToString(),
                        Subject = Request.Form["subject"].ToString(),
                        Message = Request.Form["message"].ToString(),
                        Token = Request.Form["token"].ToString()
                    };
                }

                Console.WriteLine($"Parsed model - Token: '{model?.Token}', Name: '{model?.Name}'");

                // Validate token
                if (string.IsNullOrWhiteSpace(model?.Token) || model.Token != "CafeSecure2025")
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Token không hợp lệ. Nhận được: '{model?.Token ?? "null"}'"
                    });
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(model.Name) ||
                    string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Subject) ||
                    string.IsNullOrWhiteSpace(model.Message))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin bắt buộc" });
                }

                // Get user info
                var userInfo = await GetCurrentUserInfo();

                using var httpClient = _httpClientFactory.CreateClient();

                var contactData = new
                {
                    name = userInfo.IsLoggedIn ? userInfo.FullName : model.Name,
                    email = userInfo.IsLoggedIn ? userInfo.Email : model.Email,
                    phone = userInfo.IsLoggedIn ? userInfo.Phone : model.Phone,
                    subject = model.Subject,
                    message = model.Message,
                    isLoggedIn = userInfo.IsLoggedIn,
                    userId = userInfo.UserId,
                    userRole = userInfo.Role,
                    loginTime = userInfo.IsLoggedIn ? DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") : ""
                };

                // Send to Google Sheets
                var jsonData = System.Text.Json.JsonSerializer.Serialize(contactData);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_googleAppsScriptUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new
                    {
                        success = true,
                        message = userInfo.IsLoggedIn ?
                            $"Cảm ơn {userInfo.FullName}! Chúng tôi sẽ phản hồi sớm nhất." :
                            "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất."
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Google Apps Script Error: {errorContent}");
                    return Json(new { success = false, message = "Có lỗi xảy ra khi gửi. Vui lòng thử lại sau." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }

        private async Task<UserInfoModel> GetCurrentUserInfo()
        {
            var userInfo = new UserInfoModel();

            try
            {
                var token = HttpContext.Session.GetString("JWTToken");

                if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
                {
                    return userInfo;
                }

                using var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await httpClient.GetAsync($"{_apiUrl}User/get-current-user");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse<UserResponse>>(apiResponse);

                    if (apiResult?.IsSuccess == true && apiResult.Data != null)
                    {
                        userInfo.IsLoggedIn = true;
                        userInfo.UserId = apiResult.Data.UserId.ToString();
                        userInfo.FullName = apiResult.Data.FullName;
                        userInfo.Email = apiResult.Data.Email;
                        userInfo.Phone = apiResult.Data.Phone;
                        userInfo.Role = HttpContext.Session.GetString("UserRole") ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCurrentUserInfo Error: {ex.Message}");
            }

            return userInfo;
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
