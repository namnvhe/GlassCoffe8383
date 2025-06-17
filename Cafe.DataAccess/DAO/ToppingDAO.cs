using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class ToppingDAO
    {
        public static async Task<List<Topping>> GetToppingsAsync()
        {
            var listToppings = new List<Topping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listToppings = await context.Toppings.ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listToppings;
        }

        public static async Task<List<Topping>> GetAvailableToppingsAsync()
        {
            var availableToppings = new List<Topping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    availableToppings = await context.Toppings
                        .Where(t => t.IsAvailable == true)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableToppings;
        }

        public static async Task<Topping> FindToppingByIdAsync(int toppingId)
        {
            Topping topping = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    topping = await context.Toppings.SingleOrDefaultAsync(x => x.ToppingId == toppingId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return topping;
        }

        public static async Task<List<Topping>> FindToppingsByNameAsync(string name)
        {
            var toppings = new List<Topping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    toppings = await context.Toppings
                        .Where(t => t.Name.Contains(name))
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return toppings;
        }

        public static async Task<List<Topping>> GetToppingsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var toppings = new List<Topping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    toppings = await context.Toppings
                        .Where(t => t.Price >= minPrice && t.Price <= maxPrice)
                        .OrderBy(t => t.Price)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return toppings;
        }

        public static async Task SaveToppingAsync(Topping topping)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Toppings.Add(topping);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateToppingAsync(Topping topping)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<Topping>(topping).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteToppingAsync(Topping topping)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingTopping = await context.Toppings.SingleOrDefaultAsync(c => c.ToppingId == topping.ToppingId);
                    if (existingTopping != null)
                    {
                        context.Toppings.Remove(existingTopping);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateToppingAvailabilityAsync(int toppingId, bool isAvailable)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var topping = await context.Toppings.SingleOrDefaultAsync(t => t.ToppingId == toppingId);
                    if (topping != null)
                    {
                        topping.IsAvailable = isAvailable;
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
