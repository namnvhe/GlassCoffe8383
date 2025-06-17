namespace Cafe.BusinessObjects.Models.Response
{
    public class MenuItemResponse
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string MenuItemImage { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int DrinkTypeId { get; set; }
        public string? DrinkTypeName { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsAvailable { get; set; }
        public string StockStatus { get; set; } = null!;
        public string? MainImage { get; set; }
        public int TotalOrders { get; set; }
        public List<ToppingSummary>? ToppingsAvailable { get; set; }
    }

    public class ToppingSummary
    {
        public int ToppingId { get; set; }
        public string ToppingName { get; set; } = null!;
        public decimal Price { get; set; }
    }

    public class MenuItemDetailResponse
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string MenuItemImage { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int DrinkTypeId { get; set; }
        public string? DrinkTypeName { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsAvailable { get; set; }
        public string StockStatus { get; set; } = null!;
        public List<MenuItemImageResponse>? Images { get; set; }
        public List<ToppingDetailResponse>? Toppings { get; set; }
        public List<DrinkRecipeDetailResponse>? DrinkRecipes { get; set; }
    }

    public class PopularMenuItemResponse
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public string MenuItemImage { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string? DrinkTypeName { get; set; }
        public int TotalOrders { get; set; }
        public string? MainImage { get; set; }
    }
}
