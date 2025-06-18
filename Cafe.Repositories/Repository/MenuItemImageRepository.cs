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
    public class MenuItemImageRepository : IMenuItemImageRepository
    {
        private readonly MenuItemImageDAO _menuItemImageDAO;

        public MenuItemImageRepository(MenuItemImageDAO menuItemImageDAO)
        {
            _menuItemImageDAO = menuItemImageDAO ?? throw new ArgumentNullException(nameof(menuItemImageDAO));
        }

        public async Task<List<MenuItemImage>> GetAllAsync() =>
            await _menuItemImageDAO.GetMenuItemImagesAsync();

        public async Task<MenuItemImage> FindMenuItemImageByIdAsync(int imageId) =>
            await _menuItemImageDAO.FindMenuItemImageByIdAsync(imageId);

        public async Task<List<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId) =>
            await _menuItemImageDAO.GetImagesByMenuItemIdAsync(menuItemId);

        public async Task<MenuItemImage> GetPrimaryImageByMenuItemIdAsync(int menuItemId) =>
            await _menuItemImageDAO.GetPrimaryImageByMenuItemIdAsync(menuItemId);

        public async Task<List<MenuItemImage>> GetImagesByFileTypeAsync(string fileType) =>
            await _menuItemImageDAO.GetImagesByFileTypeAsync(fileType);

        public async Task<List<MenuItemImage>> GetImagesByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await _menuItemImageDAO.GetImagesByDateRangeAsync(startDate, endDate);

        public async Task<int> GetImageCountByMenuItemIdAsync(int menuItemId) =>
            await _menuItemImageDAO.GetImageCountByMenuItemIdAsync(menuItemId);

        public async Task SaveMenuItemImageAsync(MenuItemImage image) =>
            await _menuItemImageDAO.SaveMenuItemImageAsync(image);

        public async Task SaveMultipleMenuItemImagesAsync(List<MenuItemImage> images) =>
            await _menuItemImageDAO.SaveMultipleMenuItemImagesAsync(images);

        public async Task UpdateMenuItemImageAsync(MenuItemImage image) =>
            await _menuItemImageDAO.UpdateMenuItemImageAsync(image);

        public async Task DeleteMenuItemImageAsync(MenuItemImage image) =>
            await _menuItemImageDAO.DeleteMenuItemImageAsync(image);

        public async Task DeleteImagesByMenuItemIdAsync(int menuItemId) =>
            await _menuItemImageDAO.DeleteImagesByMenuItemIdAsync(menuItemId);

        public async Task SetPrimaryImageAsync(int imageId) =>
            await _menuItemImageDAO.SetPrimaryImageAsync(imageId);

        public async Task UpdateDisplayOrderAsync(int imageId, int newDisplayOrder) =>
            await _menuItemImageDAO.UpdateDisplayOrderAsync(imageId, newDisplayOrder);

        public async Task ReorderImagesAsync(int menuItemId, List<int> imageIds) =>
            await _menuItemImageDAO.ReorderImagesAsync(menuItemId, imageIds);
    }
}
