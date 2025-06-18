using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; } = false;
    }

    public class LockScreenRequest
    {
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; } = null!;
    }
}
