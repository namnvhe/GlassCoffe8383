using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class MenuItemImageDAO
    {
        private readonly CoffeManagerContext _context;

        public MenuItemImageDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<MenuItemImage>> GetMenuItemImagesAsync()
        {
            try
            {
                return await _context.MenuItemImages
                    .Include(img => img.MenuItem)
                    .OrderBy(img => img.MenuItemId)
                    .ThenBy(img => img.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving menu item images: {e.Message}", e);
            }
        }

        public async Task<List<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                return await _context.MenuItemImages
                    .Where(img => img.MenuItemId == menuItemId)
                    .OrderBy(img => img.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving images for menu item {menuItemId}: {e.Message}", e);
            }
        }

        public async Task<MenuItemImage> GetPrimaryImageByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                return await _context.MenuItemImages
                    .Where(img => img.MenuItemId == menuItemId && img.IsMainImage)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving primary image for menu item {menuItemId}: {e.Message}", e);
            }
        }

        public async Task<List<MenuItemImage>> GetImagesByFileTypeAsync(string fileType)
        {
            try
            {
                return await _context.MenuItemImages
                    .Include(img => img.MenuItem)
                    .Where(img => img.ImageUrl.ToLower().EndsWith(fileType.ToLower()))
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving images by file type {fileType}: {e.Message}", e);
            }
        }

        public async Task<List<MenuItemImage>> GetImagesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.MenuItemImages
                    .Include(img => img.MenuItem)
                    .Where(img => img.CreatedAt >= startDate && img.CreatedAt <= endDate)
                    .OrderByDescending(img => img.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error retrieving images by date range: {e.Message}", e);
            }
        }

        public async Task<MenuItemImage> FindMenuItemImageByIdAsync(int imageId)
        {
            try
            {
                return await _context.MenuItemImages
                    .Include(img => img.MenuItem)
                    .SingleOrDefaultAsync(x => x.ImageId == imageId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error finding menu item image with ID {imageId}: {e.Message}", e);
            }
        }

        public async Task<int> GetImageCountByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                return await _context.MenuItemImages
                    .CountAsync(img => img.MenuItemId == menuItemId);
            }
            catch (Exception e)
            {
                throw new Exception($"Error counting images for menu item {menuItemId}: {e.Message}", e);
            }
        }

        public async Task SaveMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                // Nếu đây là ảnh chính, set các ảnh khác của cùng MenuItem thành không phải ảnh chính
                if (image.IsMainImage)
                {
                    var existingImages = await _context.MenuItemImages
                        .Where(img => img.MenuItemId == image.MenuItemId && img.IsMainImage)
                        .ToListAsync();

                    foreach (var existingImage in existingImages)
                    {
                        existingImage.IsMainImage = false;
                        existingImage.UpdatedAt = DateTime.Now;
                    }
                }

                image.CreatedAt = DateTime.Now;
                image.UpdatedAt = DateTime.Now;
                _context.MenuItemImages.Add(image);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving menu item image: {e.Message}", e);
            }
        }

        public async Task SaveMultipleMenuItemImagesAsync(List<MenuItemImage> images)
        {
            try
            {
                foreach (var image in images)
                {
                    image.CreatedAt = DateTime.Now;
                    image.UpdatedAt = DateTime.Now;
                }

                _context.MenuItemImages.AddRange(images);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving multiple menu item images: {e.Message}", e);
            }
        }

        public async Task UpdateMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                // Nếu đây là ảnh chính, set các ảnh khác của cùng MenuItem thành không phải ảnh chính
                if (image.IsMainImage)
                {
                    var existingImages = await _context.MenuItemImages
                        .Where(img => img.MenuItemId == image.MenuItemId &&
                                      img.ImageId != image.ImageId &&
                                      img.IsMainImage)
                        .ToListAsync();

                    foreach (var existingImage in existingImages)
                    {
                        existingImage.IsMainImage = false;
                        existingImage.UpdatedAt = DateTime.Now;
                    }
                }

                image.UpdatedAt = DateTime.Now;
                _context.Entry(image).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating menu item image: {e.Message}", e);
            }
        }

        public async Task DeleteMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                var existingImage = await _context.MenuItemImages
                    .SingleOrDefaultAsync(c => c.ImageId == image.ImageId);

                if (existingImage != null)
                {
                    _context.MenuItemImages.Remove(existingImage);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting menu item image: {e.Message}", e);
            }
        }

        public async Task DeleteImagesByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                var images = await _context.MenuItemImages
                    .Where(img => img.MenuItemId == menuItemId)
                    .ToListAsync();

                if (images.Any())
                {
                    _context.MenuItemImages.RemoveRange(images);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error deleting images for menu item {menuItemId}: {e.Message}", e);
            }
        }

        public async Task SetPrimaryImageAsync(int imageId)
        {
            try
            {
                var image = await _context.MenuItemImages
                    .SingleOrDefaultAsync(img => img.ImageId == imageId);

                if (image != null)
                {
                    // Set tất cả ảnh khác của cùng MenuItem thành không phải ảnh chính
                    var otherImages = await _context.MenuItemImages
                        .Where(img => img.MenuItemId == image.MenuItemId && img.ImageId != imageId)
                        .ToListAsync();

                    foreach (var otherImage in otherImages)
                    {
                        otherImage.IsMainImage = false;
                        otherImage.UpdatedAt = DateTime.Now;
                    }

                    // Set ảnh hiện tại thành ảnh chính
                    image.IsMainImage = true;
                    image.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error setting primary image {imageId}: {e.Message}", e);
            }
        }

        public async Task UpdateDisplayOrderAsync(int imageId, int newDisplayOrder)
        {
            try
            {
                var image = await _context.MenuItemImages
                    .SingleOrDefaultAsync(img => img.ImageId == imageId);

                if (image != null)
                {
                    image.DisplayOrder = newDisplayOrder;
                    image.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error updating display order for image {imageId}: {e.Message}", e);
            }
        }

        public async Task ReorderImagesAsync(int menuItemId, List<int> imageIds)
        {
            try
            {
                for (int i = 0; i < imageIds.Count; i++)
                {
                    var image = await _context.MenuItemImages
                        .SingleOrDefaultAsync(img => img.ImageId == imageIds[i] && img.MenuItemId == menuItemId);

                    if (image != null)
                    {
                        image.DisplayOrder = i + 1;
                        image.UpdatedAt = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Error reordering images for menu item {menuItemId}: {e.Message}", e);
            }
        }
    }
}