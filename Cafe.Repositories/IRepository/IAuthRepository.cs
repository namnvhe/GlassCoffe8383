using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafe.Services;

namespace Cafe.Repositories.IRepository
{
    public interface IAuthRepository
    {
        // Authentication methods
        Task<RegisterResponse> RegisterAsync(string fullName, string email, string password, string phone = null, string role = "Customer", EmailService emailService = null);
        Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false, EmailService emailService = null);
        Task<bool> LogoutAsync(int userId);
        Task<bool> LogoutByTokenAsync(string accessToken);

        // Password management
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, EmailService emailService = null);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(string email, EmailService emailService = null);
        Task<ResetPasswordResponse> ResetPasswordAsync(string token, string newPassword, EmailService emailService = null);

        // Token management
        Task<bool> SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime);
        Task<AuthResponse> RefreshAccessTokenAsync(string refreshToken);
        Task<bool> RevokeAllRefreshTokensAsync(int userId);

        // Email verification
        Task<bool> VerifyEmailAsync(string token);
        Task<bool> ResendEmailVerificationAsync(string email);

        // Token validation and user retrieval
        Task<bool> VerifyTokenAsync(string token, string tokenType);
        Task<User> FindUserByRefreshTokenAsync(string refreshToken);
        Task<User> GetUserFromTokenAsync(string token);

        bool ValidateAccessToken(string token);
    }
}
