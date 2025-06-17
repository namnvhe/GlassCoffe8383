using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class MenuItemDAO
    {
        public static async Task<List<MenuItem>> GetMenuItemsAsync()
        {
            var listMenuItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listMenuItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages)
                        .Include(m => m.OrderItems)
                        .OrderBy(m => m.DrinkType.TypeName)
                        .ThenBy(m => m.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listMenuItems;
        }

        public static async Task<List<MenuItem>> GetAvailableMenuItemsAsync()
        {
            var availableMenuItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    availableMenuItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .Where(m => m.IsAvailable == true)
                        .OrderBy(m => m.DrinkType.TypeName)
                        .ThenBy(m => m.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableMenuItems;
        }

        public static async Task<List<MenuItem>> GetMenuItemsByDrinkTypeIdAsync(int DrinkTypeId)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    menuItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .Where(m => m.DrinkTypeId == DrinkTypeId)
                        .OrderBy(m => m.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public static async Task<List<MenuItem>> SearchMenuItemsByNameAsync(string searchTerm)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    menuItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .Where(m => m.Name.Contains(searchTerm) ||
                                   m.Description.Contains(searchTerm))
                        .OrderBy(m => m.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public static async Task<List<MenuItem>> GetMenuItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var menuItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    menuItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .Where(m => m.Price >= minPrice && m.Price <= maxPrice)
                        .OrderBy(m => m.Price)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItems;
        }

        public static async Task<List<MenuItem>> GetPopularMenuItemsAsync(int topCount = 10)
        {
            var popularItems = new List<MenuItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    popularItems = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .Include(m => m.OrderItems)
                        .OrderByDescending(m => m.OrderItems.Sum(oi => oi.Quantity))
                        .Take(topCount)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return popularItems;
        }

        public static async Task<MenuItem> FindMenuItemByIdAsync(int menuItemId)
        {
            MenuItem menuItem = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    menuItem = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages)
                        .Include(m => m.OrderItems)
                        .SingleOrDefaultAsync(x => x.MenuItemId == menuItemId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItem;
        }

        public static async Task<MenuItem> FindMenuItemByNameAsync(string Name)
        {
            MenuItem menuItem = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    menuItem = await context.MenuItems
                        .Include(m => m.DrinkType)
                        .Include(m => m.MenuItemImages.Where(img => img.IsMainImage))
                        .SingleOrDefaultAsync(x => x.Name.Equals(Name));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return menuItem;
        }

        public static async Task<int> GetMenuItemCountByDrinkTypeAsync(int DrinkTypeId)
        {
            int count = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    count = await context.MenuItems
                        .CountAsync(m => m.DrinkTypeId == DrinkTypeId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public static async Task SaveMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.MenuItems.Add(menuItem);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<MenuItem>(menuItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateMenuItemAvailabilityAsync(int menuItemId, bool isAvailable)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var menuItem = await context.MenuItems
                        .SingleOrDefaultAsync(m => m.MenuItemId == menuItemId);

                    if (menuItem != null)
                    {
                        menuItem.IsAvailable = isAvailable;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateMenuItemPriceAsync(int menuItemId, decimal newPrice)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var menuItem = await context.MenuItems
                        .SingleOrDefaultAsync(m => m.MenuItemId == menuItemId);

                    if (menuItem != null)
                    {
                        menuItem.Price = newPrice;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteMenuItemAsync(MenuItem menuItem)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingMenuItem = await context.MenuItems
                        .SingleOrDefaultAsync(c => c.MenuItemId == menuItem.MenuItemId);

                    if (existingMenuItem != null)
                    {
                        context.MenuItems.Remove(existingMenuItem);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
