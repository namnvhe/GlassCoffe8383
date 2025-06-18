using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IMenuItemImageRepository
    {
        Task<List<MenuItemImage>> GetAllAsync();
        Task<MenuItemImage> FindMenuItemImageByIdAsync(int imageId);
        Task SaveMenuItemImageAsync(MenuItemImage image);
        Task UpdateMenuItemImageAsync(MenuItemImage image);
        Task DeleteMenuItemImageAsync(MenuItemImage image);

        // tìm kiếm theo quan hệ
        Task<List<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId);
        Task<MenuItemImage> GetPrimaryImageByMenuItemIdAsync(int menuItemId);
        Task<List<MenuItemImage>> GetImagesByFileTypeAsync(string fileType);
        Task<List<MenuItemImage>> GetImagesByDateRangeAsync(DateTime startDate, DateTime endDate);

        // xử lý hàng loạt
        Task SaveMultipleMenuItemImagesAsync(List<MenuItemImage> images);
        Task DeleteImagesByMenuItemIdAsync(int menuItemId);

        // quản lý hiển thị
        Task SetPrimaryImageAsync(int imageId);
        Task UpdateDisplayOrderAsync(int imageId, int newDisplayOrder);
        Task ReorderImagesAsync(int menuItemId, List<int> imageIds);

        // thống kê
        Task<int> GetImageCountByMenuItemIdAsync(int menuItemId);
    }
}
