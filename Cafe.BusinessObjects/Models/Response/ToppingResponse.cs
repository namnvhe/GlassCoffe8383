namespace Cafe.BusinessObjects.Models.Response
{
    public class ToppingResponse
    {
        public int ToppingId { get; set; }
        public string Name { get; set; } = null!;
        public string ToppingImage { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsAvailable { get; set; }
        public string StockStatus { get; set; } = null!;
        public int OrderCount { get; set; }
    }

    public class ToppingDetailResponse
    {
        public int ToppingId { get; set; }
        public string ToppingName { get; set; } = null!;
        public string ToppingImage { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsAvailable { get; set; }
        public string StockStatus { get; set; } = null!;
        public int OrderCount { get; set; }
        public List<MenuItemSummary>? MenuItems { get; set; }
    }

    public class ToppingStatisticsResponse
    {
        public int TotalToppings { get; set; }
        public int AvailableToppings { get; set; }
        public int OutOfStockToppings { get; set; }
        public int LowStockToppings { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AveragePrice { get; set; }
        public string MostExpensiveTopping { get; set; } = null!;
        public string CheapestTopping { get; set; } = null!;
        public string MostPopularTopping { get; set; } = null!;
    }
}
