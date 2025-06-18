using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class DrinkRecipeRepository : IDrinkRecipeRepository
    {
        private readonly DrinkRecipeDAO _drinkRecipeDAO;

        public DrinkRecipeRepository(DrinkRecipeDAO drinkRecipeDAO)
        {
            _drinkRecipeDAO = drinkRecipeDAO ?? throw new ArgumentNullException(nameof(drinkRecipeDAO));
        }

        public async Task<List<DrinkRecipe>> GetAllAsync() =>
            await _drinkRecipeDAO.GetAllAsync();

        public async Task<DrinkRecipe> FindRecipeByIdAsync(int recipeId) =>
            await _drinkRecipeDAO.FindRecipeByIdAsync(recipeId);

        public async Task<DrinkRecipe> FindRecipeByMenuItemAndIngredientAsync(int menuItemId, int ingredientId) =>
            await _drinkRecipeDAO.FindRecipeByMenuItemAndIngredientAsync(menuItemId, ingredientId);

        public async Task<List<DrinkRecipe>> GetRecipesByMenuItemIdAsync(int menuItemId) =>
            await _drinkRecipeDAO.GetRecipesByMenuItemIdAsync(menuItemId);

        public async Task<List<DrinkRecipe>> GetRecipesByIngredientIdAsync(int ingredientId) =>
            await _drinkRecipeDAO.GetRecipesByIngredientIdAsync(ingredientId);

        public async Task<int> GetRecipeCountByMenuItemAsync(int menuItemId) =>
            await _drinkRecipeDAO.GetRecipeCountByMenuItemAsync(menuItemId);

        public async Task<int> GetRecipeCountByIngredientAsync(int ingredientId) =>
            await _drinkRecipeDAO.GetRecipeCountByIngredientAsync(ingredientId);

        public async Task SaveRecipeAsync(DrinkRecipe recipe) =>
            await _drinkRecipeDAO.SaveRecipeAsync(recipe);

        public async Task UpdateRecipeAsync(DrinkRecipe recipe) =>
            await _drinkRecipeDAO.UpdateRecipeAsync(recipe);

        public async Task UpdateRecipeQuantityAsync(int recipeId, int minGram, int maxGram) =>
            await _drinkRecipeDAO.UpdateRecipeQuantityAsync(recipeId, minGram, maxGram);

        public async Task DeleteRecipeAsync(DrinkRecipe recipe) =>
            await _drinkRecipeDAO.DeleteRecipeAsync(recipe);

        public async Task DeleteRecipesByMenuItemIdAsync(int menuItemId) =>
            await _drinkRecipeDAO.DeleteRecipesByMenuItemIdAsync(menuItemId);
    }
}
