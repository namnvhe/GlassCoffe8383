using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Threading.Tasks;
using Cafe.Services;

namespace Cafe.Repositories.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDAO _authDAO;

        public AuthRepository(AuthDAO authDAO)
        {
            _authDAO = authDAO ?? throw new ArgumentNullException(nameof(authDAO));
        }

        public async Task<RegisterResponse> RegisterAsync(string fullName, string email, string password, string phone = null, string role = "Customer", EmailService emailService = null) =>
            await _authDAO.RegisterAsync(fullName, email, password, phone, role);

        public async Task<AuthResponse> LoginAsync(string email, string password, bool rememberMe = false, EmailService emailService = null) =>
            await _authDAO.LoginAsync(email, password);

        public async Task<bool> LogoutAsync(int userId) =>
            await _authDAO.LogoutAsync(userId);

        public async Task<bool> LogoutByTokenAsync(string accessToken) =>
            await _authDAO.LogoutByTokenAsync(accessToken);

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, EmailService emailService = null) =>
            await _authDAO.ChangePasswordAsync(userId, currentPassword, newPassword);

        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email, EmailService emailService = null) =>
            await _authDAO.ForgotPasswordAsync(email);

        public async Task<ResetPasswordResponse> ResetPasswordAsync(string token, string newPassword, EmailService emailService = null) =>
            await _authDAO.ResetPasswordAsync(token, newPassword);

        public async Task<bool> SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryTime) =>
            await _authDAO.SaveRefreshTokenAsync(userId, refreshToken, expiryTime);

        public async Task<AuthResponse> RefreshAccessTokenAsync(string refreshToken) =>
            await _authDAO.RefreshAccessTokenAsync(refreshToken);

        public async Task<bool> RevokeAllRefreshTokensAsync(int userId) =>
            await _authDAO.RevokeAllRefreshTokensAsync(userId);

        public async Task<bool> VerifyEmailAsync(string token) =>
            await _authDAO.VerifyEmailAsync(token);

        public async Task<bool> ResendEmailVerificationAsync(string email) =>
            await _authDAO.ResendEmailVerificationAsync(email);

        public async Task<bool> VerifyTokenAsync(string token, string tokenType) =>
            await _authDAO.VerifyTokenAsync(token, tokenType);

        public async Task<User> FindUserByRefreshTokenAsync(string refreshToken) =>
            await _authDAO.FindUserByRefreshTokenAsync(refreshToken);

        public async Task<User> GetUserFromTokenAsync(string token) =>
            await _authDAO.GetUserFromTokenAsync(token);

        public bool ValidateAccessToken(string token) =>
            _authDAO.ValidateAccessToken(token);
    }
}