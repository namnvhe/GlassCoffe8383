using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        public async Task<List<Discount>> GetAllAsync() =>
            await DiscountDAO.GetDiscountsAsync();

        public async Task<Discount> FindDiscountByIdAsync(int discountId) =>
            await DiscountDAO.FindDiscountByIdAsync(discountId);

        public async Task<Discount> FindDiscountByCodeAsync(string discountCode) =>
            await DiscountDAO.FindDiscountByCodeAsync(discountCode);

        public async Task SaveDiscountAsync(Discount discount) =>
            await DiscountDAO.SaveDiscountAsync(discount);

        public async Task UpdateDiscountAsync(Discount discount) =>
            await DiscountDAO.UpdateDiscountAsync(discount);

        public async Task DeleteDiscountAsync(Discount discount) =>
            await DiscountDAO.DeleteDiscountAsync(discount);

        // Tìm kiếm và lọc
        public async Task<List<Discount>> GetActiveDiscountsAsync() =>
            await DiscountDAO.GetActiveDiscountsAsync();

        public async Task<List<Discount>> GetExpiredDiscountsAsync() =>
            await DiscountDAO.GetExpiredDiscountsAsync();

        public async Task<List<Discount>> GetDiscountsByTypeAsync(string discountType) =>
            await DiscountDAO.GetDiscountsByTypeAsync(discountType);

        public async Task<List<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await DiscountDAO.GetDiscountsByDateRangeAsync(startDate, endDate);

        public async Task<List<Discount>> GetDiscountsByStatusAsync(bool isActive) =>
            await DiscountDAO.GetDiscountsByStatusAsync(isActive);

        public async Task<List<Discount>> SearchDiscountsByNameAsync(string searchTerm) =>
            await DiscountDAO.SearchDiscountsByNameAsync(searchTerm);

        public async Task<List<Discount>> GetDiscountsExpiringSoonAsync(int days = 7) =>
            await DiscountDAO.GetDiscountsExpiringSoonAsync(days);

        // Customer operations
        public async Task<Discount?> GetDiscountByCodeAsync(string code) =>
            await DiscountDAO.GetDiscountByCodeAsync(code);

        public async Task<Discount> ValidateDiscountCodeAsync(string discountCode, decimal orderAmount = 0) =>
            await DiscountDAO.ValidateDiscountCodeAsync(discountCode, orderAmount);

        // Cập nhật đặc biệt
        public async Task UpdateDiscountStatusAsync(int discountId, bool isActive) =>
            await DiscountDAO.UpdateDiscountStatusAsync(discountId, isActive);

        // Thống kê và validation
        public async Task<int> GetDiscountUsageCountAsync(int discountId) =>
            await DiscountDAO.GetDiscountUsageCountAsync(discountId);

        public async Task<bool> CanDeleteDiscountAsync(int discountId) =>
            await DiscountDAO.CanDeleteDiscountAsync(discountId);

        // Business logic methods
        public bool IsDiscountValid(Discount discount) =>
            DiscountDAO.IsDiscountValid(discount);

        public decimal ApplyDiscount(decimal originalAmount, Discount discount) =>
            DiscountDAO.ApplyDiscount(originalAmount, discount);
    }
}
