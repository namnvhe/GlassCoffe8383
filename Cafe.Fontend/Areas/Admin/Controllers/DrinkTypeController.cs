using Cafe.BusinessObjects.Models.Response;
using Cafe.Fontend.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Cafe.Fontend.Areas.Admin.Controllers
{
    public class DrinkTypeController : BaseController
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

        public async Task<IActionResult> Index()
        {
            try
            {
                var token = GetTokenFromSession();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // Lấy tất cả loại đồ uống
                var allDrinkTypesResponse = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-all-drinktypes");
                var popularDrinkTypesResponse = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-popular-drinktype?topCount=5");

                var viewModel = new DrinkTypeIndexViewModel();

                if (allDrinkTypesResponse.IsSuccessStatusCode)
                {
                    var content = await allDrinkTypesResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DrinkTypeResponse>>>(content);
                    viewModel.DrinkTypes = apiResponse?.Data ?? new List<DrinkTypeResponse>();
                }

                if (popularDrinkTypesResponse.IsSuccessStatusCode)
                {
                    var content = await popularDrinkTypesResponse.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<PopularDrinkTypeResponse>>>(content);
                    viewModel.PopularDrinkTypes = apiResponse?.Data ?? new List<PopularDrinkTypeResponse>();
                }

                // Tính toán thống kê
                viewModel.TotalDrinkTypes = viewModel.DrinkTypes.Count;
                viewModel.ActiveDrinkTypes = viewModel.DrinkTypes.Count(dt => dt.IsActive);
                viewModel.InactiveDrinkTypes = viewModel.TotalDrinkTypes - viewModel.ActiveDrinkTypes;
                viewModel.TotalMenuItems = viewModel.DrinkTypes.Sum(dt => dt.MenuItemCount);
                viewModel.AverageMenuItemsPerType = viewModel.TotalDrinkTypes > 0 ?
                    Math.Round((double)viewModel.TotalMenuItems / viewModel.TotalDrinkTypes, 1) : 0;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dữ liệu: " + ex.Message;
                return View(new DrinkTypeIndexViewModel());
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStatus(int drinkTypeId, bool isActive)
        {
            try
            {
                var token = GetTokenFromSession();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var request = new { IsActive = isActive };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiUrl}DrinkType/update-drinktype-status/{drinkTypeId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = isActive ? "Đã kích hoạt loại đồ uống" : "Đã vô hiệu hóa loại đồ uống";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int drinkTypeId)
        {
            try
            {
                var token = GetTokenFromSession();
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.DeleteAsync($"{_apiUrl}DrinkType/delete-drinktype/{drinkTypeId}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đã xóa loại đồ uống thành công";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(errorContent);
                    TempData["ErrorMessage"] = errorResponse?.Message ?? "Có lỗi xảy ra khi xóa loại đồ uống";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index");
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
