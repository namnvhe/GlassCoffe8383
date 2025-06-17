using Cafe.BusinessObjects.Models.Request;
using Cafe.Repositories.IRepository;
using Cafe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly EmailService _emailService;

        public AuthController(IAuthRepository authRepository, EmailService emailService)
        {
            _authRepository = authRepository;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            var result = await _authRepository.LoginAsync(request.Email, request.Password);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!request.AcceptTerms)
            {
                return BadRequest(new { message = "Bạn phải đồng ý với điều khoản sử dụng" });
            }

            var result = await _authRepository.RegisterAsync(request.FullName, request.Email, request.Password, request.Phone, request.Role, _emailService);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepository.ForgotPasswordAsync(request.Email, _emailService);

            // Luôn trả về success để không tiết lộ thông tin user
            return Ok(new { message = result.Message });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authRepository.ResetPasswordAsync(request.Token, request.NewPassword, _emailService);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("validate-reset-token")]
        public IActionResult ValidateResetToken([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var isValid = _authRepository.ValidateAccessToken(token);

            return Ok(new { isValid });
        }
    }
}
