using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateDrinkTypeRequest
    {
        [Required(ErrorMessage = "Tên loại đồ uống là bắt buộc")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên loại đồ uống phải từ 1 đến 100 ký tự")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên loại đồ uống chỉ được chứa chữ cái, số và khoảng trắng")]
        public string TypeName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
        public string? ImagePath { get; set; }
    }

    public class UpdateDrinkTypeRequest
    {
        [Required(ErrorMessage = "Tên loại đồ uống là bắt buộc")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên loại đồ uống phải từ 1 đến 100 ký tự")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên loại đồ uống chỉ được chứa chữ cái, số và khoảng trắng")]
        public string TypeName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
        public string? ImagePath { get; set; }
    }

    public class UpdateStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái hoạt động là bắt buộc")]
        public bool IsActive { get; set; }
    }

    public class UpdateDrinkTypeImageRequest
    {
        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
        public string? ImagePath { get; set; }
    }
}
