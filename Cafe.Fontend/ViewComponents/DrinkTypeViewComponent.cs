using Cafe.BusinessObjects.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Cafe.Fontend.ViewComponents
{
    public class DrinkTypeViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public DrinkTypeViewComponent(HttpClient httpClient, IConfiguration configuration)
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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allDrinkTypesResponse = await _httpClient.GetAsync($"{_apiUrl}DrinkType/get-all-drinktypes");

            if (allDrinkTypesResponse.IsSuccessStatusCode)
            {
                var content = await allDrinkTypesResponse.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DrinkTypeResponse>>>(content);

                if (apiResponse != null && apiResponse.IsSuccess && apiResponse.Data != null)
                {
                    return View(apiResponse.Data);
                }
            }

            return View(new List<DrinkTypeResponse>());
        }

    }
}
