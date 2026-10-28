using FoodDelivery.Models.DTOs;
using FoodDelivery.Models.Responses;
using FoodDelivery.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace FoodDelivery.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            List<ItemDto> items = new List<ItemDto>();

            try
            {
                HttpClient client = _httpClientFactory.CreateClient("FoodDeliveryAPI");

                HttpResponseMessage response = await client.GetAsync("api/items");

                if (response.IsSuccessStatusCode)
                {

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    JsonSerializerOptions jsonOptions = new JsonSerializerOptions();

                    jsonOptions.PropertyNameCaseInsensitive = true;

                    ApiResponse<List<ItemDto>>? apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ItemDto>>>(jsonResponse, jsonOptions);
                    
                    if(apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                    {
                        items = apiResponse.Data;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Food items could not be loaded.";
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "Food items could not be loaded.";
                }
            }
            catch(HttpRequestException exception)
            {
                _logger.LogError(exception, "Error while reqesting food items.");

                ViewBag.ErrorMessage = "Unable to connect to the Food Delivery API";
            }
            catch(JsonException exception)
            {
                _logger.LogError(exception, "Error while reading the food items from response.");

                ViewBag.ErrorMessage = "Unable to read the food items.";
            }

            return View(items);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
