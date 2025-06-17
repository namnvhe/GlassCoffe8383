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
    public class IngredientRepository : IIngredientRepository
    {
        public async Task<List<Ingredient>> GetAllAsync() =>
            await IngredientDAO.GetIngredientsAsync();

        public async Task<Ingredient> FindIngredientByIdAsync(int ingredientId) =>
            await IngredientDAO.FindIngredientByIdAsync(ingredientId);

        public async Task<Ingredient> FindIngredientByNameAsync(string ingredientName) =>
            await IngredientDAO.FindIngredientByNameAsync(ingredientName);

        public async Task<List<Ingredient>> GetAvailableIngredientsAsync() =>
            await IngredientDAO.GetAvailableIngredientsAsync();

        public async Task<List<Ingredient>> GetLowStockIngredientsAsync() =>
            await IngredientDAO.GetLowStockIngredientsAsync();

        public async Task<List<Ingredient>> GetOutOfStockIngredientsAsync() =>
            await IngredientDAO.GetOutOfStockIngredientsAsync();

        public async Task<List<Ingredient>> GetIngredientsByDrinkRecipeAsync(int drinkRecipeId) =>
            await IngredientDAO.GetIngredientsByDrinkRecipesAsync(drinkRecipeId);

        public async Task<List<Ingredient>> SearchIngredientsByNameAsync(string searchTerm) =>
            await IngredientDAO.SearchIngredientsByNameAsync(searchTerm);

        public async Task<decimal> GetTotalInventoryValueAsync() =>
            await IngredientDAO.GetTotalInventoryValueAsync();

        public async Task<int> GetTotalIngredientsCountAsync() =>
            await IngredientDAO.GetTotalIngredientsCountAsync();

        public async Task SaveIngredientAsync(Ingredient ingredient) =>
            await IngredientDAO.SaveIngredientAsync(ingredient);

        public async Task UpdateIngredientAsync(Ingredient ingredient) =>
            await IngredientDAO.UpdateIngredientAsync(ingredient);

        public async Task DeleteIngredientAsync(Ingredient ingredient) =>
            await IngredientDAO.DeleteIngredientAsync(ingredient);

        public async Task UpdateStockAsync(int ingredientId, int quantity) =>
            await IngredientDAO.UpdateStockAsync(ingredientId, quantity);

        public async Task UpdateUnitPriceAsync(int ingredientId, decimal newPrice) =>
            await IngredientDAO.UpdateUnitPriceAsync(ingredientId, newPrice);

        public async Task BulkUpdateStockAsync(Dictionary<int, int> ingredientQuantities) =>
            await IngredientDAO.BulkUpdateStockAsync(ingredientQuantities);
    }
}
