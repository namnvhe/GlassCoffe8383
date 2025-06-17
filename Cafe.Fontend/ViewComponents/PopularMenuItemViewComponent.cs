using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.ViewComponents
{
    public class PopularMenuItemViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public PopularMenuItemViewComponent(HttpClient httpClient, IConfiguration configuration)
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

        public async Task<IViewComponentResult> InvokeAsync(int topCount = 6)
        {

            var response = await _httpClient.GetAsync($"{_apiUrl}MenuItem/popular-item?topCount={topCount}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<PopularMenuItemResponse>>>(content);

                if (apiResponse?.IsSuccess == true && apiResponse.Data != null)
                {
                    return View(apiResponse.Data);
                }
            }

            return View(new List<PopularMenuItemResponse>());
        }
    }
}
