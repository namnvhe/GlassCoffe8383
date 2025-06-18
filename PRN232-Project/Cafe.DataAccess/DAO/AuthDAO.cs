using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using Cafe.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class AuthDAO
    {
        private readonly CoffeManagerContext _context;
        private readonly EmailService _emailService;

        public AuthDAO(CoffeManagerContext context, EmailService emailService = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _emailService = emailService;
        }

        // Method hỗ trợ xóa tài khoản không active sau 7 ngày
        private async Task CleanupInactiveAccountsAsync(string email)
        {
            try
            {
                var sevenDaysAgo = DateTime.Now.AddDays(-7);

                // Tìm tài khoản cùng email, chưa đăng nhập lần nào và đã tạo quá 7 ngày
                var inactiveUser = await _context.Users
                    .SingleOrDefaultAsync(u => u.Email.Equals(email) &&
                                              u.LastLoginAt == null &&
                                              u.CreatedAt <= sevenDaysAgo);

                if (inactiveUser != null)
                {
                    _context.Users.Remove(inactiveUser);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Removed inactive account for email: {email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up inactive accounts: {ex.Message}");
            }
        }

        // Đăng ký tài khoản mới
        public async Task<RegisterResponse> RegisterAsync(string fullName, string email, string password, string phone = null, string role = "Customer")
        {
            try
            {
                // Kiểm tra và xóa tài khoản cũ nếu đã quá 7 ngày không đăng nhập
                await CleanupInactiveAccountsAsync(email);

                // Kiểm tra email đã tồn tại
                var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));
                if (existingUser != null)
                {
                    return new RegisterResponse
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng",
                        Errors = new List<string> { "Email này đã được đăng ký trước đó" }
                    };
                }

                if (!string.IsNullOrEmpty(phone))
                {
                    var existingPhone = await _context.Users.SingleOrDefaultAsync(u => u.Phone == phone);
                    if (existingPhone != null)
                    {
                        return new RegisterResponse
                        {
                            IsSuccess = false,
                            Message = "Số điện thoại đã được sử dụng",
                            Errors = new List<string> { "Số điện thoại này đã được đăng ký trước đó" }
                        };
                    }
                }

                if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                {
                    return new RegisterResponse
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu không hợp lệ",
                        Errors = new List<string> { "Mật khẩu phải có ít nhất 8 ký tự" }
                    };
                }

                string tempPassword = password;
                var emailVerificationToken = GenerateRandomToken();

                // Tạo user mới với IsActive = false (chưa đăng nhập lần đầu)
                var newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    PasswordHash = HashPassword(password),
                    Role = role ?? "Customer",
                    IsActive = false, // Chưa active cho đến khi đăng nhập lần đầu
                    IsEmailVerified = false,
                    EmailVerificationToken = emailVerificationToken,
                    EmailVerificationTokenExpiry = DateTime.Now.AddDays(7), // 7 ngày để đăng nhập lần đầu
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    LastLoginAt = null // Chưa đăng nhập lần nào
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Gửi email chào mừng với thông tin 7 ngày
                if (_emailService != null)
                {
                    try
                    {
                        await _emailService.SendWelcomeEmail(email, fullName, tempPassword);
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Failed to send welcome email: {emailEx.Message}");
                    }
                }

                return new RegisterResponse
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công! Vui lòng đăng nhập trong vòng 7 ngày để kích hoạt tài khoản.",
                    User = new UserInfo
                    {
                        UserId = newUser.UserId,
                        FullName = newUser.FullName,
                        Email = newUser.Email,
                        Role = newUser.Role,
                        IsEmailVerified = newUser.IsEmailVerified
                    },
                    EmailVerificationToken = emailVerificationToken
                };
            }
            catch (Exception e)
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = "Đăng ký thất bại",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Đăng nhập
        public async Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false)
        {
            try
            {
                // Tìm user (bao gồm cả chưa active)
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));

                if (user == null)
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Tài khoản của bạn không có trong hệ thống. Vui lòng đăng ký tài khoản."
                    };
                }

                // Kiểm tra nếu tài khoản chưa đăng nhập lần nào và đã quá 7 ngày
                if (user.LastLoginAt == null)
                {
                    var sevenDaysAgo = DateTime.Now.AddDays(-7);
                    if (user.CreatedAt <= sevenDaysAgo)
                    {
                        // Xóa tài khoản đã hết hạn
                        _context.Users.Remove(user);
                        await _context.SaveChangesAsync();

                        return new AuthResponse
                        {
                            IsSuccess = false,
                            Message = "Tài khoản đã hết hạn kích hoạt. Vui lòng đăng ký lại."
                        };
                    }
                }

                // Kiểm tra mật khẩu
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        IsSuccess = false,
                        Message = "Email hoặc mật khẩu không đúng"
                    };
                }

                // Kích hoạt tài khoản nếu đây là lần đăng nhập đầu tiên
                bool isFirstLogin = user.LastLoginAt == null;
                if (isFirstLogin)
                {
                    user.IsActive = true; // Kích hoạt tài khoản
                    user.IsEmailVerified = true; // Xác thực email tự động khi đăng nhập lần đầu
                    user.EmailVerificationToken = null;
                    user.EmailVerificationTokenExpiry = null;
                }

                // Tạo JWT tokens
                var accessToken = JwtToken.GenerateAccessToken(user);
                var refreshToken = JwtToken.GenerateRefreshToken();
                var refreshTokenExpiry = rememberMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(7);

                // Cập nhật thông tin đăng nhập
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = refreshTokenExpiry;
                user.LastLoginAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Gửi email chào mừng nếu là lần đăng nhập đầu tiên
                if (isFirstLogin && _emailService != null)
                {
                    try
                    {
                        await _emailService.SendFirstLoginSuccessEmail(user.Email, user.FullName);
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"Failed to send first login email: {emailEx.Message}");
                    }
                }

                return new AuthResponse
                {
                    IsSuccess = true,
                    Message = isFirstLogin ? "Chào mừng! Tài khoản đã được kích hoạt thành công." : "Đăng nhập thành công",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = JwtToken.GetTokenExpiryTime(accessToken),
                    User = new UserInfo
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role,
                        IsEmailVerified = user.IsEmailVerified
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Login error: {e.Message} - {e.StackTrace}");
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = $"Đăng nhập thất bại: {e.Message}"
                };
            }
        }

        // Đổi mật khẩu
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

                if (user != null && VerifyPassword(currentPassword, user.PasswordHash))
                {
                    user.PasswordHash = HashPassword(newPassword);
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Gửi email xác nhận đổi mật khẩu
                    if (_emailService != null)
                    {
                        try
                        {
                            await _emailService.SendPasswordChangeConfirmationEmail(user.Email, user.FullName);
                        }
                        catch (Exception emailEx)
                        {
                            Console.WriteLine($"Failed to send password change confirmation email: {emailEx.Message}");
                        }
                    }

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Quên mật khẩu - Tạo token reset
        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email) && u.IsActive == true);

                if (user != null)
                {
                    var resetToken = GenerateRandomToken();
                    var tokenExpiry = DateTime.Now.AddHours(1);

                    user.PasswordResetToken = resetToken;
                    user.PasswordResetTokenExpiry = tokenExpiry;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Tạo reset link
                    string resetLink = $"http://localhost:3000/reset-password?token={resetToken}";

                    // Gửi email reset password
                    if (_emailService != null)
                    {
                        try
                        {
                            await _emailService.SendPasswordResetEmail(email, resetLink);
                        }
                        catch (Exception emailEx)
                        {
                            Console.WriteLine($"Failed to send password reset email: {emailEx.Message}");
                            return new ForgotPasswordResponse
                            {
                                IsSuccess = false,
                                Message = "Có lỗi xảy ra khi gửi email. Vui lòng thử lại sau.",
                                Errors = new List<string> { "Email service error" }
                            };
                        }
                    }

                    return new ForgotPasswordResponse
                    {
                        IsSuccess = true,
                        Message = "Link đặt lại mật khẩu đã được gửi đến email của bạn. Vui lòng kiểm tra hộp thư.",
                        ResetToken = resetToken, // Chỉ để test, production không nên trả về
                        TokenExpiryTime = tokenExpiry
                    };
                }

                return new ForgotPasswordResponse
                {
                    IsSuccess = true,
                    Message = "Nếu email này tồn tại trong hệ thống, bạn sẽ nhận được link đặt lại mật khẩu trong vòng vài phút."
                };
            }
            catch (Exception e)
            {
                return new ForgotPasswordResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra. Vui lòng thử lại sau.",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Reset mật khẩu với token
        public async Task<ResetPasswordResponse> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u =>
                    u.PasswordResetToken.Equals(token) &&
                    u.PasswordResetTokenExpiry > DateTime.Now &&
                    u.IsActive == true);

                if (user != null)
                {
                    user.PasswordHash = HashPassword(newPassword);
                    user.PasswordResetToken = null;
                    user.PasswordResetTokenExpiry = null;
                    user.RefreshToken = null; // Revoke tất cả refresh tokens
                    user.RefreshTokenExpiryTime = null;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Gửi email xác nhận reset password thành công
                    if (_emailService != null)
                    {
                        try
                        {
                            await _emailService.SendPasswordResetSuccessEmail(user.Email, user.FullName);
                        }
                        catch (Exception emailEx)
                        {
                            Console.WriteLine($"Failed to send password reset success email: {emailEx.Message}");
                        }
                    }

                    return new ResetPasswordResponse
                    {
                        IsSuccess = true,
                        Message = "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập với mật khẩu mới."
                    };
                }

                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "Token không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu đặt lại mật khẩu mới.",
                    Errors = new List<string> { "Invalid or expired token" }
                };
            }
            catch (Exception e)
            {
                return new ResetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi đặt lại mật khẩu. Vui lòng thử lại.",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        // Lưu Refresh Token
        public async Task<bool> SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

                if (user != null)
                {
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = expiryTime;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Refresh Token - Tạo token mới
        public async Task<AuthResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u =>
                    u.RefreshToken.Equals(refreshToken) &&
                    u.RefreshTokenExpiryTime > DateTime.Now &&
                    u.IsActive == true);

                if (user != null)
                {
                    // Tạo access token mới
                    var newAccessToken = JwtToken.GenerateAccessToken(user);
                    var newRefreshToken = JwtToken.GenerateRefreshToken();
                    var newRefreshTokenExpiry = DateTime.Now.AddDays(7);

                    // Cập nhật refresh token mới
                    user.RefreshToken = newRefreshToken;
                    user.RefreshTokenExpiryTime = newRefreshTokenExpiry;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return new AuthResponse
                    {
                        IsSuccess = true,
                        Message = "Token refreshed successfully",
                        AccessToken = newAccessToken,
                        RefreshToken = newRefreshToken,
                        ExpiresAt = JwtToken.GetTokenExpiryTime(newAccessToken),
                        User = new UserInfo
                        {
                            UserId = user.UserId,
                            FullName = user.FullName,
                            Email = user.Email,
                            Role = user.Role,
                            IsEmailVerified = user.IsEmailVerified
                        }
                    };
                }

                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Invalid or expired refresh token"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Token refresh error: {e.Message} - {e.StackTrace}");
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = $"Token refresh failed: {e.Message}"
                };
            }
        }

        // Đăng xuất - Xóa refresh token
        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Logout bằng JWT token
        public async Task<bool> LogoutByTokenAsync(string accessToken)
        {
            try
            {
                var userId = JwtToken.GetUserIdFromToken(accessToken);
                if (userId > 0)
                {
                    return await LogoutAsync(userId);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Xác thực email
        public async Task<bool> VerifyEmailAsync(string token)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u =>
                    u.EmailVerificationToken.Equals(token) &&
                    u.EmailVerificationTokenExpiry > DateTime.Now);

                if (user != null)
                {
                    user.IsEmailVerified = true;
                    user.EmailVerificationToken = null;
                    user.EmailVerificationTokenExpiry = null;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Gửi lại email xác thực
        public async Task<bool> ResendEmailVerificationAsync(string email)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email) && !u.IsEmailVerified);

                if (user != null)
                {
                    user.EmailVerificationToken = GenerateRandomToken();
                    user.EmailVerificationTokenExpiry = DateTime.Now.AddHours(24);
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Kiểm tra token có hợp lệ không
        public async Task<bool> VerifyTokenAsync(string token, string tokenType)
        {
            try
            {
                User user = null;

                switch (tokenType.ToLower())
                {
                    case "access":
                    case "jwt":
                        // Validate JWT access token
                        return ValidateAccessToken(token);

                    case "reset":
                        user = await _context.Users.SingleOrDefaultAsync(u =>
                            u.PasswordResetToken.Equals(token) &&
                            u.PasswordResetTokenExpiry > DateTime.Now);
                        break;

                    case "email":
                        user = await _context.Users.SingleOrDefaultAsync(u =>
                            u.EmailVerificationToken.Equals(token) &&
                            u.EmailVerificationTokenExpiry > DateTime.Now);
                        break;

                    case "refresh":
                        user = await _context.Users.SingleOrDefaultAsync(u =>
                            u.RefreshToken.Equals(token) &&
                            u.RefreshTokenExpiryTime > DateTime.Now);
                        break;
                }

                return user != null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Tìm user theo refresh token
        public async Task<User> FindUserByRefreshTokenAsync(string refreshToken)
        {
            try
            {
                return await _context.Users.SingleOrDefaultAsync(u =>
                    u.RefreshToken.Equals(refreshToken) &&
                    u.RefreshTokenExpiryTime > DateTime.Now &&
                    u.IsActive == true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Method để validate JWT token
        public bool ValidateAccessToken(string token)
        {
            try
            {
                var principal = JwtToken.ValidateAccessToken(token);
                return principal != null;
            }
            catch
            {
                return false;
            }
        }

        // Method để lấy User từ JWT token
        public async Task<User> GetUserFromTokenAsync(string token)
        {
            try
            {
                var userId = JwtToken.GetUserIdFromToken(token);
                if (userId > 0)
                {
                    return await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.IsActive == true);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // Vô hiệu hóa tất cả refresh token của user
        public async Task<bool> RevokeAllRefreshTokensAsync(int userId)
        {
            try
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    user.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Hash password sử dụng SHA256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + "SALT_KEY"));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Xác thực password
        private static bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hash);
        }

        // Tạo random token
        private static string GenerateRandomToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }
    }
}