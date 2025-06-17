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
    public class SizeRepository : ISizeRepository
    {
        private readonly SizeDAO _sizeDAO;

        public SizeRepository(SizeDAO sizeDAO)
        {
            _sizeDAO = sizeDAO ?? throw new ArgumentNullException(nameof(sizeDAO));
        }

        public async Task<List<Size>> GetAllAsync() =>
            await _sizeDAO.GetAllAsync();

        public async Task<Size> FindSizeByIdAsync(int sizeId) =>
            await _sizeDAO.FindSizeByIdAsync(sizeId);

        public async Task<Size> FindSizeByNameAsync(string name) =>
            await _sizeDAO.FindSizeByNameAsync(name);

        public async Task<List<Size>> GetSizesByPriceRangeAsync(decimal minPrice, decimal maxPrice) =>
            await _sizeDAO.GetSizesByPriceRangeAsync(minPrice, maxPrice);

        public async Task<List<Size>> SearchSizesByNameAsync(string searchTerm) =>
            await _sizeDAO.SearchSizesByNameAsync(searchTerm);

        public async Task<List<Size>> GetPopularSizesAsync(int topCount = 5) =>
            await _sizeDAO.GetPopularSizesAsync(topCount);

        public async Task<int> GetOrderCountBySizeAsync(int sizeId) =>
            await _sizeDAO.GetOrderCountBySizeAsync(sizeId);

        public async Task SaveSizeAsync(Size size) =>
            await _sizeDAO.SaveSizeAsync(size);

        public async Task UpdateSizeAsync(Size size) =>
            await _sizeDAO.UpdateSizeAsync(size);

        public async Task UpdateSizePriceAsync(int sizeId, decimal newExtraPrice) =>
            await _sizeDAO.UpdateSizePriceAsync(sizeId, newExtraPrice);

        public async Task UpdateSizeNameAsync(int sizeId, string newName) =>
            await _sizeDAO.UpdateSizeNameAsync(sizeId, newName);

        public async Task DeleteSizeAsync(Size size) =>
            await _sizeDAO.DeleteSizeAsync(size);
    }
}
