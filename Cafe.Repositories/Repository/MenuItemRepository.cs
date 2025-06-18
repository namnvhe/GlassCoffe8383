using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MenuItemDAO _menuItemDAO;

        public MenuItemRepository(MenuItemDAO menuItemDAO)
        {
            _menuItemDAO = menuItemDAO ?? throw new ArgumentNullException(nameof(menuItemDAO));
        }

        public async Task<List<MenuItem>> GetAllAsync() =>
            await _menuItemDAO.GetAllAsync();

        public async Task<MenuItem> FindMenuItemByIdAsync(int menuItemId) =>
            await _menuItemDAO.FindMenuItemByIdAsync(menuItemId);

        public async Task<MenuItem> FindMenuItemByNameAsync(string menuItemName) =>
            await _menuItemDAO.FindMenuItemByNameAsync(menuItemName);

        public async Task<List<MenuItem>> GetAvailableMenuItemsAsync() =>
            await _menuItemDAO.GetAvailableMenuItemsAsync();

        public async Task<List<MenuItem>> GetMenuItemsByDrinkTypeIdAsync(int drinkTypeId) =>
            await _menuItemDAO.GetMenuItemsByDrinkTypeIdAsync(drinkTypeId);

        public async Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm) =>
            await _menuItemDAO.SearchMenuItemsByNameAsync(searchTerm);

        public async Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice) =>
            await _menuItemDAO.GetMenuItemsByPriceRangeAsync(minPrice, maxPrice);

        public async Task<List<MenuItem>> GetPopularMenuItemsAsync(int topCount = 10) =>
            await _menuItemDAO.GetPopularMenuItemsAsync(topCount);

        public async Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId) =>
            await _menuItemDAO.GetMenuItemCountByDrinkTypeAsync(drinkTypeId);

        public async Task SaveMenuItemAsync(MenuItem menuItem) =>
            await _menuItemDAO.SaveMenuItemAsync(menuItem);

        public async Task UpdateMenuItemAsync(MenuItem menuItem) =>
            await _menuItemDAO.UpdateMenuItemAsync(menuItem);

        public async Task UpdateMenuItemAvailabilityAsync(int menuItemId, bool isAvailable) =>
            await _menuItemDAO.UpdateMenuItemAvailabilityAsync(menuItemId, isAvailable);

        public async Task UpdateMenuItemPriceAsync(int menuItemId, decimal newPrice) =>
            await _menuItemDAO.UpdateMenuItemPriceAsync(menuItemId, newPrice);

        public async Task DeleteMenuItemAsync(MenuItem menuItem) =>
            await _menuItemDAO.DeleteMenuItemAsync(menuItem);
    }
}