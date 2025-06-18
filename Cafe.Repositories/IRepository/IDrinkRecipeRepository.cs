using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IDrinkRecipeRepository
    {
        Task<List<DrinkRecipe>> GetAllAsync();
        Task<DrinkRecipe> FindRecipeByIdAsync(int recipeId);
        Task<DrinkRecipe> FindRecipeByMenuItemAndIngredientAsync(int menuItemId, int ingredientId);
        Task SaveRecipeAsync(DrinkRecipe recipe);
        Task UpdateRecipeAsync(DrinkRecipe recipe);
        Task DeleteRecipeAsync(DrinkRecipe recipe);

        // tìm kiếm và lọc
        Task<List<DrinkRecipe>> GetRecipesByMenuItemIdAsync(int menuItemId);
        Task<List<DrinkRecipe>> GetRecipesByIngredientIdAsync(int ingredientId);

        // cập nhật đặc biệt
        Task UpdateRecipeQuantityAsync(int recipeId, int minGram, int maxGram);
        Task DeleteRecipesByMenuItemIdAsync(int menuItemId);

        // thống kê
        Task<int> GetRecipeCountByMenuItemAsync(int menuItemId);
        Task<int> GetRecipeCountByIngredientAsync(int ingredientId);
    }
}
