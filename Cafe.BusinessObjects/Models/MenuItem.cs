using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public string MenuItemImage { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int DrinkTypeId { get; set; }

    public int StockQuantity { get; set; }

    public int MinStockLevel { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<DrinkRecipe> DrinkRecipes { get; set; } = new List<DrinkRecipe>();

    public virtual DrinkType DrinkType { get; set; } = null!;

    public virtual ICollection<MenuItemImage> MenuItemImages { get; set; } = new List<MenuItemImage>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Topping> Toppings { get; set; } = new List<Topping>();
}
