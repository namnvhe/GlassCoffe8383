using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Cafe.BusinessObjects.Models.Response;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public UserController(HttpClient httpClient, IConfiguration configuration)
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

        public async Task<IActionResult> Index()
        {
            // Lấy token
            var token = GetTokenFromSession();

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            List<UserResponse> users = new List<UserResponse>();

            try
            {
                // Thêm Bearer token vào header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}" + "User/get-all-users");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<List<UserResponse>>>(apiResponse);

                    if (apiResult.IsSuccess)
                    {
                        users = apiResult.Data;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Token hết hạn hoặc không hợp lệ
                    ClearTokenFromSession();
                    return RedirectToAction("Login", "Auth", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải dữ liệu người dùng: " + ex.Message;
            }

            return View(users);
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
