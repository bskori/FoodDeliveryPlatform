using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDelivery.Models.Responses
{
    public class LoginResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
    }
}
