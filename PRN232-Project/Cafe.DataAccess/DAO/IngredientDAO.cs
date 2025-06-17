using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class IngredientDAO
    {
        public static async Task<List<Ingredient>> GetIngredientsAsync()
        {
            var listIngredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listIngredients = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .OrderBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listIngredients;
        }

        public static async Task<List<Ingredient>> GetAvailableIngredientsAsync()
        {
            var availableIngredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    availableIngredients = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .Where(i => i.Quantity > 5)
                        .OrderBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableIngredients;
        }

        public static async Task<List<Ingredient>> GetLowStockIngredientsAsync()
        {
            var lowStockIngredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    lowStockIngredients = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .Where(i => i.Quantity <= 5)
                        .OrderBy(i => i.Quantity)
                        .ThenBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lowStockIngredients;
        }

        public static async Task<List<Ingredient>> GetOutOfStockIngredientsAsync()
        {
            var outOfStockIngredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    outOfStockIngredients = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .Where(i => i.Quantity == 0)
                        .OrderBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return outOfStockIngredients;
        }

        public static async Task<List<Ingredient>> GetIngredientsByDrinkRecipesAsync(int drinkRecipesId)
        {
            var ingredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ingredients = await context.Ingredients
                        .Where(i => i.DrinkRecipes.Any(dr => dr.RecipeId == drinkRecipesId))
                        .OrderBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredients;
        }

        public static async Task<List<Ingredient>> SearchIngredientsByNameAsync(string searchTerm)
        {
            var ingredients = new List<Ingredient>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ingredients = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .Where(i => i.Name.Contains(searchTerm))
                        .OrderBy(i => i.Name)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredients;
        }

        public static async Task<Ingredient> FindIngredientByIdAsync(int ingredientId)
        {
            Ingredient ingredient = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ingredient = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .SingleOrDefaultAsync(x => x.IngredientId == ingredientId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredient;
        }

        public static async Task<Ingredient> FindIngredientByNameAsync(string ingredientName)
        {
            Ingredient ingredient = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ingredient = await context.Ingredients
                        .Include(i => i.DrinkRecipes)
                        .SingleOrDefaultAsync(x => x.Name.Equals(ingredientName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredient;
        }

        public static async Task<decimal> GetTotalInventoryValueAsync()
        {
            decimal totalValue = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    totalValue = await context.Ingredients
                        .SumAsync(i => i.Quantity * i.UnitPrice);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalValue;
        }

        public static async Task<int> GetTotalIngredientsCountAsync()
        {
            int count = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    count = await context.Ingredients.CountAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public static async Task SaveIngredientAsync(Ingredient ingredient)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Ingredients.Add(ingredient);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<Ingredient>(ingredient).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteIngredientAsync(Ingredient ingredient)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingIngredient = await context.Ingredients
                        .SingleOrDefaultAsync(c => c.IngredientId == ingredient.IngredientId);

                    if (existingIngredient != null)
                    {
                        context.Ingredients.Remove(existingIngredient);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateStockAsync(int ingredientId, int quantity)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var ingredient = await context.Ingredients
                        .SingleOrDefaultAsync(i => i.IngredientId == ingredientId);

                    if (ingredient != null)
                    {
                        var oldQuantity = ingredient.Quantity;
                        ingredient.Quantity += quantity;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateUnitPriceAsync(int ingredientId, decimal newPrice)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var ingredient = await context.Ingredients
                        .SingleOrDefaultAsync(i => i.IngredientId == ingredientId);

                    if (ingredient != null)
                    {
                        ingredient.UnitPrice = newPrice;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task BulkUpdateStockAsync(Dictionary<int, int> ingredientQuantities)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    foreach (var item in ingredientQuantities)
                    {
                        var ingredient = await context.Ingredients
                            .SingleOrDefaultAsync(i => i.IngredientId == item.Key);

                        if (ingredient is null) continue;

                        ingredient.Quantity = item.Value;
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
