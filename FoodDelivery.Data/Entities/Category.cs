using System;
using System.Collections.Generic;

namespace FoodDelivery.Data.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
