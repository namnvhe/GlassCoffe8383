using Cafe.BusinessObjects.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class MenuItemDAO
    {
        private readonly CoffeManagerContext _context;

        public MenuItemDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<MenuItem>> GetAllAsync()
        {
            var listMenuItems = new List<MenuItem>();
            try
            {
                listMenuItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages)
                    .Include(m => m.OrderItems)
                    .OrderBy(m => m.DrinkType.TypeName)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMenuItems;
        }

        public async Task<List<MenuItem>> GetAvailableMenuItemsAsync()
        {
            var availableMenuItems = new List<MenuItem>();
            try
            {
                availableMenuItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .Where(m => m.IsAvailable == true)
                    .OrderBy(m => m.DrinkType.TypeName)
                    .ThenBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableMenuItems;
        }

        public async Task<List<MenuItem>> GetMenuItemsByDrinkTypeIdAsync(int drinkTypeId)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                menuItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .Where(m => m.DrinkTypeId == drinkTypeId)
                    .OrderBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public async Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                menuItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .Where(m => m.Name.Contains(searchTerm) || m.Description.Contains(searchTerm))
                    .OrderBy(m => m.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public async Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                menuItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .Where(m => m.Price >= minPrice && m.Price <= maxPrice)
                    .OrderBy(m => m.Price)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public async Task<List<MenuItem>> GetPopularMenuItemsAsync(int topCount = 10)
        {
            var popularItems = new List<MenuItem>();
            try
            {
                popularItems = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .Include(m => m.OrderItems)
                    .OrderByDescending(m => m.OrderItems.Sum(oi => oi.Quantity))
                    .Take(topCount)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return popularItems;
        }

        public async Task<MenuItem> FindMenuItemByIdAsync(int menuItemId)
        {
            MenuItem menuItem = null;
            try
            {
                menuItem = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages)
                    .Include(m => m.OrderItems)
                    .SingleOrDefaultAsync(x => x.MenuItemId == menuItemId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItem;
        }

        public async Task<MenuItem> FindMenuItemByNameAsync(string name)
        {
            MenuItem menuItem = null;
            try
            {
                menuItem = await _context.MenuItems
                    .Include(m => m.DrinkType)
                    .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                    .SingleOrDefaultAsync(x => x.Name.Equals(name));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItem;
        }

        public async Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId)
        {
            int count = 0;
            try
            {
                count = await _context.MenuItems
                    .CountAsync(m => m.DrinkTypeId == drinkTypeId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public async Task SaveMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign Key constraint error
                {
                    throw new InvalidOperationException(
                        "Không thể tạo món ăn vì loại đồ uống không tồn tại. Vui lòng kiểm tra lại DrinkTypeId.",
                        ex);
                }
                throw new Exception($"Lỗi cơ sở dữ liệu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định: {ex.Message}", ex);
            }
        }

        public async Task UpdateMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                _context.Entry<MenuItem>(menuItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateMenuItemAvailabilityAsync(int menuItemId, bool isAvailable)
        {
            try
            {
                var menuItem = await _context.MenuItems
                    .SingleOrDefaultAsync(m => m.MenuItemId == menuItemId);

                if (menuItem != null)
                {
                    menuItem.IsAvailable = isAvailable;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateMenuItemPriceAsync(int menuItemId, decimal newPrice)
        {
            try
            {
                var menuItem = await _context.MenuItems
                    .SingleOrDefaultAsync(m => m.MenuItemId == menuItemId);

                if (menuItem != null)
                {
                    menuItem.Price = newPrice;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                var existingMenuItem = await _context.MenuItems
                    .SingleOrDefaultAsync(c => c.MenuItemId == menuItem.MenuItemId);

                if (existingMenuItem != null)
                {
                    _context.MenuItems.Remove(existingMenuItem);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}