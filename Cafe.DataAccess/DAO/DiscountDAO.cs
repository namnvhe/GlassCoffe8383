using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Cafe.DataAccess.DAO
{
    public class DiscountDAO
    {
        private readonly CoffeManagerContext _context;

        public DiscountDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Lấy tất cả mã giảm giá, sắp xếp từ cao xuống thấp
        public async Task<List<Discount>> GetDiscountsAsync()
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .OrderByDescending(d => d.Value)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts: {e.Message}");
            }
        }

        // Lấy các mã giảm giá đang hoạt động và chưa hết hạn
        public async Task<List<Discount>> GetActiveDiscountsAsync()
        {
            try
            {
                var today = DateTime.Today;
                return await _context.Discounts
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .Where(d => d.IsActive == true && d.ExpiryDate >= today)
                    .OrderBy(d => d.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve active discounts: {e.Message}");
            }
        }

        // Kiểm tra mã giảm giá có hợp lệ không
        public static bool IsDiscountValid(Discount discount)
        {
            var today = DateTime.Today;
            return discount.IsActive && discount.ExpiryDate >= today;
        }

        // Áp dụng giảm giá vào số tiền
        public static decimal ApplyDiscount(decimal originalAmount, Discount discount)
        {
            if (!IsDiscountValid(discount))
                return originalAmount;

            switch (discount.DiscountType.ToLower())
            {
                case "percentage":
                    return originalAmount * (1 - discount.Value / 100);
                case "fixed":
                    return Math.Max(0, originalAmount - discount.Value);
                default:
                    return originalAmount;
            }
        }

        // Lấy các mã giảm giá đã hết hạn
        public async Task<List<Discount>> GetExpiredDiscountsAsync()
        {
            try
            {
                var today = DateTime.Today;
                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Where(d => d.ExpiryDate < today)
                    .OrderByDescending(d => d.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve expired discounts: {e.Message}");
            }
        }

        // Lấy mã giảm giá theo loại (percentage/fixed)
        public async Task<List<Discount>> GetDiscountsByTypeAsync(string discountType)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .Where(d => d.DiscountType == discountType)
                    .OrderByDescending(d => d.Value)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts by type: {e.Message}");
            }
        }

        // Lấy mã giảm giá trong khoảng thời gian
        public async Task<List<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Where(d => d.ExpiryDate >= startDate && d.ExpiryDate <= endDate)
                    .OrderBy(d => d.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts by date range: {e.Message}");
            }
        }

        // Tìm mã giảm giá theo ID
        public async Task<Discount?> FindDiscountByIdAsync(int discountId)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .SingleOrDefaultAsync(x => x.DiscountId == discountId);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to find discount by ID: {e.Message}");
            }
        }

        // Lấy mã giảm giá hợp lệ theo code (dành cho khách hàng)
        public async Task<Discount?> GetDiscountByCodeAsync(string code)
        {
            try
            {
                return await _context.Discounts
                    .FirstOrDefaultAsync(d => d.Code == code && d.IsActive == true && d.ExpiryDate >= DateTime.Today);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discount by code: {e.Message}");
            }
        }

        // Tìm bất kỳ mã giảm giá nào theo code (dành cho admin)
        public async Task<Discount?> FindDiscountByCodeAsync(string discountCode)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .Include(d => d.Orders)
                    .SingleOrDefaultAsync(x => x.Code == discountCode);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to find discount by code: {e.Message}");
            }
        }

        // Xác thực mã giảm giá
        public async Task<Discount?> ValidateDiscountCodeAsync(string discountCode, decimal orderAmount = 0)
        {
            try
            {
                var today = DateTime.Today;
                var validDiscount = await _context.Discounts
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .Include(d => d.Orders)
                    .FirstOrDefaultAsync(d => d.Code == discountCode && d.IsActive == true && d.ExpiryDate >= today);

                if (validDiscount != null)
                {
                    var usageCount = await GetDiscountUsageCountAsync(validDiscount.DiscountId);
                    // Có thể thêm logic xác thực khác ở đây, ví dụ: số lần sử dụng tối đa
                }
                return validDiscount;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to validate discount code: {e.Message}");
            }
        }

        // Tìm kiếm mã giảm giá
        public async Task<List<Discount>> SearchDiscountsByNameAsync(string searchTerm)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .Where(d => (d.Description != null && d.Description.Contains(searchTerm)) || d.Code.Contains(searchTerm))
                    .OrderByDescending(d => d.Value)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to search discounts by name/code: {e.Message}");
            }
        }

        // Đếm số lần sử dụng mã giảm giá
        public async Task<int> GetDiscountUsageCountAsync(int discountId)
        {
            try
            {
                var discount = await _context.Discounts
                    .Include(d => d.Orders)
                    .FirstOrDefaultAsync(d => d.DiscountId == discountId);

                return discount?.Orders?.Count ?? 0;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get discount usage count: {e.Message}");
            }
        }

        // Lưu mã giảm giá mới
        public async Task SaveDiscountAsync(Discount discount)
        {
            try
            {
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to save discount: {e.Message}");
            }
        }

        // Cập nhật thông tin mã giảm giá
        public async Task UpdateDiscountAsync(Discount discount)
        {
            try
            {
                _context.Entry(discount).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to update discount: {e.Message}");
            }
        }

        // Cập nhật trạng thái hoạt động của mã giảm giá
        public async Task UpdateDiscountStatusAsync(int discountId, bool isActive)
        {
            try
            {
                var discount = await _context.Discounts
                    .SingleOrDefaultAsync(d => d.DiscountId == discountId);

                if (discount != null)
                {
                    discount.IsActive = isActive;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to update discount status: {e.Message}");
            }
        }

        // Xóa mã giảm giá
        public async Task DeleteDiscountAsync(Discount discount)
        {
            try
            {
                var existingDiscount = await _context.Discounts
                    .SingleOrDefaultAsync(c => c.DiscountId == discount.DiscountId);

                if (existingDiscount != null)
                {
                    _context.Discounts.Remove(existingDiscount);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to delete discount: {e.Message}");
            }
        }

        // Kiểm tra có thể xóa mã giảm giá không
        public async Task<bool> CanDeleteDiscountAsync(int discountId)
        {
            try
            {
                // Kiểm tra xem có bất kỳ đơn đặt hàng nào tham khảo giảm giá này
                var discount = await _context.Discounts
                    .Include(d => d.Orders)
                    .FirstOrDefaultAsync(d => d.DiscountId == discountId);

                bool hasOrders = discount?.Orders?.Any() == true;

                return !hasOrders;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to check if discount can be deleted: {e.Message}");
            }
        }

        // Lấy mã giảm giá theo trạng thái hoạt động
        public async Task<List<Discount>> GetDiscountsByStatusAsync(bool isActive)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Include(d => d.OrderItems)
                    .Where(d => d.IsActive == isActive)
                    .OrderBy(d => d.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts by status: {e.Message}");
            }
        }

        // Lấy mã giảm giá sắp hết hạn (mặc định 7 ngày)
        public async Task<List<Discount>> GetDiscountsExpiringSoonAsync(int days = 7)
        {
            try
            {
                var today = DateTime.Today;
                var futureDate = today.AddDays(days);

                return await _context.Discounts
                    .Include(d => d.Orders)
                    .Where(d => d.IsActive == true && d.ExpiryDate >= today && d.ExpiryDate <= futureDate)
                    .OrderBy(d => d.ExpiryDate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts expiring soon: {e.Message}");
            }
        }
    }
}