using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public MenuItemController(HttpClient httpClient, IConfiguration configuration)
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

        // Hiển thị menu đồ uống
        public async Task<IActionResult> Index(int? drinkTypeId)
        {
            try
            {
                // Lấy danh sách loại đồ uống cho filter
                var drinkTypesResponse = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-drinktype-active");
                if (drinkTypesResponse.IsSuccessStatusCode)
                {
                    var drinkTypesContent = await drinkTypesResponse.Content.ReadAsStringAsync();
                    var drinkTypesApiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DrinkTypeResponse>>>(drinkTypesContent);
                    ViewBag.DrinkTypes = drinkTypesApiResponse?.Data ?? new List<DrinkTypeResponse>();
                }

                // Lấy danh sách menu items
                string endpoint = drinkTypeId.HasValue
                    ? $"MenuItem/get-menu-item-by-drink-type/{drinkTypeId}"
                    : "MenuItem/get-available-menu-item";

                var response = await _httpClient.GetAsync($"{_apiUrl}{endpoint}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<MenuItemResponse>>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        ViewBag.SelectedDrinkTypeId = drinkTypeId;
                        return View(apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách menu";
            }

            return View(new List<MenuItemResponse>());
        }

        // Xem chi tiết món
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}MenuItem/get-menu-item-by-id/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<MenuItemDetailResponse>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải thông tin món";
            }

            return NotFound();
        }

        // Tìm kiếm món
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}MenuItem/search?searchTerm={Uri.EscapeDataString(searchTerm)}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<MenuItemResponse>>>(content);

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

            return View("Index", new List<MenuItemResponse>());
        }

        // Lọc theo khoảng giá
        public async Task<IActionResult> FilterByPrice(decimal minPrice, decimal maxPrice)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}MenuItem/get-menu-item-by-price-range?minPrice={minPrice}&maxPrice={maxPrice}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<MenuItemResponse>>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        ViewBag.MinPrice = minPrice;
                        ViewBag.MaxPrice = maxPrice;
                        return View("Index", apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Có lỗi xảy ra khi lọc theo giá";
            }

            return View("Index", new List<MenuItemResponse>());
        }

        // Món phổ biến
        public async Task<IActionResult> Popular()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}MenuItem/popular-item?topCount=12");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<PopularMenuItemResponse>>>(content);

                    if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                    {
                        return View(apiResponse.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không thể tải danh sách món phổ biến";
            }

            return View(new List<PopularMenuItemResponse>());
        }
    }
}
