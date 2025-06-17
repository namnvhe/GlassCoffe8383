using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.Controllers
{
    public class DrinkTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public DrinkTypeController(HttpClient httpClient, IConfiguration configuration)
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

        // Hiển thị tất cả loại đồ uống
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-drinktype-active");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DrinkTypeResponse>>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        ViewBag.DrinkTypes = apiResponse.Data;
                        return View(apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách loại đồ uống";
            }

            return View(new List<DrinkTypeResponse>());
        }

        // Xem chi tiết loại đồ uống
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-drinktype-by-id/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<DrinkTypeDetailResponse>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải thông tin loại đồ uống";
            }

            return NotFound();
        }

        // Tìm kiếm loại đồ uống
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}DrinkType/search-drinktype-by-name?searchTerm={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DrinkTypeResponse>>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        ViewBag.SearchTerm = searchTerm;
                        return View("Index", apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra khi tìm kiếm";
            }

            return View("Index", new List<DrinkTypeResponse>());
        }
    }
}
