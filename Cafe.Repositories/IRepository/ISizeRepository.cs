using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface ISizeRepository
    {
        Task<List<Size>> GetAllAsync();
        Task<Size> FindSizeByIdAsync(int sizeId);
        Task<Size> FindSizeByNameAsync(string name);
        Task SaveSizeAsync(Size size);
        Task UpdateSizeAsync(Size size);
        Task DeleteSizeAsync(Size size);

        // tìm kiếm và lọc
        Task<List<Size>> GetSizesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<List<Size>> SearchSizesByNameAsync(string searchTerm);
        Task<List<Size>> GetPopularSizesAsync(int topCount = 5);

        // cập nhật đặc biệt
        Task UpdateSizePriceAsync(int sizeId, decimal newExtraPrice);
        Task UpdateSizeNameAsync(int sizeId, string newName);

        // thống kê
        Task<int> GetOrderCountBySizeAsync(int sizeId);
    }
}
