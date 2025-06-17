using Cafe.BusinessObjects.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class SizeDAO
    {
        private readonly CoffeManagerContext _context;

        public SizeDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Size> FindSizeByNameAsync(string name)
        {
            Size size = null;
            try
            {
                size = await _context.Sizes
                    .SingleOrDefaultAsync(x => x.Name.Equals(name));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return size;
        }

        public async Task<List<Size>> GetSizesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var sizes = new List<Size>();
            try
            {
                sizes = await _context.Sizes
                    .Where(s => s.ExtraPrice >= minPrice && s.ExtraPrice <= maxPrice)
                    .OrderBy(s => s.ExtraPrice)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sizes;
        }

        // Admin
        public async Task<List<Size>> GetAllAsync()
        {
            var listSizes = new List<Size>();
            try
            {
                listSizes = await _context.Sizes
                    .Include(s => s.OrderItems)
                    .OrderBy(s => s.ExtraPrice)
                    .ThenBy(s => s.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listSizes;
        }

        public async Task<Size> FindSizeByIdAsync(int sizeId)
        {
            Size size = null;
            try
            {
                size = await _context.Sizes
                    .Include(s => s.OrderItems)
                    .SingleOrDefaultAsync(x => x.SizeId == sizeId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return size;
        }

        public async Task<List<Size>> GetPopularSizesAsync(int topCount = 5)
        {
            var popularSizes = new List<Size>();
            try
            {
                popularSizes = await _context.Sizes
                    .Include(s => s.OrderItems)
                    .OrderByDescending(s => s.OrderItems.Sum(oi => oi.Quantity))
                    .Take(topCount)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return popularSizes;
        }

        public async Task<int> GetOrderCountBySizeAsync(int sizeId)
        {
            int count = 0;
            try
            {
                count = await _context.Sizes
                    .Where(s => s.SizeId == sizeId)
                    .SelectMany(s => s.OrderItems)
                    .SumAsync(oi => oi.Quantity);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public async Task<List<Size>> SearchSizesByNameAsync(string searchTerm)
        {
            var sizes = new List<Size>();
            try
            {
                sizes = await _context.Sizes
                    .Where(s => s.Name.Contains(searchTerm))
                    .OrderBy(s => s.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return sizes;
        }

        public async Task SaveSizeAsync(Size size)
        {
            try
            {
                _context.Sizes.Add(size);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Unique constraint error
                {
                    throw new InvalidOperationException(
                        "Tên size đã tồn tại. Vui lòng chọn tên khác.",
                        ex);
                }
                throw new Exception($"Lỗi cơ sở dữ liệu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định: {ex.Message}", ex);
            }
        }

        public async Task UpdateSizeAsync(Size size)
        {
            try
            {
                _context.Entry<Size>(size).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateSizePriceAsync(int sizeId, decimal newExtraPrice)
        {
            try
            {
                var size = await _context.Sizes
                    .SingleOrDefaultAsync(s => s.SizeId == sizeId);

                if (size != null)
                {
                    size.ExtraPrice = newExtraPrice;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateSizeNameAsync(int sizeId, string newName)
        {
            try
            {
                var size = await _context.Sizes
                    .SingleOrDefaultAsync(s => s.SizeId == sizeId);

                if (size != null)
                {
                    size.Name = newName;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteSizeAsync(Size size)
        {
            try
            {
                var existingSize = await _context.Sizes
                    .SingleOrDefaultAsync(s => s.SizeId == size.SizeId);

                if (existingSize != null)
                {
                    _context.Sizes.Remove(existingSize);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
