using FoodDelivery.API.Services;
using FoodDelivery.Data;
using FoodDelivery.Data.Entities;
using FoodDelivery.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(AppDbContext context, IPasswordHasher<User> passwordHasher, JwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            // 1. Prepare email for lookup
            string normalizedEmail = loginRequest.Email.Trim().ToUpperInvariant();

            // 2. Find user in sql server
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);

            // 3. Check whether the user exists
            if (user == null)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password"
                });
            }

            // 4. Check whether account is active
            if (!user.IsActive)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            // 5. Verify the password
            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash , loginRequest.Password);

            if(passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }

            // 6. Generate the token
            string token = _jwtTokenService.GenerateToken(user.UserId.ToString(), user.FullName, user.Role);

            // 7. Return the token

            return Ok(new
            {
                Message = "Login succesfull.",
                AccessToken = token,
                TokenType = "Bearer"
            });
        }
    }
}
