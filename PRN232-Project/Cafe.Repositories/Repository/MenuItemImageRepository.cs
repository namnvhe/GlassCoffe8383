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
        public async Task<List<MenuItemImage>> GetAllAsync() =>
            await MenuItemImageDAO.GetMenuItemImagesAsync();

        public async Task<MenuItemImage> FindMenuItemImageByIdAsync(int imageId) =>
            await MenuItemImageDAO.FindMenuItemImageByIdAsync(imageId);

        public async Task<List<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId) =>
            await MenuItemImageDAO.GetImagesByMenuItemIdAsync(menuItemId);

        public async Task<MenuItemImage> GetPrimaryImageByMenuItemIdAsync(int menuItemId) =>
            await MenuItemImageDAO.GetPrimaryImageByMenuItemIdAsync(menuItemId);

        public async Task<List<MenuItemImage>> GetImagesByFileTypeAsync(string fileType) =>
            await MenuItemImageDAO.GetImagesByFileTypeAsync(fileType);

        public async Task<List<MenuItemImage>> GetImagesByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await MenuItemImageDAO.GetImagesByDateRangeAsync(startDate, endDate);

        public async Task<int> GetImageCountByMenuItemIdAsync(int menuItemId) =>
            await MenuItemImageDAO.GetImageCountByMenuItemIdAsync(menuItemId);

        public async Task SaveMenuItemImageAsync(MenuItemImage image) =>
            await MenuItemImageDAO.SaveMenuItemImageAsync(image);

        public async Task SaveMultipleMenuItemImagesAsync(List<MenuItemImage> images) =>
            await MenuItemImageDAO.SaveMultipleMenuItemImagesAsync(images);

        public async Task UpdateMenuItemImageAsync(MenuItemImage image) =>
            await MenuItemImageDAO.UpdateMenuItemImageAsync(image);

        public async Task DeleteMenuItemImageAsync(MenuItemImage image) =>
            await MenuItemImageDAO.DeleteMenuItemImageAsync(image);

        public async Task DeleteImagesByMenuItemIdAsync(int menuItemId) =>
            await MenuItemImageDAO.DeleteImagesByMenuItemIdAsync(menuItemId);

        public async Task SetPrimaryImageAsync(int imageId) =>
            await MenuItemImageDAO.SetPrimaryImageAsync(imageId);

        public async Task UpdateDisplayOrderAsync(int imageId, int newDisplayOrder) =>
            await MenuItemImageDAO.UpdateDisplayOrderAsync(imageId, newDisplayOrder);

        public async Task ReorderImagesAsync(int menuItemId, List<int> imageIds) =>
            await MenuItemImageDAO.ReorderImagesAsync(menuItemId, imageIds);
    }
}
