namespace Cafe.BusinessObjects.Models.Response
{
    public class IngredientResponse
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public string IngredientImage { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string StockStatus { get; set; } = null!;
        public decimal TotalValue { get; set; }
        public int RecipeCount { get; set; }
    }

    public class IngredientDetailResponse
    {
        public int IngredientId { get; set; }
        public string Name { get; set; } = null!;
        public string IngredientImage { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string StockStatus { get; set; } = null!;
        public decimal TotalValue { get; set; }
        public List<DrinkRecipeResponse>? DrinkRecipes { get; set; }
    }

    public class InventoryStatsResponse
    {
        public int TotalIngredients { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int AvailableCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}
