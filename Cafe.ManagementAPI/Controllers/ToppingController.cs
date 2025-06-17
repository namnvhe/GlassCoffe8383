using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cafe.BusinessObjects.Models.Request;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToppingController : ControllerBase
    {
        private readonly IToppingRepository _toppingRepository;
        private readonly ILogger<ToppingController> _logger;

        public ToppingController(IToppingRepository toppingRepository, ILogger<ToppingController> logger)
        {
            _toppingRepository = toppingRepository;
            _logger = logger;
        }

        // ==================== PUBLIC/CUSTOMER ENDPOINTS ====================

        /// <summary>
        /// Lấy danh sách tất cả topping - Public
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllToppings()
        {
            try
            {
                var toppings = await _toppingRepository.GetAllAsync();

                var toppingResponses = toppings.Select(t => new ToppingResponse
                {
                    ToppingId = t.ToppingId,
                    Name = t.Name,
                    ToppingImage = t.ToppingImage,
                    Price = t.Price,
                    StockQuantity = t.StockQuantity,
                    MinStockLevel = t.MinStockLevel,
                    IsAvailable = t.IsAvailable,
                    StockStatus = GetStockStatus(t.StockQuantity, t.MinStockLevel),
                    OrderCount = t.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<ToppingResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách topping thành công",
                    Data = toppingResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all toppings");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách topping"
                });
            }
        }

        /// <summary>
        /// Lấy danh sách topping có sẵn - Public
        /// </summary>
        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableToppings()
        {
            try
            {
                var toppings = await _toppingRepository.GetAvailableToppingsAsync();

                var toppingResponses = toppings.Select(t => new ToppingResponse
                {
                    ToppingId = t.ToppingId,
                    Name = t.Name,
                    ToppingImage = t.ToppingImage,
                    Price = t.Price,
                    StockQuantity = t.StockQuantity,
                    IsAvailable = t.IsAvailable,
                    StockStatus = GetStockStatus(t.StockQuantity, t.MinStockLevel),
                    OrderCount = t.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<ToppingResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách topping có sẵn thành công",
                    Data = toppingResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available toppings");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách topping có sẵn"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin topping theo ID - Public
        /// </summary>
        [HttpGet("{toppingId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToppingById(int toppingId)
        {
            try
            {
                var topping = await _toppingRepository.FindToppingByIdAsync(toppingId);

                if (topping == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy topping"
                    });
                }

                var toppingResponse = new ToppingDetailResponse
                {
                    ToppingId = topping.ToppingId,
                    ToppingName = topping.Name,
                    ToppingImage = topping.ToppingImage,
                    Price = topping.Price,
                    StockQuantity = topping.StockQuantity,
                    MinStockLevel = topping.MinStockLevel,
                    IsAvailable = topping.IsAvailable,
                    StockStatus = GetStockStatus(topping.StockQuantity, topping.MinStockLevel),
                    OrderCount = topping.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0,
                    MenuItems = topping.MenuItems?.Select(mi => new MenuItemSummary
                    {
                        MenuItemId = mi.MenuItemId,
                        Name = mi.Name,
                        Price = mi.Price,
                        IsAvailable = mi.IsAvailable
                    }).ToList()
                };

                return Ok(new ApiResponse<ToppingDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin topping thành công",
                    Data = toppingResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topping by ID: {ToppingId}", toppingId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin topping"
                });
            }
        }

        /// <summary>
        /// Tìm kiếm topping theo tên - Public
        /// </summary>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchToppings([FromQuery] string searchTerm)
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

                var toppings = await _toppingRepository.FindToppingsByNameAsync(searchTerm);

                var toppingResponses = toppings.Select(t => new ToppingResponse
                {
                    ToppingId = t.ToppingId,
                    Name = t.Name,
                    ToppingImage = t.ToppingImage,
                    Price = t.Price,
                    StockQuantity = t.StockQuantity,
                    IsAvailable = t.IsAvailable,
                    StockStatus = GetStockStatus(t.StockQuantity, t.MinStockLevel),
                    OrderCount = t.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<ToppingResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {toppingResponses.Count} topping",
                    Data = toppingResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching toppings with term: {SearchTerm}", searchTerm);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm topping"
                });
            }
        }

        /// <summary>
        /// Lấy topping theo khoảng giá - Public
        /// </summary>
        [HttpGet("price-range")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToppingsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
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

                var toppings = await _toppingRepository.GetToppingsByPriceRangeAsync(minPrice, maxPrice);

                var toppingResponses = toppings.Select(t => new ToppingResponse
                {
                    ToppingId = t.ToppingId,
                    Name = t.Name,
                    ToppingImage = t.ToppingImage,
                    Price = t.Price,
                    StockQuantity = t.StockQuantity,
                    IsAvailable = t.IsAvailable,
                    StockStatus = GetStockStatus(t.StockQuantity, t.MinStockLevel),
                    OrderCount = t.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<ToppingResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {toppingResponses.Count} topping trong khoảng giá {minPrice:C} - {maxPrice:C}",
                    Data = toppingResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting toppings by price range: {MinPrice} - {MaxPrice}", minPrice, maxPrice);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy topping theo khoảng giá"
                });
            }
        }

        // ==================== ADMIN ENDPOINTS ====================

        /// <summary>
        /// Tạo topping mới - Chỉ Admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTopping([FromBody] CreateToppingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newTopping = new Topping
                {
                    Name = request.Name,
                    ToppingImage = request.ToppingImage ?? "default-topping.jpg",
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    MinStockLevel = request.MinStockLevel,
                    IsAvailable = request.IsAvailable
                };

                await _toppingRepository.SaveToppingAsync(newTopping);

                return CreatedAtAction(nameof(GetToppingById), new { toppingId = newTopping.ToppingId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo topping thành công",
                        Data = new { ToppingId = newTopping.ToppingId }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topping");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo topping"
                });
            }
        }

        /// <summary>
        /// Cập nhật topping - Chỉ Admin
        /// </summary>
        [HttpPut("{toppingId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTopping(int toppingId, [FromBody] UpdateToppingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var topping = await _toppingRepository.FindToppingByIdAsync(toppingId);
                if (topping == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy topping"
                    });
                }

                topping.Name = request.Name;
                topping.ToppingImage = request.ToppingImage ?? topping.ToppingImage;
                topping.Price = request.Price;
                topping.StockQuantity = request.StockQuantity;
                topping.MinStockLevel = request.MinStockLevel;
                topping.IsAvailable = request.IsAvailable;

                await _toppingRepository.UpdateToppingAsync(topping);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật topping thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topping: {ToppingId}", toppingId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật topping"
                });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái có sẵn - Chỉ Admin
        /// </summary>
        [HttpPatch("{toppingId:int}/availability")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateToppingAvailability(int toppingId, [FromBody] UpdateToppingAvailabilityRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var topping = await _toppingRepository.FindToppingByIdAsync(toppingId);
                if (topping == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy topping"
                    });
                }

                await _toppingRepository.UpdateToppingAvailabilityAsync(toppingId, request.IsAvailable);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Đã {(request.IsAvailable ? "kích hoạt" : "vô hiệu hóa")} topping"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topping availability: {ToppingId}", toppingId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái topping"
                });
            }
        }

        /// <summary>
        /// Xóa topping - Chỉ Admin
        /// </summary>
        [HttpDelete("{toppingId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTopping(int toppingId)
        {
            try
            {
                var topping = await _toppingRepository.FindToppingByIdAsync(toppingId);
                if (topping == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy topping"
                    });
                }

                // Kiểm tra có đơn hàng nào đang sử dụng không
                if (topping.OrderItemToppings != null && topping.OrderItemToppings.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa topping này vì đã có đơn hàng sử dụng"
                    });
                }

                await _toppingRepository.DeleteToppingAsync(topping);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa topping thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting topping: {ToppingId}", toppingId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa topping"
                });
            }
        }

        // ==================== STATISTICS ENDPOINTS ====================

        /// <summary>
        /// Lấy thống kê topping - Chỉ Admin
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetToppingStatistics()
        {
            try
            {
                var allToppings = await _toppingRepository.GetAllAsync();
                var availableToppings = await _toppingRepository.GetAvailableToppingsAsync();

                var stats = new ToppingStatisticsResponse
                {
                    TotalToppings = allToppings.Count,
                    AvailableToppings = availableToppings.Count,
                    OutOfStockToppings = allToppings.Count(t => t.StockQuantity == 0),
                    LowStockToppings = allToppings.Count(t => t.StockQuantity > 0 && t.StockQuantity <= t.MinStockLevel),
                    TotalInventoryValue = allToppings.Sum(t => t.StockQuantity * t.Price),
                    AveragePrice = allToppings.Average(t => t.Price),
                    MostExpensiveTopping = allToppings.OrderByDescending(t => t.Price).FirstOrDefault()?.Name ?? "N/A",
                    CheapestTopping = allToppings.OrderBy(t => t.Price).FirstOrDefault()?.Name ?? "N/A",
                    MostPopularTopping = allToppings
                        .OrderByDescending(t => t.OrderItemToppings?.Sum(oit => oit.Quantity) ?? 0)
                        .FirstOrDefault()?.Name ?? "N/A"
                };

                return Ok(new ApiResponse<ToppingStatisticsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thống kê topping thành công",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting topping statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thống kê topping"
                });
            }
        }

        // ==================== HELPER METHODS ====================

        private string GetStockStatus(int stockQuantity, int minStockLevel)
        {
            if (stockQuantity == 0) return "Hết hàng";
            if (stockQuantity <= minStockLevel) return "Sắp hết";
            return "Có sẵn";
        }
    }
}
