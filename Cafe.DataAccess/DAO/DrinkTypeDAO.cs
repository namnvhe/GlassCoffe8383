using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace Cafe.DataAccess.DAO
{
    public class DrinkTypeDAO
    {
        private readonly CoffeManagerContext _context;

        public DrinkTypeDAO(CoffeManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ApiResponse<List<DrinkType>>> GetDrinkTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var listDrinkTypes = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                    .OrderBy(dt => dt.TypeName)
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = true,
                    Data = listDrinkTypes,
                    Message = "Danh sách loại đồ uống đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<List<DrinkType>>> GetActiveDrinkTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var activeDrinkTypes = await _context.DrinkTypes
                    .Where(dt => dt.IsActive)
                    .Include(dt => dt.MenuItems.Where(mi => mi.IsAvailable))
                    .OrderBy(dt => dt.TypeName)
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = true,
                    Data = activeDrinkTypes,
                    Message = "Danh sách loại đồ uống đang hoạt động đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách loại đồ uống đang hoạt động",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<List<DrinkType>>> GetDrinkTypesWithMenuItemsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var drinkTypesWithItems = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                    .Where(dt => dt.MenuItems.Any())
                    .OrderBy(dt => dt.TypeName)
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = true,
                    Data = drinkTypesWithItems,
                    Message = "Danh sách loại đồ uống có menu items đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách loại đồ uống có menu items",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<DrinkType>> FindDrinkTypeByIdAsync(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var drinkType = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                        .ThenInclude(mi => mi.MenuItemImages.Where(img => img.IsMainImage))
                    .SingleOrDefaultAsync(x => x.DrinkTypeId == drinkTypeId, cancellationToken);

                if (drinkType == null)
                {
                    return new ApiResponse<DrinkType>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                return new ApiResponse<DrinkType>
                {
                    IsSuccess = true,
                    Data = drinkType,
                    Message = "Loại đồ uống đã được tìm thấy"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm loại đồ uống theo ID",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<DrinkType>> FindDrinkTypeByNameAsync(string typeName, CancellationToken cancellationToken = default)
        {
            try
            {
                var trimmedTypeName = typeName.Trim().ToLower();
                var drinkType = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                    .SingleOrDefaultAsync(x => EF.Functions.Collate(x.TypeName, "SQL_Latin1_General_CP1_CI_AS").ToLower() == trimmedTypeName, cancellationToken);

                if (drinkType == null)
                {
                    return new ApiResponse<DrinkType>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                return new ApiResponse<DrinkType>
                {
                    IsSuccess = true,
                    Data = drinkType,
                    Message = "Loại đồ uống đã được tìm thấy"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm loại đồ uống theo tên",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<List<DrinkType>>> SearchDrinkTypesByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            try
            {
                var trimmedSearchTerm = searchTerm.Trim().ToLower();
                var drinkTypes = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                    .Where(dt => EF.Functions.Collate(dt.TypeName, "SQL_Latin1_General_CP1_CI_AS").ToLower().Contains(trimmedSearchTerm))
                    .OrderBy(dt => dt.TypeName)
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = true,
                    Data = drinkTypes,
                    Message = "Danh sách loại đồ uống đã được tìm thấy"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm loại đồ uống theo tên",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<int>> GetMenuItemCountByDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _context.MenuItems
                    .CountAsync(mi => mi.DrinkTypeId == drinkTypeId, cancellationToken);

                return new ApiResponse<int>
                {
                    IsSuccess = true,
                    Data = count,
                    Message = "Số lượng menu items đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi đếm số lượng menu items",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<List<DrinkType>>> GetPopularDrinkTypesAsync(int topCount = 5, CancellationToken cancellationToken = default)
        {
            try
            {
                var popularTypes = await _context.DrinkTypes
                    .Include(dt => dt.MenuItems)
                        .ThenInclude(mi => mi.OrderItems)
                    .OrderByDescending(dt => dt.MenuItems
                        .SelectMany(mi => mi.OrderItems)
                        .Sum(oi => oi.Quantity))
                    .Take(topCount)
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = true,
                    Data = popularTypes,
                    Message = "Danh sách loại đồ uống phổ biến đã được lấy thành công"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<List<DrinkType>>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách loại đồ uống phổ biến",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<DrinkType>> SaveDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default)
        {
            try
            {
                drinkType.TypeName = drinkType.TypeName.Trim();
                var trimmedTypeName = drinkType.TypeName.ToLower();
                var existingDrinkType = await _context.DrinkTypes
                    .SingleOrDefaultAsync(dt => EF.Functions.Collate(dt.TypeName, "SQL_Latin1_General_CP1_CI_AS").ToLower() == trimmedTypeName, cancellationToken);
                if (existingDrinkType != null)
                {
                    return new ApiResponse<DrinkType>
                    {
                        IsSuccess = false,
                        Message = "Tên loại đồ uống đã tồn tại",
                        Errors = new List<string> { "Tên loại đồ uống này đã được sử dụng" }
                    };
                }

                _context.DrinkTypes.Add(drinkType);
                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<DrinkType>
                {
                    IsSuccess = true,
                    Data = drinkType,
                    Message = "Loại đồ uống đã được tạo thành công"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo loại đồ uống",
                    Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<DrinkType>> UpdateDrinkTypeAsync(DrinkType drinkType, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingDrinkType = await _context.DrinkTypes
                    .SingleOrDefaultAsync(dt => dt.DrinkTypeId == drinkType.DrinkTypeId, cancellationToken);
                if (existingDrinkType == null)
                {
                    return new ApiResponse<DrinkType>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                drinkType.TypeName = drinkType.TypeName.Trim();
                var trimmedTypeName = drinkType.TypeName.ToLower();
                if (!existingDrinkType.TypeName.ToLower().Equals(trimmedTypeName))
                {
                    var nameExists = await _context.DrinkTypes
                        .AnyAsync(dt => EF.Functions.Collate(dt.TypeName, "SQL_Latin1_General_CP1_CI_AS").ToLower() == trimmedTypeName && dt.DrinkTypeId != drinkType.DrinkTypeId, cancellationToken);
                    if (nameExists)
                    {
                        return new ApiResponse<DrinkType>
                        {
                            IsSuccess = false,
                            Message = "Tên loại đồ uống đã tồn tại",
                            Errors = new List<string> { "Tên loại đồ uống này đã được sử dụng bởi loại khác" }
                        };
                    }
                }

                existingDrinkType.TypeName = drinkType.TypeName;
                existingDrinkType.IsActive = drinkType.IsActive;
                existingDrinkType.ImagePath = drinkType.ImagePath;

                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<DrinkType>
                {
                    IsSuccess = true,
                    Data = existingDrinkType,
                    Message = "Loại đồ uống đã được cập nhật thành công"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật loại đồ uống",
                    Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<DrinkType>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrinkTypeStatusAsync(int drinkTypeId, bool isActive, CancellationToken cancellationToken = default)
        {
            try
            {
                var drinkType = await _context.DrinkTypes
                    .SingleOrDefaultAsync(dt => dt.DrinkTypeId == drinkTypeId, cancellationToken);
                if (drinkType == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                drinkType.IsActive = isActive;
                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = isActive ? "Loại đồ uống đã được kích hoạt" : "Loại đồ uống đã được vô hiệu hóa"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thay đổi trạng thái loại đồ uống",
                    Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thay đổi trạng thái loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> UpdateDrinkTypeImageAsync(int drinkTypeId, string? imagePath, CancellationToken cancellationToken = default)
        {
            try
            {
                var drinkType = await _context.DrinkTypes
                    .SingleOrDefaultAsync(dt => dt.DrinkTypeId == drinkTypeId, cancellationToken);
                if (drinkType == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                drinkType.ImagePath = imagePath;
                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Ảnh loại đồ uống đã được cập nhật thành công"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật ảnh loại đồ uống",
                    Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật ảnh loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var drinkType = await _context.DrinkTypes
                    .SingleOrDefaultAsync(dt => dt.DrinkTypeId == drinkTypeId, cancellationToken);
                if (drinkType == null)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy loại đồ uống",
                        Errors = new List<string> { "Drink type not found" }
                    };
                }

                var hasMenuItems = await _context.MenuItems
                    .AnyAsync(mi => mi.DrinkTypeId == drinkTypeId, cancellationToken);
                if (hasMenuItems)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa loại đồ uống vì vẫn còn menu items liên quan",
                        Errors = new List<string> { "Drink type has associated menu items" }
                    };
                }

                _context.DrinkTypes.Remove(drinkType);
                await _context.SaveChangesAsync(cancellationToken);

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = true,
                    Message = "Loại đồ uống đã được xóa thành công"
                };
            }
            catch (DbUpdateException dbEx)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa loại đồ uống",
                    Errors = new List<string> { dbEx.InnerException?.Message ?? dbEx.Message }
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        public async Task<ApiResponse<bool>> CanDeleteDrinkTypeAsync(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var hasMenuItems = await _context.MenuItems
                    .AnyAsync(mi => mi.DrinkTypeId == drinkTypeId, cancellationToken);

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Data = !hasMenuItems,
                    Message = hasMenuItems
                        ? "Loại đồ uống không thể xóa vì có menu items liên quan"
                        : "Loại đồ uống có thể xóa"
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi kiểm tra khả năng xóa loại đồ uống",
                    Errors = new List<string> { e.Message }
                };
            }
        }
    }
}