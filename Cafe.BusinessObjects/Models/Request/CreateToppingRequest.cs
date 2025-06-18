using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateToppingRequest
    {
        [Required(ErrorMessage = "Tên topping là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên topping không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? ToppingImage { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0")]
        public int MinStockLevel { get; set; }

        public bool IsAvailable { get; set; } = true;
    }

    public class UpdateToppingRequest
    {
        [Required(ErrorMessage = "Tên topping là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên topping không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? ToppingImage { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0")]
        public int MinStockLevel { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class UpdateToppingAvailabilityRequest
    {
        [Required(ErrorMessage = "Trạng thái có sẵn là bắt buộc")]
        public bool IsAvailable { get; set; }
    }
}
