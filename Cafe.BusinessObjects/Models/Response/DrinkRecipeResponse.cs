namespace Cafe.BusinessObjects.Models.Response
{
    public class DrinkRecipeResponse
    {
        public int RecipeId { get; set; }
        public int MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public int QuantityMinGram { get; set; }
        public int QuantityMaxGram { get; set; }
        public string QuantityRange { get; set; } = null!;
    }

    public class DrinkRecipeDetailResponse
    {
        public int RecipeId { get; set; }
        public int MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        public decimal MenuItemPrice { get; set; }
        public int IngredientId { get; set; }
        public string? IngredientName { get; set; }
        public decimal IngredientUnitPrice { get; set; }
        public int QuantityMinGram { get; set; }
        public int QuantityMaxGram { get; set; }
        public string QuantityRange { get; set; } = null!;
        public decimal EstimatedCost { get; set; }
    }

    public class RecipeOverviewStatisticsResponse
    {
        public int TotalRecipes { get; set; }
        public int TotalMenuItems { get; set; }
        public int TotalIngredients { get; set; }
        public decimal AverageIngredientsPerMenuItem { get; set; }
        public string MostUsedIngredient { get; set; } = null!;
        public string ComplexMenuItem { get; set; } = null!;
    }
}