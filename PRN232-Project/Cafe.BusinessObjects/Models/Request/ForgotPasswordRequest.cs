using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.BusinessObjects.Models.Request
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = null!;
    }
}
