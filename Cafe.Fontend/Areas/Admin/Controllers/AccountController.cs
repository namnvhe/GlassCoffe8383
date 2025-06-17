using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    public class AccountController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public AccountController(HttpClient httpClient, IConfiguration configuration)
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

        public async Task<IActionResult> MyProfile()
        {
            // Lấy token từ session
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            UserProfileResponse userProfile = null;
            try
            {
                // Thêm Bearer token vào header
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}User/my-profile");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<UserProfileResponse>>(apiResponse);
                    if (apiResult.IsSuccess)
                    {
                        userProfile = apiResult.Data;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Token hết hạn hoặc không hợp lệ
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToAction("Login", "Auth", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải thông tin hồ sơ: " + ex.Message;
            }

            return View(userProfile ?? new UserProfileResponse());
        }

        public async Task<IActionResult> UpdateProfile()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            UserProfileResponse userProfile = null;
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUrl}User/my-profile");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<UserProfileResponse>>(apiResponse);
                    if (apiResult.IsSuccess)
                    {
                        userProfile = apiResult.Data;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToAction("Login", "Auth", new { area = "Admin" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải thông tin hồ sơ: " + ex.Message;
            }

            var model = new UpdateProfileRequest
            {
                FullName = userProfile?.FullName ?? "",
                Phone = userProfile?.Phone
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest model, IFormFile? Photo)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = HttpContext.Session.GetString("JWTToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.FullName), "FullName");
                if (!string.IsNullOrEmpty(model.Phone))
                {
                    formData.Add(new StringContent(model.Phone), "Phone");
                }
                if (Photo != null)
                {
                    var streamContent = new StreamContent(Photo.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(Photo.ContentType);
                    formData.Add(streamContent, "Photo", Photo.FileName);
                }

                HttpResponseMessage response = await _httpClient.PutAsync($"{_apiUrl}User/change-profile", formData);

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonConvert.DeserializeObject<ApiResponse<bool>>(apiResponse);
                    if (apiResult.IsSuccess)
                    {
                        ViewBag.SuccessMessage = "Cập nhật hồ sơ thành công!";
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, apiResult.Message);
                        return View(model);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToAction("Login", "Auth", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật hồ sơ.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }
    }
}
