using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IDrinkTypeRepository
    {
        Task<List<DrinkType>> GetAllAsync();
        Task<DrinkType> FindDrinkTypeByIdAsync(int drinkTypeId);
        Task<DrinkType> FindDrinkTypeByNameAsync(string typeName);
        Task SaveDrinkTypeAsync(DrinkType drinkType);
        Task UpdateDrinkTypeAsync(DrinkType drinkType);
        Task DeleteDrinkTypeAsync(DrinkType drinkType);

        // tìm kiếm và lọc
        Task<List<DrinkType>> GetActiveDrinkTypesAsync();
        Task<List<DrinkType>> GetDrinkTypesWithMenuItemsAsync();
        Task<List<DrinkType>> SearchDrinkTypesByNameAsync(string searchTerm);
        Task<List<DrinkType>> GetPopularDrinkTypesAsync(int topCount = 5);

        // cập nhật đặc biệt
        Task UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive);

        // thống kê và validation
        Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId);
        Task<bool> CanDeleteDrinkTypeAsync(int drinkTypeId);
    }
}
