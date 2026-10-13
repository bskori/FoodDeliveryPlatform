using System;
using System.Collections.Generic;
using System.Text;

namespace FoodDelivery.Models.DTOs
{
    public class ItemDto
    {
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
