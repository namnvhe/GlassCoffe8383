using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IIngredientRepository
    {
        Task<List<Ingredient>> GetAllAsync();
        Task<Ingredient> FindIngredientByIdAsync(int ingredientId);
        Task<Ingredient> FindIngredientByNameAsync(string ingredientName);
        Task SaveIngredientAsync(Ingredient ingredient);
        Task UpdateIngredientAsync(Ingredient ingredient);
        Task DeleteIngredientAsync(Ingredient ingredient);

        // quản lý tồn kho
        Task<List<Ingredient>> GetAvailableIngredientsAsync();
        Task<List<Ingredient>> GetLowStockIngredientsAsync();
        Task<List<Ingredient>> GetOutOfStockIngredientsAsync();

        // tìm kiếm và lọc
        Task<List<Ingredient>> GetIngredientsByDrinkRecipeAsync(int drinkRecipeId);
        Task<List<Ingredient>> SearchIngredientsByNameAsync(string searchTerm);

        // cập nhật đặc biệt
        Task UpdateStockAsync(int ingredientId, int quantity);
        Task UpdateUnitPriceAsync(int ingredientId, decimal newPrice);
        Task BulkUpdateStockAsync(Dictionary<int, int> ingredientQuantities);

        // thống kê
        Task<decimal> GetTotalInventoryValueAsync();
        Task<int> GetTotalIngredientsCountAsync();
    }
}
