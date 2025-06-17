using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IDiscountRepository
    {
        Task<List<Discount>> GetAllAsync();
        Task<Discount> FindDiscountByIdAsync(int discountId);
        Task<Discount> FindDiscountByCodeAsync(string discountCode);
        Task SaveDiscountAsync(Discount discount);
        Task UpdateDiscountAsync(Discount discount);
        Task DeleteDiscountAsync(Discount discount);

        // Tìm kiếm và lọc
        Task<List<Discount>> GetActiveDiscountsAsync();
        Task<List<Discount>> GetExpiredDiscountsAsync();
        Task<List<Discount>> GetDiscountsByTypeAsync(string discountType);
        Task<List<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Discount>> GetDiscountsByStatusAsync(bool isActive);
        Task<List<Discount>> SearchDiscountsByNameAsync(string searchTerm);
        Task<List<Discount>> GetDiscountsExpiringSoonAsync(int days = 7);

        // Customer operations
        Task<Discount?> GetDiscountByCodeAsync(string code);
        Task<Discount> ValidateDiscountCodeAsync(string discountCode, decimal orderAmount = 0);

        // Cập nhật đặc biệt  
        Task UpdateDiscountStatusAsync(int discountId, bool isActive);

        // Thống kê và validation
        Task<int> GetDiscountUsageCountAsync(int discountId);
        Task<bool> CanDeleteDiscountAsync(int discountId);

        // Business logic methods
        bool IsDiscountValid(Discount discount);
        decimal ApplyDiscount(decimal originalAmount, Discount discount);
    }
}
