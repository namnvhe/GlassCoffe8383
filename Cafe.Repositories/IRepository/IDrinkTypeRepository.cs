using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;

namespace Cafe.Repositories.IRepository
{
    public interface IDrinkTypeRepository
    {
        Task<ApiResponse<List<DrinkType>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<DrinkType>> FindDrinkTypeByIdAsync(int drinkTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse<DrinkType>> FindDrinkTypeByNameAsync(string typeName, CancellationToken cancellationToken = default);
        Task<ApiResponse<DrinkType>> SaveDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default);
        Task<ApiResponse<DrinkType>> UpdateDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> DeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> UpdateDrinkTypeImageAsync(int drinkTypeId, string? imagePath, CancellationToken cancellationToken = default);

        // Tìm kiếm và lọc
        Task<ApiResponse<List<DrinkType>>> GetActiveDrinkTypesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<List<DrinkType>>> GetDrinkTypesWithMenuItemsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse<List<DrinkType>>> SearchDrinkTypesByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<ApiResponse<List<DrinkType>>> GetPopularDrinkTypesAsync(int topCount = 5, CancellationToken cancellationToken = default);

        // Thống kê và validation
        Task<ApiResponse<int>> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse<bool>> CanDeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default);
    }
}