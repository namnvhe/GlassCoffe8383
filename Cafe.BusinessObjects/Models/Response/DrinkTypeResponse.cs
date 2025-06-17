namespace Cafe.BusinessObjects.Models.Response
{
    public class DrinkTypeResponse
    {
        public int DrinkTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public int MenuItemCount { get; set; }
        public List<MenuItemSummary>? MenuItems { get; set; }
    }

    public class MenuItemSummary
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; } = null!;
    }

    public class PopularDrinkTypeResponse
    {
        public int DrinkTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public int TotalOrders { get; set; }
        public int MenuItemCount { get; set; }
    }

    public class DrinkTypeDetailResponse
    {
        public int DrinkTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public List<MenuItemDetailResponse>? MenuItems { get; set; }
    }

    public class DrinkTypeStatsResponse
    {
        public int DrinkTypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public int MenuItemCount { get; set; }
        public int TotalOrders { get; set; }
        public bool CanDelete { get; set; }
    }
}
