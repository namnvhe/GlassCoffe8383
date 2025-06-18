using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateMenuItemImageRequest
    {
        [Required(ErrorMessage = "ID món ăn là bắt buộc")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "URL hình ảnh là bắt buộc")]
        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
        public string ImageUrl { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tên hình ảnh không được vượt quá 100 ký tự")]
        public string? ImageName { get; set; }

        public bool IsMainImage { get; set; } = false;

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0")]
        public int DisplayOrder { get; set; } = 1;
    }

    public class CreateMultipleImagesRequest
    {
        [Required(ErrorMessage = "ID món ăn là bắt buộc")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Danh sách hình ảnh là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 hình ảnh")]
        public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();
    }

    public class ImageInfo
    {
        [Required(ErrorMessage = "URL hình ảnh là bắt buộc")]
        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
        public string ImageUrl { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tên hình ảnh không được vượt quá 100 ký tự")]
        public string? ImageName { get; set; }

        public bool IsMainImage { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateMenuItemImageRequest
    {
        [Required(ErrorMessage = "URL hình ảnh là bắt buộc")]
        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá 500 ký tự")]
        public string ImageUrl { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Tên hình ảnh không được vượt quá 100 ký tự")]
        public string? ImageName { get; set; }

        public bool IsMainImage { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0")]
        public int DisplayOrder { get; set; }
    }

    public class UpdateDisplayOrderRequest
    {
        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0")]
        public int DisplayOrder { get; set; }
    }

    public class ReorderImagesRequest
    {
        [Required(ErrorMessage = "Danh sách ID hình ảnh là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 hình ảnh")]
        public List<int> ImageIds { get; set; } = new List<int>();
    }
}
