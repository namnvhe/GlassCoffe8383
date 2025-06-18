using Cafe.BusinessObjects.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Cafe.DataAccess.DAO
{
    public class DrinkRecipeDAO
    {
        private readonly CoffeManagerContext _context;

        public DrinkRecipeDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Customer 
        public async Task<List<DrinkRecipe>> GetRecipesByMenuItemIdAsync(int menuItemId)
        {
            var recipes = new List<DrinkRecipe>();
            try
            {
                recipes = await _context.DrinkRecipes
                    .Include(r => r.Ingredient)
                    .Include(r => r.MenuItem)
                    .Where(r => r.MenuItemId == menuItemId)
                    .OrderBy(r => r.Ingredient.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return recipes;
        }

        public async Task<List<DrinkRecipe>> GetRecipesByIngredientIdAsync(int ingredientId)
        {
            var recipes = new List<DrinkRecipe>();
            try
            {
                recipes = await _context.DrinkRecipes
                    .Include(r => r.Ingredient)
                    .Include(r => r.MenuItem)
                    .Where(r => r.IngredientId == ingredientId)
                    .OrderBy(r => r.MenuItem.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return recipes;
        }

        // Admin
        public async Task<List<DrinkRecipe>> GetAllAsync()
        {
            var listRecipes = new List<DrinkRecipe>();
            try
            {
                listRecipes = await _context.DrinkRecipes
                    .Include(r => r.Ingredient)
                    .Include(r => r.MenuItem)
                    .OrderBy(r => r.MenuItem.Name)
                    .ThenBy(r => r.Ingredient.Name)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listRecipes;
        }

        public async Task<DrinkRecipe> FindRecipeByIdAsync(int recipeId)
        {
            DrinkRecipe recipe = null;
            try
            {
                recipe = await _context.DrinkRecipes
                    .Include(r => r.Ingredient)
                    .Include(r => r.MenuItem)
                    .SingleOrDefaultAsync(x => x.RecipeId == recipeId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return recipe;
        }

        public async Task<DrinkRecipe> FindRecipeByMenuItemAndIngredientAsync(int menuItemId, int ingredientId)
        {
            DrinkRecipe recipe = null;
            try
            {
                recipe = await _context.DrinkRecipes
                    .Include(r => r.Ingredient)
                    .Include(r => r.MenuItem)
                    .SingleOrDefaultAsync(x => x.MenuItemId == menuItemId && x.IngredientId == ingredientId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return recipe;
        }

        public async Task<int> GetRecipeCountByMenuItemAsync(int menuItemId)
        {
            int count = 0;
            try
            {
                count = await _context.DrinkRecipes
                    .CountAsync(r => r.MenuItemId == menuItemId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public async Task<int> GetRecipeCountByIngredientAsync(int ingredientId)
        {
            int count = 0;
            try
            {
                count = await _context.DrinkRecipes
                    .CountAsync(r => r.IngredientId == ingredientId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public async Task SaveRecipeAsync(DrinkRecipe recipe)
        {
            try
            {
                _context.DrinkRecipes.Add(recipe);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                if (sqlEx.Number == 547) // Foreign Key constraint error
                {
                    throw new InvalidOperationException(
                        "Không thể tạo công thức vì món ăn hoặc nguyên liệu không tồn tại. Vui lòng kiểm tra lại MenuItemId và IngredientId.",
                        ex);
                }
                throw new Exception($"Lỗi cơ sở dữ liệu: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi không xác định: {ex.Message}", ex);
            }
        }

        public async Task UpdateRecipeAsync(DrinkRecipe recipe)
        {
            try
            {
                _context.Entry<DrinkRecipe>(recipe).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateRecipeQuantityAsync(int recipeId, int minGram, int maxGram)
        {
            try
            {
                var recipe = await _context.DrinkRecipes
                    .SingleOrDefaultAsync(r => r.RecipeId == recipeId);

                if (recipe != null)
                {
                    recipe.QuantityMinGram = minGram;
                    recipe.QuantityMaxGram = maxGram;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteRecipeAsync(DrinkRecipe recipe)
        {
            try
            {
                var existingRecipe = await _context.DrinkRecipes
                    .SingleOrDefaultAsync(r => r.RecipeId == recipe.RecipeId);

                if (existingRecipe != null)
                {
                    _context.DrinkRecipes.Remove(existingRecipe);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task DeleteRecipesByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                var recipes = await _context.DrinkRecipes
                    .Where(r => r.MenuItemId == menuItemId)
                    .ToListAsync();

                if (recipes.Any())
                {
                    _context.DrinkRecipes.RemoveRange(recipes);
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
