using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cafe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public UserController(IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
        }

        // Lấy danh sách tất cả người dùng
        [HttpGet("get-all-users")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _userRepository.GetAllAsync();

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                var userResponses = response.Data.Select(u => new UserResponse
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    IsEmailVerified = u.IsEmailVerified,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    LastLoginAt = u.LastLoginAt
                }).ToList();

                return Ok(new ApiResponse<List<UserResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách người dùng thành công",
                    Data = userResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách người dùng"
                });
            }
        }

        // Lấy thông tin người dùng theo ID
        [HttpGet("get-user-by-id/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.FindUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                var userResponse = new UserResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return Ok(new ApiResponse<UserResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin người dùng thành công",
                    Data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin người dùng"
                });
            }
        }

        // Tìm người dùng theo email
        [HttpGet("get-user-by-email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.FindUserByEmailAsync(email);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng với email này"
                    });
                }

                var userResponse = new UserResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return Ok(new ApiResponse<UserResponse>
                {
                    IsSuccess = true,
                    Message = "Tìm người dùng thành công",
                    Data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm người dùng"
                });
            }
        }

        // Tạo người dùng mới
        [HttpPost("create-new-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra email đã tồn tại
                var existingUser = await _userRepository.FindUserByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng"
                    });
                }

                var newUser = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Phone = request.Phone,
                    PasswordHash = AuthDAO.HashPassword(request.Password), 
                    Role = request.Role,
                    IsActive = true,
                    IsEmailVerified = true, 
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _userRepository.SaveUserAsync(newUser);

                return CreatedAtAction(nameof(GetUserById), new { userId = newUser.UserId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo người dùng thành công",
                        Data = new { UserId = newUser.UserId }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo người dùng"
                });
            }
        }

        // Cập nhật thông tin người dùng
        [HttpPut("update-user-by-id/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userRepository.FindUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                // Cập nhật thông tin
                user.FullName = request.FullName;
                user.Phone = request.Phone;
                user.Role = request.Role;
                user.IsActive = request.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _userRepository.UpdateUserAsync(user);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật người dùng thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật người dùng"
                });
            }
        }

        // Xóa người dùng
        [HttpDelete("delete-user/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _userRepository.FindUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng"
                    });
                }

                // Không cho phép xóa chính mình
                var currentUserId = GetCurrentUserId();
                if (currentUserId == userId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa chính mình"
                    });
                }

                var result = await _userRepository.DeleteUserAsync(userId);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa người dùng"
                });
            }
        }

        // Lấy thông tin profile của chính mình
        [HttpGet("my-profile")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var response = await _userRepository.GetUserAsync(currentUserId);

                if (!response.IsSuccess)
                {
                    return NotFound(response);
                }

                var user = response.Data;
                var userResponse = new UserProfileResponse
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return Ok(new ApiResponse<UserProfileResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin profile thành công",
                    Data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin profile"
                });
            }
        }

        // Cập nhật profile của chính mình
        [HttpPut("change-profile")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = GetCurrentUserId();
                string? photoPath = null;

                if (request.Photo != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(request.Photo.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            IsSuccess = false,
                            Message = "Định dạng file không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png"
                        });
                    }

                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "user-photos");
                    Directory.CreateDirectory(uploadPath); // Ensure directory exists
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Photo.CopyToAsync(stream);
                    }

                    photoPath = $"/uploads/user-photos/{fileName}";
                }

                var result = await _userRepository.ChangeProfileAsync(currentUserId, request.FullName, request.Phone, photoPath);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật profile"
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}
