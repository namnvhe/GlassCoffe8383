using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DrinkTypeController : ControllerBase
    {
        private readonly IDrinkTypeRepository _drinkTypeRepository;

        public DrinkTypeController(IDrinkTypeRepository drinkTypeRepository)
        {
            _drinkTypeRepository = drinkTypeRepository ?? throw new ArgumentNullException(nameof(drinkTypeRepository));
        }

        // Lấy danh sách tất cả loại đồ uống
        [HttpGet("get-all-drinktypes")]
        public async Task<IActionResult> GetAllDrinkTypes(CancellationToken cancellationToken = default)
        {
            var response = await _drinkTypeRepository.GetAllAsync(cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 404, response);
            }

            var drinkTypeResponses = response.Data.Select(dt => new DrinkTypeResponse
            {
                DrinkTypeId = dt.DrinkTypeId,
                TypeName = dt.TypeName,
                IsActive = dt.IsActive,
                ImagePath = dt.ImagePath,
                MenuItemCount = dt.MenuItems?.Count ?? 0,
                MenuItems = dt.MenuItems?.Select(mi => new MenuItemSummary
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    Price = mi.Price,
                    IsAvailable = mi.IsAvailable,
                    ImageUrl = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl ?? string.Empty
                }).ToList() ?? new List<MenuItemSummary>()
            }).ToList();

            return Ok(new ApiResponse<List<DrinkTypeResponse>>
            {
                IsSuccess = true,
                Message = "Lấy danh sách loại đồ uống thành công",
                Data = drinkTypeResponses
            });
        }

        // Lấy danh sách loại đồ uống đang hoạt động
        [HttpGet("get-drinktype-active")]
        public async Task<IActionResult> GetActiveDrinkTypes(CancellationToken cancellationToken = default)
        {
            var response = await _drinkTypeRepository.GetActiveDrinkTypesAsync(cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 404, response);
            }

            var drinkTypeResponses = response.Data.Select(dt => new DrinkTypeResponse
            {
                DrinkTypeId = dt.DrinkTypeId,
                TypeName = dt.TypeName,
                IsActive = dt.IsActive,
                ImagePath = dt.ImagePath,
                MenuItemCount = dt.MenuItems?.Count ?? 0,
                MenuItems = dt.MenuItems?.Where(mi => mi.IsAvailable).Select(mi => new MenuItemSummary
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    Price = mi.Price,
                    IsAvailable = mi.IsAvailable,
                    ImageUrl = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl ?? string.Empty
                }).ToList() ?? new List<MenuItemSummary>()
            }).ToList();

            return Ok(new ApiResponse<List<DrinkTypeResponse>>
            {
                IsSuccess = true,
                Message = "Lấy danh sách loại đồ uống hoạt động thành công",
                Data = drinkTypeResponses
            });
        }

        // Lấy thông tin loại đồ uống theo ID
        [HttpGet("get-drinktype-by-id/{drinkTypeId:int}")]
        public async Task<IActionResult> GetDrinkTypeById(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            var response = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 404, response);
            }

            var drinkTypeResponse = new DrinkTypeDetailResponse
            {
                DrinkTypeId = response.Data.DrinkTypeId,
                TypeName = response.Data.TypeName,
                IsActive = response.Data.IsActive,
                ImagePath = response.Data.ImagePath,
                MenuItems = response.Data.MenuItems?.Select(mi => new MenuItemDetailResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    Price = mi.Price,
                    IsAvailable = mi.IsAvailable,
                    MenuItemImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl ?? string.Empty
                }).ToList() ?? new List<MenuItemDetailResponse>()
            };

            return Ok(new ApiResponse<DrinkTypeDetailResponse>
            {
                IsSuccess = true,
                Message = "Lấy thông tin loại đồ uống thành công",
                Data = drinkTypeResponse
            });
        }

        // Tìm kiếm loại đồ uống theo tên
        [HttpGet("search-drinktype-by-name")]
        public async Task<IActionResult> SearchDrinkTypes([FromQuery] string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Từ khóa tìm kiếm không được để trống"
                });
            }

            var response = await _drinkTypeRepository.SearchDrinkTypesByNameAsync(searchTerm, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 404, response);
            }

            var drinkTypeResponses = response.Data.Select(dt => new DrinkTypeResponse
            {
                DrinkTypeId = dt.DrinkTypeId,
                TypeName = dt.TypeName,
                IsActive = dt.IsActive,
                ImagePath = dt.ImagePath,
                MenuItemCount = dt.MenuItems?.Count ?? 0,
                MenuItems = dt.MenuItems?.Select(mi => new MenuItemSummary
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    Price = mi.Price,
                    IsAvailable = mi.IsAvailable,
                    ImageUrl = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl ?? string.Empty
                }).ToList() ?? new List<MenuItemSummary>()
            }).ToList();

            return Ok(new ApiResponse<List<DrinkTypeResponse>>
            {
                IsSuccess = true,
                Message = $"Tìm thấy {drinkTypeResponses.Count} loại đồ uống",
                Data = drinkTypeResponses
            });
        }

        // Lấy danh sách loại đồ uống phổ biến
        [HttpGet("get-popular-drinktype")]
        public async Task<IActionResult> GetPopularDrinkTypes([FromQuery] int topCount = 5, CancellationToken cancellationToken = default)
        {
            var response = await _drinkTypeRepository.GetPopularDrinkTypesAsync(topCount, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 404, response);
            }

            var drinkTypeResponses = response.Data.Select(dt => new PopularDrinkTypeResponse
            {
                DrinkTypeId = dt.DrinkTypeId,
                TypeName = dt.TypeName,
                IsActive = dt.IsActive,
                ImagePath = dt.ImagePath,
                TotalOrders = dt.MenuItems?.SelectMany(mi => mi.OrderItems).Sum(oi => oi.Quantity) ?? 0,
                MenuItemCount = dt.MenuItems?.Count ?? 0
            }).ToList();

            return Ok(new ApiResponse<List<PopularDrinkTypeResponse>>
            {
                IsSuccess = true,
                Message = "Lấy danh sách loại đồ uống phổ biến thành công",
                Data = drinkTypeResponses
            });
        }

        // Tạo loại đồ uống mới
        [HttpPost("create-new-drinktype")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDrinkType([FromBody] CreateDrinkTypeRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đầu vào không hợp lệ",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var trimmedTypeName = request.TypeName.Trim();
            if (string.IsNullOrEmpty(trimmedTypeName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Tên loại đồ uống không được để trống sau khi loại bỏ khoảng trắng"
                });
            }

            var existingDrinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByNameAsync(trimmedTypeName, cancellationToken);
            if (existingDrinkTypeResponse.IsSuccess && existingDrinkTypeResponse.Data != null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Tên loại đồ uống đã tồn tại"
                });
            }

            var newDrinkType = new DrinkType
            {
                TypeName = trimmedTypeName,
                IsActive = true,
                ImagePath = request.ImagePath
            };

            var response = await _drinkTypeRepository.SaveDrinkTypeAsync(newDrinkType, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 400, response);
            }

            return CreatedAtAction(nameof(GetDrinkTypeById), new { drinkTypeId = response.Data.DrinkTypeId },
                new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Tạo loại đồ uống thành công",
                    Data = new { DrinkTypeId = response.Data.DrinkTypeId }
                });
        }

        // Cập nhật loại đồ uống
        [HttpPut("update-drinktype/{drinkTypeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDrinkType(int drinkTypeId, [FromBody] UpdateDrinkTypeRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đầu vào không hợp lệ",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var trimmedTypeName = request.TypeName.Trim();
            if (string.IsNullOrEmpty(trimmedTypeName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Tên loại đồ uống không được để trống sau khi loại bỏ khoảng trắng"
                });
            }

            var drinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!drinkTypeResponse.IsSuccess || drinkTypeResponse.Data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy loại đồ uống"
                });
            }

            var existingDrinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByNameAsync(trimmedTypeName, cancellationToken);
            if (existingDrinkTypeResponse.IsSuccess && existingDrinkTypeResponse.Data != null && existingDrinkTypeResponse.Data.DrinkTypeId != drinkTypeId)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Tên loại đồ uống đã tồn tại"
                });
            }

            var drinkType = drinkTypeResponse.Data;
            drinkType.TypeName = trimmedTypeName;
            drinkType.ImagePath = request.ImagePath;

            var updateResponse = await _drinkTypeRepository.UpdateDrinkTypeAsync(drinkType, cancellationToken);
            if (!updateResponse.IsSuccess)
            {
                return StatusCode(updateResponse.Errors.Any() ? 500 : 400, updateResponse);
            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cập nhật loại đồ uống thành công"
            });
        }

        // Cập nhật hình ảnh loại đồ uống
        [HttpPut("update-drinktype-image/{drinkTypeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDrinkTypeImage(int drinkTypeId, [FromBody] UpdateDrinkTypeImageRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đầu vào không hợp lệ",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var drinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!drinkTypeResponse.IsSuccess || drinkTypeResponse.Data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy loại đồ uống"
                });
            }

            var response = await _drinkTypeRepository.UpdateDrinkTypeImageAsync(drinkTypeId, request.ImagePath, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 400, response);
            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Cập nhật hình ảnh loại đồ uống thành công"
            });
        }

        // Cập nhật trạng thái loại đồ uống
        [HttpPut("update-drinktype-status/{drinkTypeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDrinkTypeStatus(int drinkTypeId, [FromBody] UpdateStatusRequest request, CancellationToken cancellationToken = default)
        {
            var drinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!drinkTypeResponse.IsSuccess || drinkTypeResponse.Data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy loại đồ uống"
                });
            }

            var response = await _drinkTypeRepository.UpdateDrinkTypeStatusAsync(drinkTypeId, request.IsActive, cancellationToken);
            if (!response.IsSuccess)
            {
                return StatusCode(response.Errors.Any() ? 500 : 400, response);
            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = request.IsActive ? "Loại đồ uống đã được kích hoạt" : "Loại đồ uống đã được vô hiệu hóa"
            });
        }

        // Xóa loại đồ uống
        [HttpDelete("delete-drinktype/{drinkTypeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDrinkType(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            var drinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!drinkTypeResponse.IsSuccess || drinkTypeResponse.Data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy loại đồ uống"
                });
            }

            var canDeleteResponse = await _drinkTypeRepository.CanDeleteDrinkTypeAsync(drinkTypeId, cancellationToken);
            if (!canDeleteResponse.IsSuccess || !canDeleteResponse.Data)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = canDeleteResponse.Message ?? "Không thể xóa loại đồ uống này vì đang có món ăn liên quan"
                });
            }

            var deleteResponse = await _drinkTypeRepository.DeleteDrinkTypeAsync(drinkTypeId, cancellationToken);
            if (!deleteResponse.IsSuccess)
            {
                return StatusCode(deleteResponse.Errors.Any() ? 500 : 400, deleteResponse);
            }

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Xóa loại đồ uống thành công"
            });
        }

        // Lấy thống kê loại đồ uống
        [HttpGet("get/{drinkTypeId:int}/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDrinkTypeStats(int drinkTypeId, CancellationToken cancellationToken = default)
        {
            var drinkTypeResponse = await _drinkTypeRepository.FindDrinkTypeByIdAsync(drinkTypeId, cancellationToken);
            if (!drinkTypeResponse.IsSuccess || drinkTypeResponse.Data == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy loại đồ uống"
                });
            }

            var menuItemCountResponse = await _drinkTypeRepository.GetMenuItemCountByDrinkTypeAsync(drinkTypeId, cancellationToken);
            if (!menuItemCountResponse.IsSuccess)
            {
                return StatusCode(menuItemCountResponse.Errors.Any() ? 500 : 400, menuItemCountResponse);
            }

            var canDeleteResponse = await _drinkTypeRepository.CanDeleteDrinkTypeAsync(drinkTypeId, cancellationToken);
            if (!canDeleteResponse.IsSuccess)
            {
                return StatusCode(canDeleteResponse.Errors.Any() ? 500 : 400, canDeleteResponse);
            }

            var stats = new DrinkTypeStatsResponse
            {
                DrinkTypeId = drinkTypeResponse.Data.DrinkTypeId,
                TypeName = drinkTypeResponse.Data.TypeName,
                IsActive = drinkTypeResponse.Data.IsActive,
                ImagePath = drinkTypeResponse.Data.ImagePath,
                MenuItemCount = menuItemCountResponse.Data,
                TotalOrders = drinkTypeResponse.Data.MenuItems?.SelectMany(mi => mi.OrderItems).Sum(oi => oi.Quantity) ?? 0,
                CanDelete = canDeleteResponse.Data
            };

            return Ok(new ApiResponse<DrinkTypeStatsResponse>
            {
                IsSuccess = true,
                Message = "Lấy thống kê loại đồ uống thành công",
                Data = stats
            });
        }
    }
}