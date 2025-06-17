namespace Cafe.BusinessObjects.Models.Response
{
    public class SizeResponse
    {
        public int SizeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; }
        public int OrderCount { get; set; }
        public bool IsPopular { get; set; }
    }

    public class SizeDetailResponse
    {
        public int SizeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; }
        public int OrderCount { get; set; }
        public bool IsPopular { get; set; }
        public List<OrderItemSummary>? RecentOrders { get; set; }
    }

    public class PopularSizeResponse
    {
        public int SizeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; }
        public int TotalOrders { get; set; }
        public int PopularityRank { get; set; }
    }

    public class SizeStatisticsResponse
    {
        public int SizeId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; }
        public int TotalOrders { get; set; }
        public decimal Revenue { get; set; }
        public bool CanDelete { get; set; }
    }

    public class SizeOverviewStatisticsResponse
    {
        public int TotalSizes { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageExtraPrice { get; set; }
        public string MostPopularSize { get; set; } = null!;
        public string LeastPopularSize { get; set; } = null!;
    }

    public class OrderItemSummary
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string? MenuItemName { get; set; }
    }
}
