using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class DrinkTypeRepository : IDrinkTypeRepository
    {
        private readonly DrinkTypeDAO _drinkTypeDAO;

        public DrinkTypeRepository(DrinkTypeDAO drinkTypeDAO)
        {
            _drinkTypeDAO = drinkTypeDAO ?? throw new ArgumentNullException(nameof(drinkTypeDAO));
        }

        public async Task<ApiResponse<List<DrinkType>>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.GetDrinkTypesAsync(cancellationToken);

        public async Task<ApiResponse<DrinkType>> FindDrinkTypeByIdAsync(int drinkTypeId, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);

        public async Task<ApiResponse<DrinkType>> FindDrinkTypeByNameAsync(string typeName, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.FindDrinkTypeByNameAsync(typeName, cancellationToken);

        public async Task<ApiResponse<List<DrinkType>>> GetActiveDrinkTypesAsync(CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.GetActiveDrinkTypesAsync(cancellationToken);

        public async Task<ApiResponse<List<DrinkType>>> GetDrinkTypesWithMenuItemsAsync(CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.GetDrinkTypesWithMenuItemsAsync(cancellationToken);

        public async Task<ApiResponse<List<DrinkType>>> SearchDrinkTypesByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.SearchDrinkTypesByNameAsync(searchTerm, cancellationToken);

        public async Task<ApiResponse<List<DrinkType>>> GetPopularDrinkTypesAsync(int topCount = 5, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.GetPopularDrinkTypesAsync(topCount, cancellationToken);

        public async Task<ApiResponse<int>> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.GetMenuItemCountByDrinkTypeAsync(drinkTypeId, cancellationToken);

        public async Task<ApiResponse<bool>> CanDeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.CanDeleteDrinkTypeAsync(drinkTypeId, cancellationToken);

        public async Task<ApiResponse<DrinkType>> SaveDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.SaveDrinkTypeAsync(drinkType, cancellationToken);

        public async Task<ApiResponse<DrinkType>> UpdateDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.UpdateDrinkTypeAsync(drinkType, cancellationToken);

        public async Task<ApiResponse<bool>> UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.UpdateDrinkTypeStatusAsync(drinkTypeId, isActive, cancellationToken);

        public async Task<ApiResponse<bool>> DeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.DeleteDrinkTypeAsync(drinkTypeId, cancellationToken);

        public async Task<ApiResponse<bool>> UpdateDrinkTypeImageAsync(int drinkTypeId, string? imagePath, CancellationToken cancellationToken = default)
            => await _drinkTypeDAO.UpdateDrinkTypeImageAsync(drinkTypeId, imagePath, cancellationToken);
    }
}