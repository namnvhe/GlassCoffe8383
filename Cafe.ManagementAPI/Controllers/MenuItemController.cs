using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cafe.BusinessObjects.Models.Request;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IDrinkTypeRepository _drinkTypeRepository;
        private readonly CultureInfo _cultureInfo = new CultureInfo("vi-VN"); // định dạng tiền tệ Việt Nam

        public MenuItemController(IMenuItemRepository menuItemRepository, IDrinkTypeRepository drinkTypeRepository)
        {
            _menuItemRepository = menuItemRepository ?? throw new ArgumentNullException(nameof(menuItemRepository));
            _drinkTypeRepository = drinkTypeRepository ?? throw new ArgumentNullException(nameof(drinkTypeRepository));
        }

        // Lấy danh sách tất cả món ăn
        [HttpGet("get-all-menu-items")]
        public async Task<IActionResult> GetAllMenuItems()
        {
            try
            {
                var menuItems = await _menuItemRepository.GetAllAsync();

                var menuItemResponses = menuItems.Select(mi => new MenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeId = mi.DrinkTypeId,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    StockQuantity = mi.StockQuantity,
                    MinStockLevel = mi.MinStockLevel,
                    IsAvailable = mi.IsAvailable,
                    StockStatus = GetStockStatus(mi.StockQuantity, mi.MinStockLevel),
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl,
                    TotalOrders = mi.OrderItems?.Sum(oi => oi.Quantity) ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách món ăn thành công",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Lấy danh sách món ăn có sẵn
        [HttpGet("get-available-menu-item")]
        public async Task<IActionResult> GetAvailableMenuItems()
        {
            try
            {
                var menuItems = await _menuItemRepository.GetAvailableMenuItemsAsync();

                var menuItemResponses = menuItems.Select(mi => new MenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeId = mi.DrinkTypeId,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    StockQuantity = mi.StockQuantity,
                    IsAvailable = mi.IsAvailable,
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl,
                    ToppingsAvailable = mi.Toppings?.Where(t => t.IsAvailable).Select(t => new ToppingSummary
                    {
                        ToppingId = t.ToppingId,
                        ToppingName = t.Name,
                        Price = t.Price
                    }).ToList() ?? new List<ToppingSummary>()
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách món ăn có sẵn thành công",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách món ăn có sẵn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Lấy thông tin món ăn theo ID
        [HttpGet("get-menu-item-by-id/{menuItemId:int}")]
        public async Task<IActionResult> GetMenuItemById(int menuItemId)
        {
            try
            {
                if (menuItemId <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "ID món ăn không hợp lệ"
                    });
                }

                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                var menuItemResponse = new MenuItemDetailResponse
                {
                    MenuItemId = menuItem.MenuItemId,
                    Name = menuItem.Name,
                    MenuItemImage = menuItem.MenuItemImage ?? "default-menu-item.jpg",
                    Description = menuItem.Description,
                    Price = menuItem.Price,
                    DrinkTypeId = menuItem.DrinkTypeId,
                    DrinkTypeName = menuItem.DrinkType?.TypeName ?? "Unknown",
                    StockQuantity = menuItem.StockQuantity,
                    MinStockLevel = menuItem.MinStockLevel,
                    IsAvailable = menuItem.IsAvailable,
                    StockStatus = GetStockStatus(menuItem.StockQuantity, menuItem.MinStockLevel),
                    Images = menuItem.MenuItemImages?.Select(img => new MenuItemImageResponse
                    {
                        ImageId = img.ImageId,
                        ImagePath = img.ImageUrl,
                        IsMainImage = img.IsMainImage,
                        DisplayOrder = img.DisplayOrder
                    }).OrderBy(img => img.DisplayOrder).ToList() ?? new List<MenuItemImageResponse>(),
                    Toppings = menuItem.Toppings?.Where(t => t.IsAvailable).Select(t => new ToppingDetailResponse
                    {
                        ToppingId = t.ToppingId,
                        ToppingName = t.Name,
                        Price = t.Price,
                        IsAvailable = t.IsAvailable
                    }).ToList() ?? new List<ToppingDetailResponse>(),
                    DrinkRecipes = menuItem.DrinkRecipes?.Select(dr => new DrinkRecipeDetailResponse
                    {
                        RecipeId = dr.RecipeId,
                        IngredientId = dr.IngredientId,
                        IngredientName = dr.Ingredient?.Name ?? "Unknown",
                        QuantityMinGram = dr.QuantityMinGram,
                        QuantityMaxGram = dr.QuantityMaxGram
                    }).ToList() ?? new List<DrinkRecipeDetailResponse>()
                };

                return Ok(new ApiResponse<MenuItemDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin món ăn thành công",
                    Data = menuItemResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Lấy món ăn theo loại đồ uống
        [HttpGet("get-menu-item-by-drink-type/{drinkTypeId:int}")]
        public async Task<IActionResult> GetMenuItemsByDrinkType(int drinkTypeId)
        {
            try
            {
                if (drinkTypeId <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "ID loại đồ uống không hợp lệ"
                    });
                }

                var menuItems = await _menuItemRepository.GetMenuItemsByDrinkTypeIdAsync(drinkTypeId);

                var menuItemResponses = menuItems.Select(mi => new MenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeId = mi.DrinkTypeId,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    IsAvailable = mi.IsAvailable,
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách món ăn theo loại thành công",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách món ăn theo loại: {0}",
                    Data = ex.Message
                });
            }
        }

        // Tìm kiếm món ăn theo tên
        [HttpGet("search")]
        public async Task<IActionResult> SearchMenuItems([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Từ khóa tìm kiếm không được để trống"
                    });
                }

                var menuItems = await _menuItemRepository.SearchMenuItemsByNameAsync(searchTerm);

                var menuItemResponses = menuItems.Select(mi => new MenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeId = mi.DrinkTypeId,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    IsAvailable = mi.IsAvailable,
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {menuItemResponses.Count} món ăn",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Lấy món ăn theo khoảng giá
        [HttpGet("get-menu-item-by-price-range")]
        public async Task<IActionResult> GetMenuItemsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Khoảng giá không hợp lệ"
                    });
                }

                var menuItems = await _menuItemRepository.GetMenuItemsByPriceRangeAsync(minPrice, maxPrice);

                var menuItemResponses = menuItems.Select(mi => new MenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeId = mi.DrinkTypeId,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    IsAvailable = mi.IsAvailable,
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {menuItemResponses.Count} món ăn trong khoảng giá {minPrice.ToString("C", _cultureInfo)} - {maxPrice.ToString("C", _cultureInfo)}",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy món ăn theo khoảng giá: {0}",
                    Data = ex.Message
                });
            }
        }

        // Lấy món ăn phổ biến
        [HttpGet("popular-item")]
        public async Task<IActionResult> GetPopularMenuItems([FromQuery] int topCount = 10)
        {
            try
            {
                if (topCount <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Số lượng món ăn phải lớn hơn 0"
                    });
                }

                var menuItems = await _menuItemRepository.GetPopularMenuItemsAsync(topCount);

                var menuItemResponses = menuItems.Select(mi => new PopularMenuItemResponse
                {
                    MenuItemId = mi.MenuItemId,
                    Name = mi.Name,
                    MenuItemImage = mi.MenuItemImage ?? "default-menu-item.jpg",
                    Description = mi.Description,
                    Price = mi.Price,
                    DrinkTypeName = mi.DrinkType?.TypeName ?? "Unknown",
                    TotalOrders = mi.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    MainImage = mi.MenuItemImages?.FirstOrDefault(img => img.IsMainImage)?.ImageUrl
                }).ToList();

                return Ok(new ApiResponse<List<PopularMenuItemResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách món ăn phổ biến thành công",
                    Data = menuItemResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách món ăn phổ biến: {0}",
                    Data = ex.Message
                });
            }
        }

        // Tạo món ăn mới
        [HttpPost("create-new-menu-item")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemRequest request)
        {
            try
            {
                // Kiểm tra validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Dữ liệu đầu vào không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                // Kiểm tra DrinkType tồn tại TRƯỚC khi kiểm tra tên món ăn
                var drinkType = await _drinkTypeRepository.FindDrinkTypeByIdAsync(request.DrinkTypeId);
                if (drinkType == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = $"Loại đồ uống với ID {request.DrinkTypeId} không tồn tại"
                    });
                }

                // Kiểm tra tên món ăn đã tồn tại
                var existingMenuItem = await _menuItemRepository.FindMenuItemByNameAsync(request.Name);
                if (existingMenuItem != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên món ăn đã tồn tại"
                    });
                }

                // Tạo MenuItem mới
                var newMenuItem = new MenuItem
                {
                    Name = request.Name,
                    MenuItemImage = request.MenuItemImage ?? "default-menu-item.jpg",
                    Description = request.Description,
                    Price = request.Price,
                    DrinkTypeId = request.DrinkTypeId, // Đảm bảo ID này hợp lệ
                    StockQuantity = request.StockQuantity,
                    MinStockLevel = request.MinStockLevel,
                    IsAvailable = request.IsAvailable
                };

                await _menuItemRepository.SaveMenuItemAsync(newMenuItem);

                return CreatedAtAction(nameof(GetMenuItemById),
                    new { menuItemId = newMenuItem.MenuItemId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo món ăn thành công",
                        Data = new { MenuItemId = newMenuItem.MenuItemId }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = $"Có lỗi xảy ra khi tạo món ăn: {ex.Message}",
                    Data = ex.InnerException?.Message
                });
            }
        }

        // Cập nhật món ăn
        [HttpPut("update-item/{menuItemId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, [FromBody] UpdateMenuItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Dữ liệu đầu vào không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                var existingMenuItem = await _menuItemRepository.FindMenuItemByNameAsync(request.Name);
                if (existingMenuItem != null && existingMenuItem.MenuItemId != menuItemId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên món ăn đã tồn tại"
                    });
                }

                var drinkType = await _drinkTypeRepository.FindDrinkTypeByIdAsync(request.DrinkTypeId);
                if (drinkType == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Loại đồ uống không tồn tại"
                    });
                }

                menuItem.Name = request.Name;
                menuItem.MenuItemImage = request.MenuItemImage ?? menuItem.MenuItemImage;
                menuItem.Description = request.Description;
                menuItem.Price = request.Price;
                menuItem.DrinkTypeId = request.DrinkTypeId;
                menuItem.StockQuantity = request.StockQuantity;
                menuItem.MinStockLevel = request.MinStockLevel;
                menuItem.IsAvailable = request.IsAvailable;

                await _menuItemRepository.UpdateMenuItemAsync(menuItem);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Cập nhật trạng thái có sẵn
        [HttpPatch("update/{menuItemId:int}/availability")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItemAvailability(int menuItemId, [FromBody] UpdateAvailabilityRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Dữ liệu đầu vào không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                if (menuItemId <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "ID món ăn không hợp lệ"
                    });
                }

                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                await _menuItemRepository.UpdateMenuItemAvailabilityAsync(menuItemId, request.IsAvailable);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Đã {(request.IsAvailable ? "kích hoạt" : "vô hiệu hóa")} món ăn"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Cập nhật giá món ăn
        [HttpPatch("update/{menuItemId:int}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMenuItemPrice(int menuItemId, [FromBody] UpdateMenuItemPriceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Dữ liệu đầu vào không hợp lệ",
                        Data = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                if (menuItemId <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "ID món ăn không hợp lệ"
                    });
                }

                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                await _menuItemRepository.UpdateMenuItemPriceAsync(menuItemId, request.Price);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật giá món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật giá món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        // Xóa món ăn
        [HttpDelete("delete-item/{menuItemId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMenuItem(int menuItemId)
        {
            try
            {
                if (menuItemId <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "ID món ăn không hợp lệ"
                    });
                }

                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                if (menuItem.OrderItems != null && menuItem.OrderItems.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa món ăn này vì đã có đơn hàng"
                    });
                }

                await _menuItemRepository.DeleteMenuItemAsync(menuItem);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa món ăn: {0}",
                    Data = ex.Message
                });
            }
        }

        private string GetStockStatus(int stockQuantity, int? minStockLevel)
        {
            if (stockQuantity == 0) return "Hết hàng";
            if (minStockLevel.HasValue && stockQuantity <= minStockLevel.Value) return "Sắp hết";
            return "Có sẵn";
        }
    }
}