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
    public class MenuItemRepository : IMenuItemRepository
    {
        public async Task<List<MenuItem>> GetAllAsync() =>
            await MenuItemDAO.GetMenuItemsAsync();

        public async Task<MenuItem> FindMenuItemByIdAsync(int menuItemId) =>
            await MenuItemDAO.FindMenuItemByIdAsync(menuItemId);

        public async Task<MenuItem> FindMenuItemByNameAsync(string menuItemName) =>
            await MenuItemDAO.FindMenuItemByNameAsync(menuItemName);

        public async Task<List<MenuItem>> GetAvailableMenuItemsAsync() =>
            await MenuItemDAO.GetAvailableMenuItemsAsync();

        public async Task<List<MenuItem>> GetMenuItemsByDrinkTypeIdAsync(int drinkTypeId) =>
            await MenuItemDAO.GetMenuItemsByDrinkTypeIdAsync(drinkTypeId);

        public async Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm) =>
            await MenuItemDAO.SearchMenuItemsByNameAsync(searchTerm);

        public async Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice) =>
            await MenuItemDAO.GetMenuItemsByPriceRangeAsync(minPrice, maxPrice);

        public async Task<List<MenuItem>> GetPopularMenuItemsAsync(int topCount = 10) =>
            await MenuItemDAO.GetPopularMenuItemsAsync(topCount);

        public async Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId) =>
            await MenuItemDAO.GetMenuItemCountByDrinkTypeAsync(drinkTypeId);

        public async Task SaveMenuItemAsync(MenuItem menuItem) =>
            await MenuItemDAO.SaveMenuItemAsync(menuItem);

        public async Task UpdateMenuItemAsync(MenuItem menuItem) =>
            await MenuItemDAO.UpdateMenuItemAsync(menuItem);

        public async Task UpdateMenuItemAvailabilityAsync(int menuItemId, bool isAvailable) =>
            await MenuItemDAO.UpdateMenuItemAvailabilityAsync(menuItemId, isAvailable);

        public async Task UpdateMenuItemPriceAsync(int menuItemId, decimal newPrice) =>
            await MenuItemDAO.UpdateMenuItemPriceAsync(menuItemId, newPrice);

        public async Task DeleteMenuItemAsync(MenuItem menuItem) =>
            await MenuItemDAO.DeleteMenuItemAsync(menuItem);
    }
}
