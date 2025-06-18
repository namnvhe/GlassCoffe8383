using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;

        public UserRepository(UserDAO userDAO)
        {
            _userDAO = userDAO ?? throw new ArgumentNullException(nameof(userDAO));
        }

        // Admin
        public async Task<ApiResponse<List<User>>> GetAllAsync()
            => await _userDAO.GetUsersAsync();

        public async Task<User> FindUserByIdAsync(int userId)
            => await _userDAO.FindUserByIdAsync(userId);

        public async Task<User> FindUserByEmailAsync(string email)
            => await _userDAO.FindUserByEmailAsync(email);

        public async Task<ApiResponse<User>> SaveUserAsync(User user)
            => await _userDAO.SaveUserAsync(user);

        public async Task<ApiResponse<User>> UpdateUserAsync(User user)
            => await _userDAO.UpdateUserAsync(user);

        public async Task<ApiResponse<bool>> DeleteUserAsync(int userId)
            => await _userDAO.DeleteUserAsync(userId);

        public async Task<ApiResponse<List<User>>> GetUsersByRoleAsync(string role)
            => await _userDAO.GetUsersByRoleAsync(role);

        public async Task<ApiResponse<bool>> SetUserActiveStatusAsync(int userId, bool isActive)
            => await _userDAO.SetUserActiveStatusAsync(userId, isActive);

        // Customer 
        public async Task<ApiResponse<User>> GetUserAsync(int userId)
            => await _userDAO.GetCurrentUserAsync(userId);

        public async Task<ApiResponse<bool>> ChangeProfileAsync(int userId, string fullName, string phone, string? photoPath)
            => await _userDAO.UpdateProfileAsync(userId, fullName, phone, photoPath);
    }
}
