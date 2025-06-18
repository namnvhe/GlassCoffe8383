using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateSizeRequest
    {
        [Required(ErrorMessage = "Tên size là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên size không được vượt quá 50 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Giá phụ thu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phụ thu phải lớn hơn hoặc bằng 0")]
        public decimal ExtraPrice { get; set; }
    }

    public class UpdateSizeRequest
    {
        [Required(ErrorMessage = "Tên size là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên size không được vượt quá 50 ký tự")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Giá phụ thu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phụ thu phải lớn hơn hoặc bằng 0")]
        public decimal ExtraPrice { get; set; }
    }

    public class UpdateSizePriceRequest
    {
        [Required(ErrorMessage = "Giá phụ thu là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phụ thu phải lớn hơn hoặc bằng 0")]
        public decimal ExtraPrice { get; set; }
    }

    public class UpdateSizeNameRequest
    {
        [Required(ErrorMessage = "Tên size là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên size không được vượt quá 50 ký tự")]
        public string Name { get; set; } = null!;
    }
}
