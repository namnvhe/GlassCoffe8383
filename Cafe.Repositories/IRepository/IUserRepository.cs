using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IUserRepository
    {
        // Admin
        Task<ApiResponse<List<User>>> GetAllAsync();
        Task<User> FindUserByIdAsync(int userId);
        Task<User> FindUserByEmailAsync(string email);
        Task<ApiResponse<User>> SaveUserAsync(User user);
        Task<ApiResponse<User>> UpdateUserAsync(User user);
        Task<ApiResponse<bool>> DeleteUserAsync(int userId);
        Task<ApiResponse<List<User>>> GetUsersByRoleAsync(string role);
        Task<ApiResponse<bool>> SetUserActiveStatusAsync(int userId, bool isActive);

        // Customer
        Task<ApiResponse<User>> GetUserAsync(int userId);
        Task<ApiResponse<bool>> ChangeProfileAsync(int userId, string fullName, string phone, string? photoPath);
    }
}
