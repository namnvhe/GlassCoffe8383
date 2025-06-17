using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly IngredientDAO _ingredientDAO;

        public IngredientRepository(IngredientDAO ingredientDAO)
        {
            _ingredientDAO = ingredientDAO ?? throw new ArgumentNullException(nameof(ingredientDAO));
        }

        public async Task<List<Ingredient>> GetAllAsync() =>
            await _ingredientDAO.GetIngredientsAsync();

        public async Task<Ingredient> FindIngredientByIdAsync(int ingredientId) =>
            await _ingredientDAO.FindIngredientByIdAsync(ingredientId);

        public async Task<Ingredient> FindIngredientByNameAsync(string ingredientName) =>
            await _ingredientDAO.FindIngredientByNameAsync(ingredientName);

        public async Task<List<Ingredient>> GetAvailableIngredientsAsync() =>
            await _ingredientDAO.GetAvailableIngredientsAsync();

        public async Task<List<Ingredient>> GetLowStockIngredientsAsync() =>
            await _ingredientDAO.GetLowStockIngredientsAsync();

        public async Task<List<Ingredient>> GetOutOfStockIngredientsAsync() =>
            await _ingredientDAO.GetOutOfStockIngredientsAsync();

        public async Task<List<Ingredient>> GetIngredientsByDrinkRecipeAsync(int drinkRecipeId) =>
            await _ingredientDAO.GetIngredientsByDrinkRecipesAsync(drinkRecipeId);

        public async Task<List<Ingredient>> SearchIngredientsByNameAsync(string searchTerm) =>
            await _ingredientDAO.SearchIngredientsByNameAsync(searchTerm);

        public async Task<decimal> GetTotalInventoryValueAsync() =>
            await _ingredientDAO.GetTotalInventoryValueAsync();

        public async Task<int> GetTotalIngredientsCountAsync() =>
            await _ingredientDAO.GetTotalIngredientsCountAsync();

        public async Task SaveIngredientAsync(Ingredient ingredient) =>
            await _ingredientDAO.SaveIngredientAsync(ingredient);

        public async Task UpdateIngredientAsync(Ingredient ingredient) =>
            await _ingredientDAO.UpdateIngredientAsync(ingredient);

        public async Task DeleteIngredientAsync(Ingredient ingredient) =>
            await _ingredientDAO.DeleteIngredientAsync(ingredient);

        public async Task UpdateStockAsync(int ingredientId, int quantity) =>
            await _ingredientDAO.UpdateStockAsync(ingredientId, quantity);

        public async Task UpdateUnitPriceAsync(int ingredientId, decimal newPrice) =>
            await _ingredientDAO.UpdateUnitPriceAsync(ingredientId, newPrice);

        public async Task BulkUpdateStockAsync(Dictionary<int, int> ingredientQuantities) =>
            await _ingredientDAO.BulkUpdateStockAsync(ingredientQuantities);
    }
}