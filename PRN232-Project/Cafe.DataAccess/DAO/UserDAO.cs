using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class UserDAO
    {
        private readonly CoffeManagerContext _context;

        public UserDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy danh sách tất cả người dùng
        public async Task<ApiResponse<List<User>>> GetUsersAsync()
        {
            try
            {
                var listUsers = await _context.Users
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        Phone = u.Phone,
                        IsActive = u.IsActive,
                        Role = u.Role,
                        IsEmailVerified = u.IsEmailVerified
                    }).ToListAsync();

                return new ApiResponse<List<User>>
                {
                    IsSuccess = true,
                    Data = listUsers,
                    Message = "Users retrieved successfully"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<User>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách người dùng",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Tìm người dùng theo ID
        public async Task<User> FindUserByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.UserId == userId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding user by ID: {e.Message}");
            }
        }

        // Tìm người dùng theo Email
        public async Task<User> FindUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(x => x.Email.Equals(email));
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding user by email: {e.Message}");
            }
        }

        // Lưu người dùng mới
        public async Task<ApiResponse<User>> SaveUserAsync(User user)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(user.Email));
                if (existingUser != null)
                {
                    return new ApiResponse<User>
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng",
                        Errors = new List<string> { "Email này đã được đăng ký trước đó" }
                    };
                }

                // Kiểm tra số điện thoại nếu có
                if (!string.IsNullOrEmpty(user.Phone))
                {
                    var existingPhone = await _context.Users.SingleOrDefaultAsync(u => u.Phone == user.Phone);
                    if (existingPhone != null)
                    {
                        return new ApiResponse<User>
                        {
                            IsSuccess = false,
                            Message = "Số điện thoại đã được sử dụng",
                            Errors = new List<string> { "Số điện thoại này đã được đăng ký trước đó" }
                        };
                    }
                }

                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new ApiResponse<User>
                {
                    IsSuccess = true,
                    Data = user,
                    Message = "Người dùng đã được tạo thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<User>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo người dùng",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Cập nhật thông tin người dùng
        public async Task<ApiResponse<User>> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    return new ApiResponse<User>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        Errors = new List<string> { "User not found" }
                    };
                }

                // Kiểm tra email trùng lặp (ngoại trừ user hiện tại)
                if (!existingUser.Email.Equals(user.Email))
                {
                    var emailExists = await _context.Users.AnyAsync(u => u.Email.Equals(user.Email) && u.UserId != user.UserId);
                    if (emailExists)
                    {
                        return new ApiResponse<User>
                        {
                            IsSuccess = false,
                            Message = "Email đã được sử dụng",
                            Errors = new List<string> { "Email này đã được sử dụng bởi người dùng khác" }
                        };
                    }
                }

                // Kiểm tra số điện thoại trùng lặp (ngoại trừ user hiện tại)
                if (!string.IsNullOrEmpty(user.Phone) && !existingUser.Phone?.Equals(user.Phone) == true)
                {
                    var phoneExists = await _context.Users.AnyAsync(u => u.Phone == user.Phone && u.UserId != user.UserId);
                    if (phoneExists)
                    {
                        return new ApiResponse<User>
                        {
                            IsSuccess = false,
                            Message = "Số điện thoại đã được sử dụng",
                            Errors = new List<string> { "Số điện thoại này đã được sử dụng bởi người dùng khác" }
                        };
                    }
                }

                user.UpdatedAt = DateTime.Now;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ApiResponse<User>
                {
                    IsSuccess = true,
                    Data = user,
                    Message = "Thông tin người dùng đã được cập nhật thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<User>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật thông tin người dùng",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Xóa người dùng
        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(c => c.UserId == userId);
                if (user == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        Errors = new List<string> { "User not found" }
                    };
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Người dùng đã được xóa thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa người dùng",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Lấy thông tin của current user
        public async Task<ApiResponse<User>> GetCurrentUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Orders) // Bao gồm danh sách đơn hàng
                    .SingleOrDefaultAsync(u => u.UserId == userId && u.IsActive);

                if (user == null)
                {
                    return new ApiResponse<User>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng hoặc tài khoản đã bị vô hiệu hóa",
                        Errors = new List<string> { "User not found or inactive" }
                    };
                }

                return new ApiResponse<User>
                {
                    IsSuccess = true,
                    Data = user,
                    Message = "Thông tin người dùng đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<User>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin người dùng",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Cập nhật thông tin cá nhân của current user
        public async Task<ApiResponse<bool>> UpdateProfileAsync(int userId, string fullName, string phone)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        Errors = new List<string> { "User not found" }
                    };
                }

                // Kiểm tra số điện thoại trùng lặp nếu có thay đổi
                if (!string.IsNullOrEmpty(phone) && !user.Phone?.Equals(phone) == true)
                {
                    var phoneExists = await _context.Users.AnyAsync(u => u.Phone == phone && u.UserId != userId);
                    if (phoneExists)
                    {
                        return new ApiResponse<bool>
                        {
                            IsSuccess = false,
                            Message = "Số điện thoại đã được sử dụng",
                            Errors = new List<string> { "Số điện thoại này đã được sử dụng bởi người dùng khác" }
                        };
                    }
                }

                user.FullName = fullName;
                user.Phone = phone;
                user.UpdatedAt = DateTime.Now;

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Thông tin cá nhân đã được cập nhật thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật thông tin cá nhân",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Lấy danh sách người dùng theo vai trò
        public async Task<ApiResponse<List<User>>> GetUsersByRoleAsync(string role)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Role.Equals(role) && u.IsActive)
                    .ToListAsync();

                return new ApiResponse<List<User>>
                {
                    IsSuccess = true,
                    Data = users,
                    Message = $"Danh sách người dùng với vai trò {role} đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<User>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách người dùng theo vai trò",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Kích hoạt/vô hiệu hóa tài khoản người dùng
        public async Task<ApiResponse<bool>> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy người dùng",
                        Errors = new List<string> { "User not found" }
                    };
                }

                user.IsActive = isActive;
                user.UpdatedAt = DateTime.Now;

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = isActive ? "Tài khoản đã được kích hoạt" : "Tài khoản đã được vô hiệu hóa"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thay đổi trạng thái tài khoản",
                    Errors = new List<string> { e.Message }
                };
            }
        }
    }
}
