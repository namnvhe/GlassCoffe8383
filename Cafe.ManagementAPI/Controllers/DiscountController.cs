using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Cafe.BusinessObjects.Models.Request;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountController(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        // Lấy danh sách discount đang hoạt động
        [HttpGet("get-active-discount")]
        public async Task<IActionResult> GetActiveDiscounts()
        {
            try
            {
                var discounts = await _discountRepository.GetActiveDiscountsAsync();

                var discountResponses = discounts.Select(d => new DiscountResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days
                }).ToList();

                return Ok(new ApiResponse<List<DiscountResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách khuyến mãi đang hoạt động thành công",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách khuyến mãi"
                });
            }
        }

        // Validate mã giảm giá
        [HttpPost("validate-discount-code")]
        public async Task<IActionResult> ValidateDiscountCode([FromBody] ValidateDiscountRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var discount = await _discountRepository.GetDiscountByCodeAsync(request.Code);

                if (discount == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Mã giảm giá không tồn tại hoặc đã hết hạn"
                    });
                }

                var isValid = _discountRepository.IsDiscountValid(discount);
                if (!isValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Mã giảm giá đã hết hạn hoặc không còn hiệu lực"
                    });
                }

                var discountedAmount = _discountRepository.ApplyDiscount(request.OrderAmount, discount);
                var discountAmount = request.OrderAmount - discountedAmount;

                var response = new ValidateDiscountResponse
                {
                    DiscountId = discount.DiscountId,
                    Code = discount.Code,
                    Description = discount.Description,
                    DiscountType = discount.DiscountType,
                    Value = discount.Value,
                    OriginalAmount = request.OrderAmount,
                    DiscountAmount = discountAmount,
                    FinalAmount = discountedAmount,
                    IsValid = true
                };

                return Ok(new ApiResponse<ValidateDiscountResponse>
                {
                    IsSuccess = true,
                    Message = "Mã giảm giá hợp lệ",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi kiểm tra mã giảm giá"
                });
            }
        }

        // Lấy danh sách tất cả discount
        [HttpGet("get-all-discounts")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDiscounts()
        {
            try
            {
                var discounts = await _discountRepository.GetAllAsync();

                var discountResponses = discounts.Select(d => new DiscountDetailResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days,
                    CanDelete = d.Orders?.Count == 0
                }).ToList();

                return Ok(new ApiResponse<List<DiscountDetailResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tất cả khuyến mãi thành công",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách khuyến mãi"
                });
            }
        }

        // Lấy thông tin discount theo ID 
        [HttpGet("get-discount-by-id/{discountId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscountById(int discountId)
        {
            try
            {
                var discount = await _discountRepository.FindDiscountByIdAsync(discountId);

                if (discount == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy khuyến mãi"
                    });
                }

                var discountResponse = new DiscountDetailResponse
                {
                    DiscountId = discount.DiscountId,
                    Code = discount.Code,
                    Description = discount.Description,
                    DiscountType = discount.DiscountType,
                    Value = discount.Value,
                    ExpiryDate = discount.ExpiryDate,
                    IsActive = discount.IsActive,
                    UsageCount = discount.Orders?.Count ?? 0,
                    DaysUntilExpiry = (discount.ExpiryDate - DateTime.Today).Days,
                    CanDelete = discount.Orders?.Count == 0,
                    Orders = discount.Orders?.Select(o => new OrderSummary
                    {
                        OrderId = o.OrderId,
                        OrderTime = o.OrderTime,
                        Status = o.Status,
                        UserId = o.UserId
                    }).ToList()
                };

                return Ok(new ApiResponse<DiscountDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin khuyến mãi thành công",
                    Data = discountResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin khuyến mãi"
                });
            }
        }

        // Tạo discount mới
        [HttpPost("create-new-discount")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra mã code đã tồn tại
                var existingDiscount = await _discountRepository.FindDiscountByCodeAsync(request.Code);
                if (existingDiscount != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Mã giảm giá đã tồn tại"
                    });
                }

                // Validate discount type
                if (request.DiscountType != "Amount" && request.DiscountType != "Percent")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Loại giảm giá chỉ có thể là 'Amount' hoặc 'Percent'"
                    });
                }

                // Validate discount value
                if (request.DiscountType == "Percent" && (request.Value <= 0 || request.Value > 100))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Giá trị giảm giá theo phần trăm phải từ 1% đến 100%"
                    });
                }

                if (request.DiscountType == "Amount" && request.Value <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Giá trị giảm giá theo số tiền phải lớn hơn 0"
                    });
                }

                var newDiscount = new Discount
                {
                    Code = request.Code.ToUpper(),
                    Description = request.Description,
                    DiscountType = request.DiscountType,
                    Value = request.Value,
                    ExpiryDate = request.ExpiryDate,
                    IsActive = request.IsActive
                };

                await _discountRepository.SaveDiscountAsync(newDiscount);

                return CreatedAtAction(nameof(GetDiscountById), new { discountId = newDiscount.DiscountId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo khuyến mãi thành công",
                        Data = new { DiscountId = newDiscount.DiscountId }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo khuyến mãi"
                });
            }
        }

        // Cập nhật discount
        [HttpPut("update-discount/{discountId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDiscount(int discountId, [FromBody] UpdateDiscountRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var discount = await _discountRepository.FindDiscountByIdAsync(discountId);
                if (discount == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy khuyến mãi"
                    });
                }

                // Kiểm tra mã code trùng (ngoại trừ chính nó)
                var existingDiscount = await _discountRepository.FindDiscountByCodeAsync(request.Code);
                if (existingDiscount != null && existingDiscount.DiscountId != discountId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Mã giảm giá đã tồn tại"
                    });
                }

                // Validate discount type
                if (request.DiscountType != "Amount" && request.DiscountType != "Percent")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Loại giảm giá chỉ có thể là 'Amount' hoặc 'Percent'"
                    });
                }

                // Validate discount value
                if (request.DiscountType == "Percent" && (request.Value <= 0 || request.Value > 100))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Giá trị giảm giá theo phần trăm phải từ 1% đến 100%"
                    });
                }

                if (request.DiscountType == "Amount" && request.Value <= 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Giá trị giảm giá theo số tiền phải lớn hơn 0"
                    });
                }

                discount.Code = request.Code.ToUpper();
                discount.Description = request.Description;
                discount.DiscountType = request.DiscountType;
                discount.Value = request.Value;
                discount.ExpiryDate = request.ExpiryDate;
                discount.IsActive = request.IsActive;

                await _discountRepository.UpdateDiscountAsync(discount);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật khuyến mãi"
                });
            }
        }

        // Cập nhật trạng thái discount
        [HttpPatch("update/{discountId:int}/status")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDiscountStatus(int discountId, [FromBody] UpdateDiscountStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var discount = await _discountRepository.FindDiscountByIdAsync(discountId);
                if (discount == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy khuyến mãi"
                    });
                }

                await _discountRepository.UpdateDiscountStatusAsync(discountId, request.IsActive);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Đã {(request.IsActive ? "kích hoạt" : "vô hiệu hóa")} khuyến mãi"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái khuyến mãi"
                });
            }
        }

        // Xóa discount
        [HttpDelete("delete-by-id/{discountId:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDiscount(int discountId)
        {
            try
            {
                var discount = await _discountRepository.FindDiscountByIdAsync(discountId);
                if (discount == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy khuyến mãi"
                    });
                }

                // Kiểm tra có thể xóa không
                var canDelete = await _discountRepository.CanDeleteDiscountAsync(discountId);
                if (!canDelete)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa khuyến mãi này vì đã có đơn hàng sử dụng"
                    });
                }

                await _discountRepository.DeleteDiscountAsync(discount);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa khuyến mãi thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa khuyến mãi"
                });
            }
        }


        // Lấy discount theo loại
        [HttpGet("get-by-type/{discountType}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscountsByType(string discountType)
        {
            try
            {
                if (discountType != "Amount" && discountType != "Percent")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Loại giảm giá chỉ có thể là 'Amount' hoặc 'Percent'"
                    });
                }

                var discounts = await _discountRepository.GetDiscountsByTypeAsync(discountType);

                var discountResponses = discounts.Select(d => new DiscountResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days
                }).ToList();

                return Ok(new ApiResponse<List<DiscountResponse>>
                {
                    IsSuccess = true,
                    Message = $"Lấy danh sách khuyến mãi loại {discountType} thành công",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách khuyến mãi theo loại"
                });
            }
        }

        // Lấy discount hết hạn
        [HttpGet("discount-expired")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetExpiredDiscounts()
        {
            try
            {
                var discounts = await _discountRepository.GetExpiredDiscountsAsync();

                var discountResponses = discounts.Select(d => new DiscountResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days
                }).ToList();

                return Ok(new ApiResponse<List<DiscountResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách khuyến mãi đã hết hạn thành công",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách khuyến mãi đã hết hạn"
                });
            }
        }

        // Lấy discount sắp hết hạn
        // </summary>
        [HttpGet("discount-expiring-soon")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscountsExpiringSoon([FromQuery] int days = 7)
        {
            try
            {
                var discounts = await _discountRepository.GetDiscountsExpiringSoonAsync(days);

                var discountResponses = discounts.Select(d => new DiscountResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days
                }).ToList();

                return Ok(new ApiResponse<List<DiscountResponse>>
                {
                    IsSuccess = true,
                    Message = $"Lấy danh sách khuyến mãi sắp hết hạn trong {days} ngày thành công",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách khuyến mãi sắp hết hạn"
                });
            }
        }

        // Tìm kiếm discount
        [HttpGet("search-discount")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchDiscounts([FromQuery] string searchTerm)
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

                var discounts = await _discountRepository.SearchDiscountsByNameAsync(searchTerm);

                var discountResponses = discounts.Select(d => new DiscountResponse
                {
                    DiscountId = d.DiscountId,
                    Code = d.Code,
                    Description = d.Description,
                    DiscountType = d.DiscountType,
                    Value = d.Value,
                    ExpiryDate = d.ExpiryDate,
                    IsActive = d.IsActive,
                    UsageCount = d.Orders?.Count ?? 0,
                    DaysUntilExpiry = (d.ExpiryDate - DateTime.Today).Days
                }).ToList();

                return Ok(new ApiResponse<List<DiscountResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {discountResponses.Count} khuyến mãi",
                    Data = discountResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm khuyến mãi"
                });
            }
        }

        // Lấy thống kê discount
        [HttpGet("statistics")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDiscountStatistics()
        {
            try
            {
                var allDiscounts = await _discountRepository.GetAllAsync();
                var activeDiscounts = await _discountRepository.GetActiveDiscountsAsync();
                var expiredDiscounts = await _discountRepository.GetExpiredDiscountsAsync();
                var expiringSoon = await _discountRepository.GetDiscountsExpiringSoonAsync(7);

                var stats = new DiscountStatisticsResponse
                {
                    TotalDiscounts = allDiscounts.Count,
                    ActiveDiscounts = activeDiscounts.Count,
                    ExpiredDiscounts = expiredDiscounts.Count,
                    ExpiringSoonDiscounts = expiringSoon.Count,
                    PercentDiscounts = allDiscounts.Count(d => d.DiscountType == "Percent"),
                    AmountDiscounts = allDiscounts.Count(d => d.DiscountType == "Amount"),
                    TotalUsage = allDiscounts.Sum(d => d.Orders?.Count ?? 0),
                    MostUsedDiscount = allDiscounts
                        .OrderByDescending(d => d.Orders?.Count ?? 0)
                        .FirstOrDefault()?.Code ?? "N/A"
                };

                return Ok(new ApiResponse<DiscountStatisticsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thống kê khuyến mãi thành công",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thống kê khuyến mãi"
                });
            }
        }
    }
}
