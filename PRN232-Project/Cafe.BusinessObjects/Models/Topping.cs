using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class Topping
{
    public int ToppingId { get; set; }

    public string Name { get; set; } = null!;

    public string ToppingImage { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int MinStockLevel { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<OrderItemTopping> OrderItemToppings { get; set; } = new List<OrderItemTopping>();

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
