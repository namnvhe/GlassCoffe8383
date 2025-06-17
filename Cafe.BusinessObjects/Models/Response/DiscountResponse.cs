namespace Cafe.BusinessObjects.Models.Response
{
    public class DiscountResponse
    {
        public int DiscountId { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public int DaysUntilExpiry { get; set; }
    }

    public class DiscountDetailResponse
    {
        public int DiscountId { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public int DaysUntilExpiry { get; set; }
        public bool CanDelete { get; set; }
        public List<OrderSummary>? Orders { get; set; }
    }

    public class OrderSummary
    {
        public int OrderId { get; set; }
        public DateTime OrderTime { get; set; }
        public string Status { get; set; } = null!;
        public int UserId { get; set; }
    }

    public class ValidateDiscountResponse
    {
        public int DiscountId { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal Value { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public bool IsValid { get; set; }
    }

    public class DiscountStatisticsResponse
    {
        public int TotalDiscounts { get; set; }
        public int ActiveDiscounts { get; set; }
        public int ExpiredDiscounts { get; set; }
        public int ExpiringSoonDiscounts { get; set; }
        public int PercentDiscounts { get; set; }
        public int AmountDiscounts { get; set; }
        public int TotalUsage { get; set; }
        public string MostUsedDiscount { get; set; } = null!;
    }
}
