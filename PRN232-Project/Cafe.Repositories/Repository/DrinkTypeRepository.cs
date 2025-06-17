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
    public class DrinkTypeRepository : IDrinkTypeRepository
    {
        public async Task<List<DrinkType>> GetAllAsync() =>
            await DrinkTypeDAO.GetDrinkTypesAsync();

        public async Task<DrinkType> FindDrinkTypeByIdAsync(int drinkTypeId) =>
            await DrinkTypeDAO.FindDrinkTypeByIdAsync(drinkTypeId);

        public async Task<DrinkType> FindDrinkTypeByNameAsync(string typeName) =>
            await DrinkTypeDAO.FindDrinkTypeByNameAsync(typeName);

        public async Task<List<DrinkType>> GetActiveDrinkTypesAsync() =>
            await DrinkTypeDAO.GetActiveDrinkTypesAsync();

        public async Task<List<DrinkType>> GetDrinkTypesWithMenuItemsAsync() =>
            await DrinkTypeDAO.GetDrinkTypesWithMenuItemsAsync();

        public async Task<List<DrinkType>> SearchDrinkTypesByNameAsync(string searchTerm) =>
            await DrinkTypeDAO.SearchDrinkTypesByNameAsync(searchTerm);

        public async Task<List<DrinkType>> GetPopularDrinkTypesAsync(int topCount = 5) =>
            await DrinkTypeDAO.GetPopularDrinkTypesAsync(topCount);

        public async Task<int> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId) =>
            await DrinkTypeDAO.GetMenuItemCountByDrinkTypeAsync(drinkTypeId);

        public async Task<bool> CanDeleteDrinkTypeAsync(int drinkTypeId) =>
            await DrinkTypeDAO.CanDeleteDrinkTypeAsync(drinkTypeId);

        public async Task SaveDrinkTypeAsync(DrinkType drinkType) =>
            await DrinkTypeDAO.SaveDrinkTypeAsync(drinkType);

        public async Task UpdateDrinkTypeAsync(DrinkType drinkType) =>
            await DrinkTypeDAO.UpdateDrinkTypeAsync(drinkType);

        public async Task UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive) =>
            await DrinkTypeDAO.UpdateDrinkTypeStatusAsync(drinkTypeId, isActive);

        public async Task DeleteDrinkTypeAsync(DrinkType drinkType) =>
            await DrinkTypeDAO.DeleteDrinkTypeAsync(drinkType);
    }
}
