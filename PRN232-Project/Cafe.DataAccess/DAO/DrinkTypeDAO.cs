using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class DrinkTypeDAO
    {
        public static async Task<List<DrinkType>> GetDrinkTypesAsync()
        {
            var listDrinkTypes = new List<DrinkType>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listDrinkTypes = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                        .OrderBy(dt => dt.TypeName)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listDrinkTypes;
        }

        public static async Task<List<DrinkType>> GetActiveDrinkTypesAsync()
        {
            var activeDrinkTypes = new List<DrinkType>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    activeDrinkTypes = await context.DrinkTypes
                        .Include(dt => dt.MenuItems.Where(mi => mi.IsAvailable))
                        .OrderBy(dt => dt.TypeName)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return activeDrinkTypes;
        }

        public static async Task<List<DrinkType>> GetDrinkTypesWithMenuItemsAsync()
        {
            var drinkTypesWithItems = new List<DrinkType>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    drinkTypesWithItems = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                        .Where(dt => dt.MenuItems.Any())
                        .OrderBy(dt => dt.TypeName)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return drinkTypesWithItems;
        }

        public static async Task<DrinkType> FindDrinkTypeByIdAsync(int drinkTypeId)
        {
            DrinkType drinkType = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    drinkType = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                            .ThenInclude(mi => mi.MenuItemImages.Where(img => img.IsMainImage))
                        .SingleOrDefaultAsync(x => x.DrinkTypeId == drinkTypeId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return drinkType;
        }

        public static async Task<DrinkType> FindDrinkTypeByNameAsync(string typeName)
        {
            DrinkType drinkType = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    drinkType = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                        .SingleOrDefaultAsync(x => x.TypeName.Equals(typeName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return drinkType;
        }

        public static async Task<List<DrinkType>> SearchDrinkTypesByNameAsync(string searchTerm)
        {
            var drinkTypes = new List<DrinkType>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    drinkTypes = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                        .Where(dt => dt.TypeName.Contains(searchTerm))
                        .OrderBy(dt => dt.TypeName)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return drinkTypes;
        }

        public static async Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId)
        {
            int count = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    count = await context.MenuItems
                        .CountAsync(mi => mi.DrinkTypeId == drinkTypeId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public static async Task<List<DrinkType>> GetPopularDrinkTypesAsync(int topCount = 5)
        {
            var popularTypes = new List<DrinkType>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    popularTypes = await context.DrinkTypes
                        .Include(dt => dt.MenuItems)
                            .ThenInclude(mi => mi.OrderItems)
                        .OrderByDescending(dt => dt.MenuItems
                            .SelectMany(mi => mi.OrderItems)
                            .Sum(oi => oi.Quantity))
                        .Take(topCount)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return popularTypes;
        }

        public static async Task SaveDrinkTypeAsync(DrinkType drinkType)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.DrinkTypes.Add(drinkType);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateDrinkTypeAsync(DrinkType drinkType)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<DrinkType>(drinkType).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var drinkType = await context.DrinkTypes
                        .SingleOrDefaultAsync(dt => dt.DrinkTypeId == drinkTypeId);

                    if (drinkType != null)
                    {
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteDrinkTypeAsync(DrinkType drinkType)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingDrinkType = await context.DrinkTypes
                        .SingleOrDefaultAsync(c => c.DrinkTypeId == drinkType.DrinkTypeId);

                    if (existingDrinkType != null)
                    {
                        context.DrinkTypes.Remove(existingDrinkType);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task<bool> CanDeleteDrinkTypeAsync(int drinkTypeId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var hasMenuItems = await context.MenuItems
                        .AnyAsync(mi => mi.DrinkTypeId == drinkTypeId);

                    return !hasMenuItems;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
