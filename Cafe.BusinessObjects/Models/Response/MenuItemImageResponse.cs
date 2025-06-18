namespace Cafe.BusinessObjects.Models.Response
{
    public class MenuItemImageResponse
    {
        public int ImageId { get; set; }
        public int MenuItemId { get; set; }
        public string ImagePath { get; set; } = null!;
        public string? ImageName { get; set; }
        public bool IsMainImage { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MenuItemImageDetailResponse
    {
        public int ImageId { get; set; }
        public int MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? ImageName { get; set; }
        public bool IsMainImage { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
