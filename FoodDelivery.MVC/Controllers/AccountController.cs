using FoodDelivery.Models.Requests;
using FoodDelivery.Models.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FoodDelivery.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public AccountController(ILogger<AccountController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginRequestDto());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequest);
            }

            try
            {   
                //Create our configured API
                HttpClient client = _httpClientFactory.CreateClient("FoodDeliveryAPI");

                //Send login details to the Web API
                HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", loginRequest);

                //Check whether credentials were accepted
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(loginRequest);
                }

                //Convert the API Json into C# object
                LoginResponseDto? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if(loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.AccessToken))
                {
                    ModelState.AddModelError("", "Login response was invalid.");

                    return View(loginRequest);
                }

                //Store JWT on the MVC server in Session
                HttpContext.Session.SetString("AccessToken", loginResponse.AccessToken);

                //Create an MVC identity
                List<Claim> claims = new List<Claim>();

                claims.Add(new Claim(
                    ClaimTypes.Name,
                    ClaimTypes.Email.Trim()));

                ClaimsIdentity identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                //Create the authentication cookie
                AuthenticationProperties properties = new AuthenticationProperties();

                properties.IsPersistent = false;
                properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(20);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, properties);

                return RedirectToAction("Dashboard", "Account");
            }
            catch(HttpRequestException exception)
            {
                _logger.LogError(exception, "Login API connection failed");

                ModelState.AddModelError("", "Unable to connect to the login service.");
            }
            catch(JsonException exception)
            {
                _logger.LogError(exception, "Login API returned invalid JSON.");

                ModelState.AddModelError("", "Unable to read the login response.");
            }

            return View(loginRequest);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            string? token = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return View("SessionExpired");
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        
    }
}
