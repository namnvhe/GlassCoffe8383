using Cafe.BusinessObjects.Models;

namespace Cafe.Repositories.IRepository
{
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllAsync();
        Task<MenuItem> FindMenuItemByIdAsync(int menuItemId);
        Task<MenuItem> FindMenuItemByNameAsync(string menuItemName);
        Task SaveMenuItemAsync(MenuItem menuItem);
        Task UpdateMenuItemAsync(MenuItem menuItem);
        Task DeleteMenuItemAsync(MenuItem menuItem);

        // tìm kiếm và lọc
        Task<List<MenuItem>> GetAvailableMenuItemsAsync();
        Task<List<MenuItem>> GetMenuItemsByDrinkTypeIdAsync(int drinkTypeId);
        Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm);
        Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<List<MenuItem>> GetPopularMenuItemsAsync(int topCount = 10);

        // cập nhật đặc biệt
        Task UpdateMenuItemAvailabilityAsync(int menuItemId, bool isAvailable);
        Task UpdateMenuItemPriceAsync(int menuItemId, decimal newPrice);

        // thống kê
        Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId);
    }
}