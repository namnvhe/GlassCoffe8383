using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class DiscountDAO
    {
        public static async Task<List<Discount>> GetDiscountsAsync()
        {
            var listDiscounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listDiscounts = await context.Discounts
                        .Include(d => d.Orders)
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .OrderByDescending(d => d.Value)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listDiscounts;
        }

        public static async Task<List<Discount>> GetActiveDiscountsAsync()
        {
            var activeDiscounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    activeDiscounts = await context.Discounts
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Where(d => d.IsActive == true && d.ExpiryDate >= today)
                        .OrderBy(d => d.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return activeDiscounts;
        }

        public static bool IsDiscountValid(Discount discount)
        {
            var today = DateTime.Today;
            return discount.IsActive && discount.ExpiryDate >= today;
        }

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

        public static async Task<List<Discount>> GetExpiredDiscountsAsync()
        {
            var expiredDiscounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    expiredDiscounts = await context.Discounts
                        .Include(d => d.Orders)
                        .Where(d => d.ExpiryDate < today)
                        .OrderByDescending(d => d.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return expiredDiscounts;
        }

        public static async Task<List<Discount>> GetDiscountsByTypeAsync(string discountType)
        {
            var discounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discounts = await context.Discounts
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Where(d => d.DiscountType == discountType)
                        .OrderByDescending(d => d.Value) // Changed from DiscountType to Value for better sorting
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return discounts;
        }

        public static async Task<List<Discount>> GetDiscountsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var discounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discounts = await context.Discounts
                        .Include(d => d.Orders)
                        .Where(d => d.ExpiryDate >= startDate && d.ExpiryDate <= endDate)
                        .OrderBy(d => d.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return discounts;
        }

        public static async Task<Discount> FindDiscountByIdAsync(int discountId)
        {
            Discount discount = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discount = await context.Discounts
                        .Include(d => d.Orders)
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .SingleOrDefaultAsync(x => x.DiscountId == discountId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return discount;
        }

        // Customer - Get valid discount by code
        public static async Task<Discount?> GetDiscountByCodeAsync(string code)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    return await context.Discounts
                        .FirstOrDefaultAsync(d => d.Code == code &&
                                           d.IsActive == true &&
                                           d.ExpiryDate >= DateTime.Today);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discount by code: {e.Message}");
            }
        }

        // Admin - Find any discount by code
        public static async Task<Discount> FindDiscountByCodeAsync(string discountCode)
        {
            Discount discount = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discount = await context.Discounts
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Include(d => d.Orders)
                        .SingleOrDefaultAsync(x => x.Code == discountCode);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return discount;
        }

        public static async Task<Discount> ValidateDiscountCodeAsync(string discountCode, decimal orderAmount = 0)
        {
            Discount validDiscount = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    validDiscount = await context.Discounts
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Include(d => d.Orders)
                        .FirstOrDefaultAsync(d => d.Code == discountCode &&
                                              d.IsActive == true &&
                                              d.ExpiryDate >= today);

                    if (validDiscount != null)
                    {
                        var usageCount = await GetDiscountUsageCountAsync(validDiscount.DiscountId);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return validDiscount;
        }

        public static async Task<List<Discount>> SearchDiscountsByNameAsync(string searchTerm)
        {
            var discounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discounts = await context.Discounts
                        .Include(d => d.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Where(d => (d.Description != null && d.Description.Contains(searchTerm)) ||
                                   d.Code.Contains(searchTerm))
                        .OrderByDescending(d => d.Value)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return discounts;
        }

        // Get usage count by counting related orders
        public static async Task<int> GetDiscountUsageCountAsync(int discountId)
        {
            int usageCount = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    // Đếm các đơn đặt hàng tham chiếu giảm giá này
                    var discount = await context.Discounts
                        .Include(d => d.Orders)
                        .FirstOrDefaultAsync(d => d.DiscountId == discountId);

                    usageCount = discount?.Orders?.Count ?? 0;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return usageCount;
        }

        public static async Task SaveDiscountAsync(Discount discount)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Discounts.Add(discount);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateDiscountAsync(Discount discount)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<Discount>(discount).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateDiscountStatusAsync(int discountId, bool isActive)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var discount = await context.Discounts
                        .SingleOrDefaultAsync(d => d.DiscountId == discountId);

                    if (discount != null)
                    {
                        discount.IsActive = isActive;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteDiscountAsync(Discount discount)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingDiscount = await context.Discounts
                        .SingleOrDefaultAsync(c => c.DiscountId == discount.DiscountId);

                    if (existingDiscount != null)
                    {
                        context.Discounts.Remove(existingDiscount);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task<bool> CanDeleteDiscountAsync(int discountId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    // Kiểm tra xem có bất kỳ đơn đặt hàng nào tham khảo giảm giá này
                    var discount = await context.Discounts
                        .Include(d => d.Orders)
                        .FirstOrDefaultAsync(d => d.DiscountId == discountId);

                    bool hasOrders = discount?.Orders?.Any() == true;
                    
                    return !hasOrders;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<Discount>> GetDiscountsByStatusAsync(bool isActive)
        {
            var discounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    discounts = await context.Discounts
                        .Include(d => d.Orders)
                        .Include(d => d.OrderItems)
                        .Where(d => d.IsActive == isActive)
                        .OrderBy(d => d.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts by status: {e.Message}");
            }
            return discounts;
        }

        // Get discounts expiring soon
        public static async Task<List<Discount>> GetDiscountsExpiringSoonAsync(int days = 7)
        {
            var discounts = new List<Discount>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    var futureDate = today.AddDays(days);

                    discounts = await context.Discounts
                        .Include(d => d.Orders)
                        .Where(d => d.IsActive == true &&
                                   d.ExpiryDate >= today &&
                                   d.ExpiryDate <= futureDate)
                        .OrderBy(d => d.ExpiryDate)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to retrieve discounts expiring soon: {e.Message}");
            }
            return discounts;
        }
    }
}