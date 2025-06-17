using Cafe.BusinessObjects.Models;
using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        private readonly ISizeRepository _sizeRepository;

        public SizeController(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }

        // Lấy danh sách tất cả size 
        [HttpGet("get-all-size")]
        public async Task<IActionResult> GetAllSizes()
        {
            try
            {
                var sizes = await _sizeRepository.GetAllAsync();

                var sizeResponses = sizes.Select(s => new SizeResponse
                {
                    SizeId = s.SizeId,
                    Name = s.Name,
                    ExtraPrice = s.ExtraPrice,
                    OrderCount = s.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    IsPopular = s.OrderItems?.Sum(oi => oi.Quantity) > 10
                }).ToList();

                return Ok(new ApiResponse<List<SizeResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách size thành công",
                    Data = sizeResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách size"
                });
            }
        }

        // Lấy thông tin size theo ID
        [HttpGet("get-size-by-id/{sizeId:int}")]
        public async Task<IActionResult> GetSizeById(int sizeId)
        {
            try
            {
                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);

                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                var sizeResponse = new SizeDetailResponse
                {
                    SizeId = size.SizeId,
                    Name = size.Name,
                    ExtraPrice = size.ExtraPrice,
                    OrderCount = size.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    IsPopular = size.OrderItems?.Sum(oi => oi.Quantity) > 10,
                    RecentOrders = size.OrderItems?.Take(5).Select(oi => new OrderItemSummary
                    {
                        OrderItemId = oi.OrderItemId,
                        OrderId = oi.OrderId,
                        Quantity = oi.Quantity,
                        MenuItemName = oi.MenuItem?.Name
                    }).ToList()
                };

                return Ok(new ApiResponse<SizeDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin size thành công",
                    Data = sizeResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin size"
                });
            }
        }

        // Lấy size theo khoảng giá
        [HttpGet("get-size-by-price-range")]
        public async Task<IActionResult> GetSizesByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
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

                var sizes = await _sizeRepository.GetSizesByPriceRangeAsync(minPrice, maxPrice);

                var sizeResponses = sizes.Select(s => new SizeResponse
                {
                    SizeId = s.SizeId,
                    Name = s.Name,
                    ExtraPrice = s.ExtraPrice,
                    OrderCount = s.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    IsPopular = s.OrderItems?.Sum(oi => oi.Quantity) > 10
                }).ToList();

                return Ok(new ApiResponse<List<SizeResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {sizeResponses.Count} size trong khoảng giá {minPrice:C} - {maxPrice:C}",
                    Data = sizeResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy size theo khoảng giá"
                });
            }
        }

        // Lấy size phổ biến
        [HttpGet("get-popular-size")]
        public async Task<IActionResult> GetPopularSizes([FromQuery] int topCount = 5)
        {
            try
            {
                var sizes = await _sizeRepository.GetPopularSizesAsync(topCount);

                var sizeResponses = sizes.Select(s => new PopularSizeResponse
                {
                    SizeId = s.SizeId,
                    Name = s.Name,
                    ExtraPrice = s.ExtraPrice,
                    TotalOrders = s.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    PopularityRank = sizes.ToList().IndexOf(s) + 1
                }).ToList();

                return Ok(new ApiResponse<List<PopularSizeResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách size phổ biến thành công",
                    Data = sizeResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách size phổ biến"
                });
            }
        }

        // Tìm kiếm size theo tên
        [HttpGet("search-size")]
        public async Task<IActionResult> SearchSizes([FromQuery] string searchTerm)
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

                var sizes = await _sizeRepository.SearchSizesByNameAsync(searchTerm);

                var sizeResponses = sizes.Select(s => new SizeResponse
                {
                    SizeId = s.SizeId,
                    Name = s.Name,
                    ExtraPrice = s.ExtraPrice,
                    OrderCount = s.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                    IsPopular = s.OrderItems?.Sum(oi => oi.Quantity) > 10
                }).ToList();

                return Ok(new ApiResponse<List<SizeResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {sizeResponses.Count} size",
                    Data = sizeResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm size"
                });
            }
        }

        // Tạo size mới 
        [HttpPost("create-new-size")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSize([FromBody] CreateSizeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra tên đã tồn tại
                var existingSize = await _sizeRepository.FindSizeByNameAsync(request.Name);
                if (existingSize != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên size đã tồn tại"
                    });
                }

                var newSize = new Size
                {
                    Name = request.Name,
                    ExtraPrice = request.ExtraPrice
                };

                await _sizeRepository.SaveSizeAsync(newSize);

                return CreatedAtAction(nameof(GetSizeById), new { sizeId = newSize.SizeId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo size thành công",
                        Data = new { SizeId = newSize.SizeId }
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo size"
                });
            }
        }

        // Cập nhật size
        [HttpPut("update-size/{sizeId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSize(int sizeId, [FromBody] UpdateSizeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);
                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                // Kiểm tra tên trùng (ngoại trừ chính nó)
                var existingSize = await _sizeRepository.FindSizeByNameAsync(request.Name);
                if (existingSize != null && existingSize.SizeId != sizeId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên size đã tồn tại"
                    });
                }

                size.Name = request.Name;
                size.ExtraPrice = request.ExtraPrice;

                await _sizeRepository.UpdateSizeAsync(size);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật size thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật size"
                });
            }
        }

        // Cập nhật giá size
        [HttpPatch("update/{sizeId:int}/price")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSizePrice(int sizeId, [FromBody] UpdateSizePriceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);
                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                await _sizeRepository.UpdateSizePriceAsync(sizeId, request.ExtraPrice);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật giá size thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật giá size"
                });
            }
        }

        // Cập nhật tên size
        [HttpPatch("update/{sizeId:int}/name")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSizeName(int sizeId, [FromBody] UpdateSizeNameRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);
                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                // Kiểm tra tên trùng
                var existingSize = await _sizeRepository.FindSizeByNameAsync(request.Name);
                if (existingSize != null && existingSize.SizeId != sizeId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên size đã tồn tại"
                    });
                }

                await _sizeRepository.UpdateSizeNameAsync(sizeId, request.Name);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật tên size thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật tên size"
                });
            }
        }

        // Xóa size 
        [HttpDelete("delete/{sizeId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSize(int sizeId)
        {
            try
            {
                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);
                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                // Kiểm tra có đơn hàng nào đang sử dụng không
                if (size.OrderItems != null && size.OrderItems.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa size này vì đã có đơn hàng sử dụng"
                    });
                }

                await _sizeRepository.DeleteSizeAsync(size);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa size thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa size"
                });
            }
        }

        // Lấy thống kê size
        [HttpGet("{sizeId:int}/statistics")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSizeStatistics(int sizeId)
        {
            try
            {
                var size = await _sizeRepository.FindSizeByIdAsync(sizeId);
                if (size == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy size"
                    });
                }

                var orderCount = await _sizeRepository.GetOrderCountBySizeAsync(sizeId);

                var stats = new SizeStatisticsResponse
                {
                    SizeId = size.SizeId,
                    Name = size.Name,
                    ExtraPrice = size.ExtraPrice,
                    TotalOrders = orderCount,
                    CanDelete = size.OrderItems?.Count == 0,
                    Revenue = size.OrderItems?.Sum(oi => oi.Quantity * size.ExtraPrice) ?? 0
                };

                return Ok(new ApiResponse<SizeStatisticsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thống kê size thành công",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thống kê size"
                });
            }
        }

        // Lấy tổng quan thống kê tất cả size 
        [HttpGet("statistics/overview")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSizeOverviewStatistics()
        {
            try
            {
                var allSizes = await _sizeRepository.GetAllAsync();
                var popularSizes = await _sizeRepository.GetPopularSizesAsync(3);

                var overview = new SizeOverviewStatisticsResponse
                {
                    TotalSizes = allSizes.Count,
                    TotalOrders = allSizes.Sum(s => s.OrderItems?.Sum(oi => oi.Quantity) ?? 0),
                    TotalRevenue = allSizes.Sum(s => s.OrderItems?.Sum(oi => oi.Quantity * s.ExtraPrice) ?? 0),
                    AverageExtraPrice = allSizes.Average(s => s.ExtraPrice),
                    MostPopularSize = popularSizes.FirstOrDefault()?.Name ?? "N/A",
                    LeastPopularSize = allSizes.OrderBy(s => s.OrderItems?.Sum(oi => oi.Quantity) ?? 0).FirstOrDefault()?.Name ?? "N/A"
                };

                return Ok(new ApiResponse<SizeOverviewStatisticsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy tổng quan thống kê size thành công",
                    Data = overview
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy tổng quan thống kê size"
                });
            }
        }
    }
}
