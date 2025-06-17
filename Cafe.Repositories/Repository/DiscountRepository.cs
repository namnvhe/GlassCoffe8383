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
        private readonly DiscountDAO _discountDAO;

        public DiscountRepository(DiscountDAO discountDAO)
        {
            _discountDAO = discountDAO ?? throw new ArgumentNullException(nameof(discountDAO));
        }

        public async Task<List<Discount>> GetAllAsync() =>
            await _discountDAO.GetDiscountsAsync();

        public async Task<Discount> FindDiscountByIdAsync(int discountId) =>
            await _discountDAO.FindDiscountByIdAsync(discountId);

        public async Task<Discount> FindDiscountByCodeAsync(string discountCode) =>
            await _discountDAO.FindDiscountByCodeAsync(discountCode);

        public async Task SaveDiscountAsync(Discount discount) =>
            await _discountDAO.SaveDiscountAsync(discount);

        public async Task UpdateDiscountAsync(Discount discount) =>
            await _discountDAO.UpdateDiscountAsync(discount);

        public async Task DeleteDiscountAsync(Discount discount) =>
            await _discountDAO.DeleteDiscountAsync(discount);

        // Tìm kiếm và lọc
        public async Task<List<Discount>> GetActiveDiscountsAsync() =>
            await _discountDAO.GetActiveDiscountsAsync();

        public async Task<List<Discount>> GetExpiredDiscountsAsync() =>
            await _discountDAO.GetExpiredDiscountsAsync();

        public async Task<List<Discount>> GetDiscountsByTypeAsync(string discountType) =>
            await _discountDAO.GetDiscountsByTypeAsync(discountType);

        public async Task<List<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await _discountDAO.GetDiscountsByDateRangeAsync(startDate, endDate);

        public async Task<List<Discount>> GetDiscountsByStatusAsync(bool isActive) =>
            await _discountDAO.GetDiscountsByStatusAsync(isActive);

        public async Task<List<Discount>> SearchDiscountsByNameAsync(string searchTerm) =>
            await _discountDAO.SearchDiscountsByNameAsync(searchTerm);

        public async Task<List<Discount>> GetDiscountsExpiringSoonAsync(int days = 7) =>
            await _discountDAO.GetDiscountsExpiringSoonAsync(days);

        // Customer operations
        public async Task<Discount?> GetDiscountByCodeAsync(string code) =>
            await _discountDAO.GetDiscountByCodeAsync(code);

        public async Task<Discount> ValidateDiscountCodeAsync(string discountCode, decimal orderAmount = 0) =>
            await _discountDAO.ValidateDiscountCodeAsync(discountCode, orderAmount);

        // Cập nhật đặc biệt
        public async Task UpdateDiscountStatusAsync(int discountId, bool isActive) =>
            await _discountDAO.UpdateDiscountStatusAsync(discountId, isActive);

        // Thống kê và validation
        public async Task<int> GetDiscountUsageCountAsync(int discountId) =>
            await _discountDAO.GetDiscountUsageCountAsync(discountId);

        public async Task<bool> CanDeleteDiscountAsync(int discountId) =>
            await _discountDAO.CanDeleteDiscountAsync(discountId);

        // Business logic methods
        public bool IsDiscountValid(Discount discount) =>
            DiscountDAO.IsDiscountValid(discount);

        public decimal ApplyDiscount(decimal originalAmount, Discount discount) =>
            DiscountDAO.ApplyDiscount(originalAmount, discount);
    }
}
