using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class IngredientDAO
    {
        private readonly CoffeManagerContext _context;

        public IngredientDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Ingredient>> GetIngredientsAsync()
        {
            var listIngredients = new List<Ingredient>();
            try
            {
                listIngredients = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listIngredients;
        }

        public async Task<List<Ingredient>> GetAvailableIngredientsAsync()
        {
            var availableIngredients = new List<Ingredient>();
            try
            {
                availableIngredients = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .Where(i => i.Quantity > 5)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableIngredients;
        }

        public async Task<List<Ingredient>> GetLowStockIngredientsAsync()
        {
            var lowStockIngredients = new List<Ingredient>();
            try
            {
                lowStockIngredients = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .Where(i => i.Quantity <= 5)
                    .OrderBy(i => i.Quantity)
                    .ThenBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return lowStockIngredients;
        }

        public async Task<List<Ingredient>> GetOutOfStockIngredientsAsync()
        {
            var outOfStockIngredients = new List<Ingredient>();
            try
            {
                outOfStockIngredients = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .Where(i => i.Quantity == 0)
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return outOfStockIngredients;
        }

        public async Task<List<Ingredient>> GetIngredientsByDrinkRecipesAsync(int drinkRecipesId)
        {
            var ingredients = new List<Ingredient>();
            try
            {
                ingredients = await _context.Ingredients
                    .Where(i => i.DrinkRecipes.Any(dr => dr.RecipeId == drinkRecipesId))
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredients;
        }

        public async Task<List<Ingredient>> SearchIngredientsByNameAsync(string searchTerm)
        {
            var ingredients = new List<Ingredient>();
            try
            {
                ingredients = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .Where(i => i.Name.Contains(searchTerm))
                    .OrderBy(i => i.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredients;
        }

        public async Task<Ingredient> FindIngredientByIdAsync(int ingredientId)
        {
            Ingredient ingredient = null;
            try
            {
                ingredient = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .SingleOrDefaultAsync(x => x.IngredientId == ingredientId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredient;
        }

        public async Task<Ingredient> FindIngredientByNameAsync(string ingredientName)
        {
            Ingredient ingredient = null;
            try
            {
                var trimmedName = ingredientName.Trim().ToLower();
                ingredient = await _context.Ingredients
                    .Include(i => i.DrinkRecipes)
                    .FirstOrDefaultAsync(x => x.Name.ToLower().Equals(trimmedName));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ingredient;
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            try
            {
                var ingredients = await _context.Ingredients
                    .Select(i => new { i.Quantity, i.UnitPrice })
                    .ToListAsync();

                return ingredients.Sum(i => (decimal)i.Quantity * i.UnitPrice);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> GetTotalIngredientsCountAsync()
        {
            int count = 0;
            try
            {
                count = await _context.Ingredients.CountAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public async Task SaveIngredientAsync(Ingredient ingredient)
        {
            try
            {
                ingredient.Name = ingredient.Name.Trim();
                var trimmedName = ingredient.Name.ToLower();
                var existingIngredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == trimmedName);
                if (existingIngredient != null)
                {
                    throw new InvalidOperationException("Tên nguyên liệu đã tồn tại");
                }
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            try
            {
                _context.Entry<Ingredient>(ingredient).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteIngredientAsync(Ingredient ingredient)
        {
            try
            {
                var existingIngredient = await _context.Ingredients
                    .SingleOrDefaultAsync(c => c.IngredientId == ingredient.IngredientId);

                if (existingIngredient != null)
                {
                    _context.Ingredients.Remove(existingIngredient);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateStockAsync(int ingredientId, int quantity)
        {
            try
            {
                var ingredient = await _context.Ingredients
                    .SingleOrDefaultAsync(i => i.IngredientId == ingredientId);

                if (ingredient != null)
                {
                    var oldQuantity = ingredient.Quantity;
                    ingredient.Quantity += quantity;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateUnitPriceAsync(int ingredientId, decimal newPrice)
        {
            try
            {
                var ingredient = await _context.Ingredients
                    .SingleOrDefaultAsync(i => i.IngredientId == ingredientId);

                if (ingredient != null)
                {
                    ingredient.UnitPrice = newPrice;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task BulkUpdateStockAsync(Dictionary<int, int> ingredientQuantities)
        {
            try
            {
                foreach (var item in ingredientQuantities)
                {
                    var ingredient = await _context.Ingredients
                        .SingleOrDefaultAsync(i => i.IngredientId == item.Key);

                    if (ingredient is null) continue;

                    ingredient.Quantity = item.Value;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}