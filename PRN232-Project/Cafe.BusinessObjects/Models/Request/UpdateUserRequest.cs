using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.BusinessObjects.Models.Request
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}
