using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDelivery.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected int? CurrentUserId
        {
            get
            {
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(int.TryParse(userId, out int id))
                {
                    return id;
                }

                return null;
            }
        }

        protected string? CurrentUserEmail
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.Email);
            }
        }

        protected string? CurrentUserFullName 
        {
            get
            {
                return User.FindFirst("FullName")?.Value;
            }
        }

        protected string? CurrentUserRole
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.Role);
            }
        }

        protected bool IsUserLoggedIn
        {
            get
            {
                return User.Identity?.IsAuthenticated == true;
            }
        }
    }
}
