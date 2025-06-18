using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class ValidateDiscountRequest
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã giảm giá không được vượt quá 20 ký tự")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Số tiền đơn hàng là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền đơn hàng phải lớn hơn hoặc bằng 0")]
        public decimal OrderAmount { get; set; }
    }

    public class CreateDiscountRequest
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã giảm giá không được vượt quá 20 ký tự")]
        public string Code { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [RegularExpression("^(Amount|Percent)$", ErrorMessage = "Loại giảm giá chỉ có thể là 'Amount' hoặc 'Percent'")]
        public string DiscountType { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateDiscountRequest
    {
        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã giảm giá không được vượt quá 20 ký tự")]
        public string Code { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        [RegularExpression("^(Amount|Percent)$", ErrorMessage = "Loại giảm giá chỉ có thể là 'Amount' hoặc 'Percent'")]
        public string DiscountType { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }
    }

    public class UpdateDiscountStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        public bool IsActive { get; set; }
    }
}
