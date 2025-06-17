using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateIngredientRequest
    {
        [Required(ErrorMessage = "Tên nguyên liệu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nguyên liệu không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? IngredientImage { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá đơn vị phải lớn hơn hoặc bằng 0")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateIngredientRequest
    {
        [Required(ErrorMessage = "Tên nguyên liệu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nguyên liệu không được vượt quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 255 ký tự")]
        public string? IngredientImage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá đơn vị phải lớn hơn hoặc bằng 0")]
        public decimal UnitPrice { get; set; }
    }

    public class UpdateStockRequest
    {
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        public int Quantity { get; set; }

        [StringLength(200, ErrorMessage = "Ghi chú không được vượt quá 200 ký tự")]
        public string? Note { get; set; }
    }

    public class UpdatePriceRequest
    {
        [Required(ErrorMessage = "Giá đơn vị là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá đơn vị phải lớn hơn hoặc bằng 0")]
        public decimal UnitPrice { get; set; }
    }

    public class BulkUpdateStockRequest
    {
        [Required(ErrorMessage = "Danh sách cập nhật là bắt buộc")]
        public Dictionary<int, int> IngredientQuantities { get; set; } = new Dictionary<int, int>();
    }
}
