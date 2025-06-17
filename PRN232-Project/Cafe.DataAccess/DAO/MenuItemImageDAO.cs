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
        public static async Task<List<MenuItemImage>> GetMenuItemImagesAsync()
        {
            var listImages = new List<MenuItemImage>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listImages = await context.MenuItemImages
                        .Include(img => img.MenuItem)
                        .OrderBy(img => img.MenuItemId)
                        .ThenBy(img => img.DisplayOrder)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listImages;
        }

        public static async Task<List<MenuItemImage>> GetImagesByMenuItemIdAsync(int menuItemId)
        {
            var images = new List<MenuItemImage>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    images = await context.MenuItemImages
                        .Where(img => img.MenuItemId == menuItemId)
                        .OrderBy(img => img.DisplayOrder)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return images;
        }

        public static async Task<MenuItemImage> GetPrimaryImageByMenuItemIdAsync(int menuItemId)
        {
            MenuItemImage primaryImage = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    primaryImage = await context.MenuItemImages
                        .Where(img => img.MenuItemId == menuItemId && img.IsMainImage)
                        .FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return primaryImage;
        }

        public static async Task<List<MenuItemImage>> GetImagesByFileTypeAsync(string fileType)
        {
            var images = new List<MenuItemImage>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    images = await context.MenuItemImages
                        .Include(img => img.MenuItem)
                        .Where(img => img.ImageUrl.ToLower().EndsWith(fileType.ToLower()))
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return images;
        }

        public static async Task<List<MenuItemImage>> GetImagesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var images = new List<MenuItemImage>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    images = await context.MenuItemImages
                        .Include(img => img.MenuItem)
                        .Where(img => img.CreatedAt >= startDate && img.CreatedAt <= endDate)
                        .OrderByDescending(img => img.CreatedAt)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return images;
        }

        public static async Task<MenuItemImage> FindMenuItemImageByIdAsync(int imageId)
        {
            MenuItemImage image = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    image = await context.MenuItemImages
                        .Include(img => img.MenuItem)
                        .SingleOrDefaultAsync(x => x.ImageId == imageId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return image;
        }

        public static async Task<int> GetImageCountByMenuItemIdAsync(int menuItemId)
        {
            int count = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    count = await context.MenuItemImages
                        .CountAsync(img => img.MenuItemId == menuItemId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public static async Task SaveMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    // Nếu đây là ảnh chính, set các ảnh khác của cùng MenuItem thành không phải ảnh chính
                    if (image.IsMainImage)
                    {
                        var existingImages = await context.MenuItemImages
                            .Where(img => img.MenuItemId == image.MenuItemId && img.IsMainImage)
                            .ToListAsync();

                        foreach (var existingImage in existingImages)
                        {
                            existingImage.IsMainImage = false;
                        }
                    }

                    image.CreatedAt = DateTime.Now;
                    image.UpdatedAt = DateTime.Now;
                    context.MenuItemImages.Add(image);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task SaveMultipleMenuItemImagesAsync(List<MenuItemImage> images)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    foreach (var image in images)
                    {
                        image.CreatedAt = DateTime.Now;
                        image.UpdatedAt = DateTime.Now;
                    }

                    context.MenuItemImages.AddRange(images);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    // Nếu đây là ảnh chính, set các ảnh khác của cùng MenuItem thành không phải ảnh chính
                    if (image.IsMainImage)
                    {
                        var existingImages = await context.MenuItemImages
                            .Where(img => img.MenuItemId == image.MenuItemId &&
                                          img.ImageId != image.ImageId &&
                                          img.IsMainImage)
                            .ToListAsync();

                        foreach (var existingImage in existingImages)
                        {
                            existingImage.IsMainImage = false;
                        }
                    }

                    image.UpdatedAt = DateTime.Now;
                    context.Entry<MenuItemImage>(image).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteMenuItemImageAsync(MenuItemImage image)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingImage = await context.MenuItemImages
                        .SingleOrDefaultAsync(c => c.ImageId == image.ImageId);

                    if (existingImage != null)
                    {
                        context.MenuItemImages.Remove(existingImage);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteImagesByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var images = await context.MenuItemImages
                        .Where(img => img.MenuItemId == menuItemId)
                        .ToListAsync();

                    if (images.Any())
                    {
                        context.MenuItemImages.RemoveRange(images);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task SetPrimaryImageAsync(int imageId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var image = await context.MenuItemImages
                        .SingleOrDefaultAsync(img => img.ImageId == imageId);

                    if (image != null)
                    {
                        // Set tất cả ảnh khác của cùng MenuItem thành không phải ảnh chính
                        var otherImages = await context.MenuItemImages
                            .Where(img => img.MenuItemId == image.MenuItemId && img.ImageId != imageId)
                            .ToListAsync();

                        foreach (var otherImage in otherImages)
                        {
                            otherImage.IsMainImage = false;
                        }

                        // Set ảnh hiện tại thành ảnh chính
                        image.IsMainImage = true;
                        image.UpdatedAt = DateTime.Now;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateDisplayOrderAsync(int imageId, int newDisplayOrder)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var image = await context.MenuItemImages
                        .SingleOrDefaultAsync(img => img.ImageId == imageId);

                    if (image != null)
                    {
                        image.DisplayOrder = newDisplayOrder;
                        image.UpdatedAt = DateTime.Now;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task ReorderImagesAsync(int menuItemId, List<int> imageIds)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    for (int i = 0; i < imageIds.Count; i++)
                    {
                        var image = await context.MenuItemImages
                            .SingleOrDefaultAsync(img => img.ImageId == imageIds[i] && img.MenuItemId == menuItemId);

                        if (image != null)
                        {
                            image.DisplayOrder = i + 1;
                            image.UpdatedAt = DateTime.Now;
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
