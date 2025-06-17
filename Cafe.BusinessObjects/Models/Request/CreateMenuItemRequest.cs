using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateMenuItemRequest
    {
        [Required(ErrorMessage = "Tên món ăn là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên món ăn không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? MenuItemImage { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "ID loại đồ uống là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID loại đồ uống phải lớn hơn 0")]
        public int DrinkTypeId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0")]
        public int MinStockLevel { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateMenuItemRequest
    {
        [Required(ErrorMessage = "Tên món ăn là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên món ăn không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? MenuItemImage { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Loại đồ uống là bắt buộc")]
        public int DrinkTypeId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0")]
        public int MinStockLevel { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class UpdateAvailabilityRequest
    {
        [Required(ErrorMessage = "Trạng thái có sẵn là bắt buộc")]
        public bool IsAvailable { get; set; }
    }

    public class UpdateMenuItemPriceRequest
    {
        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }
    }
}
