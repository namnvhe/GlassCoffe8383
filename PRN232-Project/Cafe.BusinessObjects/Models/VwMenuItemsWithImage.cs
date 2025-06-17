using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class VwMenuItemsWithImage
{
    public int MenuItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int MinStockLevel { get; set; }

    public bool IsAvailable { get; set; }

    public string? DrinkType { get; set; }

    public string? MainImageUrl { get; set; }

    public int? TotalImages { get; set; }
}
