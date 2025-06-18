using Cafe.BusinessObjects.Models.Request;
using Cafe.BusinessObjects.Models.Response;
using Cafe.BusinessObjects.Models;
using Cafe.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cafe.ManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemImageController : ControllerBase
    {
        private readonly IMenuItemImageRepository _menuItemImageRepository;
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemImageController(IMenuItemImageRepository menuItemImageRepository, IMenuItemRepository menuItemRepository)
        {
            _menuItemImageRepository = menuItemImageRepository;
            _menuItemRepository = menuItemRepository;
        }

        // Lấy danh sách hình ảnh theo MenuItem ID
        [HttpGet("get-img-menu-item/{menuItemId:int}")]
        public async Task<IActionResult> GetImagesByMenuItemId(int menuItemId)
        {
            try
            {
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                var images = await _menuItemImageRepository.GetImagesByMenuItemIdAsync(menuItemId);

                var imageResponses = images.Select(img => new MenuItemImageResponse
                {
                    ImageId = img.ImageId,
                    MenuItemId = img.MenuItemId,
                    ImagePath = img.ImageUrl,
                    ImageName = img.ImageName,
                    IsMainImage = img.IsMainImage,
                    DisplayOrder = img.DisplayOrder,
                    CreatedAt = img.CreatedAt,
                    UpdatedAt = img.UpdatedAt
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemImageResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách hình ảnh thành công",
                    Data = imageResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách hình ảnh"
                });
            }
        }

        // Lấy hình ảnh chính theo MenuItem ID 
        [HttpGet("get-img-menu-item/{menuItemId:int}/primary")]
        public async Task<IActionResult> GetPrimaryImageByMenuItemId(int menuItemId)
        {
            try
            {
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                var primaryImage = await _menuItemImageRepository.GetPrimaryImageByMenuItemIdAsync(menuItemId);

                if (primaryImage == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh chính"
                    });
                }

                var imageResponse = new MenuItemImageResponse
                {
                    ImageId = primaryImage.ImageId,
                    MenuItemId = primaryImage.MenuItemId,
                    ImagePath = primaryImage.ImageUrl,
                    ImageName = primaryImage.ImageName,
                    IsMainImage = primaryImage.IsMainImage,
                    DisplayOrder = primaryImage.DisplayOrder,
                    CreatedAt = primaryImage.CreatedAt,
                    UpdatedAt = primaryImage.UpdatedAt
                };

                return Ok(new ApiResponse<MenuItemImageResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy hình ảnh chính thành công",
                    Data = imageResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy hình ảnh chính"
                });
            }
        }

        // Lấy danh sách tất cả hình ảnh
        [HttpGet("get-all-imgs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllImages()
        {
            try
            {
                var images = await _menuItemImageRepository.GetAllAsync();

                var imageResponses = images.Select(img => new MenuItemImageDetailResponse
                {
                    ImageId = img.ImageId,
                    MenuItemId = img.MenuItemId,
                    MenuItemName = img.MenuItem?.Name,
                    ImageUrl = img.ImageUrl,
                    ImageName = img.ImageName,
                    IsMainImage = img.IsMainImage,
                    DisplayOrder = img.DisplayOrder,
                    CreatedAt = img.CreatedAt,
                    UpdatedAt = img.UpdatedAt
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemImageDetailResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tất cả hình ảnh thành công",
                    Data = imageResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách hình ảnh"
                });
            }
        }

        // Lấy thông tin hình ảnh theo ID 
        [HttpGet("get-img-by-id/{imageId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetImageById(int imageId)
        {
            try
            {
                var image = await _menuItemImageRepository.FindMenuItemImageByIdAsync(imageId);

                if (image == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh"
                    });
                }

                var imageResponse = new MenuItemImageDetailResponse
                {
                    ImageId = image.ImageId,
                    MenuItemId = image.MenuItemId,
                    MenuItemName = image.MenuItem?.Name,
                    ImageUrl = image.ImageUrl,
                    ImageName = image.ImageName,
                    IsMainImage = image.IsMainImage,
                    DisplayOrder = image.DisplayOrder,
                    CreatedAt = image.CreatedAt,
                    UpdatedAt = image.UpdatedAt
                };

                return Ok(new ApiResponse<MenuItemImageDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin hình ảnh thành công",
                    Data = imageResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin hình ảnh"
                });
            }
        }

        // Thêm hình ảnh mới
        [HttpPost("create-img")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateImage([FromBody] CreateMenuItemImageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra MenuItem có tồn tại
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(request.MenuItemId);
                if (menuItem == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Món ăn không tồn tại"
                    });
                }

                var newImage = new MenuItemImage
                {
                    MenuItemId = request.MenuItemId,
                    ImageUrl = request.ImageUrl,
                    ImageName = request.ImageName,
                    IsMainImage = request.IsMainImage,
                    DisplayOrder = request.DisplayOrder
                };

                await _menuItemImageRepository.SaveMenuItemImageAsync(newImage);

                return CreatedAtAction(nameof(GetImageById), new { imageId = newImage.ImageId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Thêm hình ảnh thành công",
                        Data = new { ImageId = newImage.ImageId }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thêm hình ảnh"
                });
            }
        }

        // Thêm nhiều hình ảnh cùng lúc 
        [HttpPost("bulk-multiple-imgs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMultipleImages([FromBody] CreateMultipleImagesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra MenuItem có tồn tại
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(request.MenuItemId);
                if (menuItem == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Món ăn không tồn tại"
                    });
                }

                var images = request.Images.Select((img, index) => new MenuItemImage
                {
                    MenuItemId = request.MenuItemId,
                    ImageUrl = img.ImageUrl,
                    ImageName = img.ImageName,
                    IsMainImage = img.IsMainImage,
                    DisplayOrder = img.DisplayOrder > 0 ? img.DisplayOrder : index + 1
                }).ToList();

                await _menuItemImageRepository.SaveMultipleMenuItemImagesAsync(images);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Thêm {images.Count} hình ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi thêm nhiều hình ảnh"
                });
            }
        }

        // Cập nhật thông tin hình ảnh
        [HttpPut("update-img/{imageId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateImage(int imageId, [FromBody] UpdateMenuItemImageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var image = await _menuItemImageRepository.FindMenuItemImageByIdAsync(imageId);
                if (image == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh"
                    });
                }

                image.ImageUrl = request.ImageUrl;
                image.ImageName = request.ImageName;
                image.IsMainImage = request.IsMainImage;
                image.DisplayOrder = request.DisplayOrder;

                await _menuItemImageRepository.UpdateMenuItemImageAsync(image);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật hình ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật hình ảnh"
                });
            }
        }

        // Đặt làm hình ảnh chính
        [HttpPatch("{imageId:int}/set-primary")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPrimaryImage(int imageId)
        {
            try
            {
                var image = await _menuItemImageRepository.FindMenuItemImageByIdAsync(imageId);
                if (image == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh"
                    });
                }

                await _menuItemImageRepository.SetPrimaryImageAsync(imageId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Đặt hình ảnh chính thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi đặt hình ảnh chính"
                });
            }
        }

        // Cập nhật thứ tự hiển thị 
        [HttpPatch("update/{imageId:int}/display-order")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDisplayOrder(int imageId, [FromBody] UpdateDisplayOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var image = await _menuItemImageRepository.FindMenuItemImageByIdAsync(imageId);
                if (image == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh"
                    });
                }

                await _menuItemImageRepository.UpdateDisplayOrderAsync(imageId, request.DisplayOrder);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật thứ tự hiển thị thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật thứ tự hiển thị"
                });
            }
        }

        // Sắp xếp lại thứ tự hình ảnh 
        [HttpPatch("menu-item/{menuItemId:int}/reorder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReorderImages(int menuItemId, [FromBody] ReorderImagesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
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

                await _menuItemImageRepository.ReorderImagesAsync(menuItemId, request.ImageIds);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Sắp xếp lại thứ tự hình ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi sắp xếp lại thứ tự hình ảnh"
                });
            }
        }

        // Xóa hình ảnh
        [HttpDelete("delete-img/{imageId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                var image = await _menuItemImageRepository.FindMenuItemImageByIdAsync(imageId);
                if (image == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy hình ảnh"
                    });
                }

                await _menuItemImageRepository.DeleteMenuItemImageAsync(image);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa hình ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa hình ảnh"
                });
            }
        }

        // Xóa tất cả hình ảnh của một món ăn
        [HttpDelete("delete-all-imgs-in-menu-item/{menuItemId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImagesByMenuItemId(int menuItemId)
        {
            try
            {
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                await _menuItemImageRepository.DeleteImagesByMenuItemIdAsync(menuItemId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa tất cả hình ảnh của món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa hình ảnh"
                });
            }
        }

        // Tìm kiếm hình ảnh theo loại file
        [HttpGet("file-type/{fileType}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetImagesByFileType(string fileType)
        {
            try
            {
                var images = await _menuItemImageRepository.GetImagesByFileTypeAsync(fileType);

                var imageResponses = images.Select(img => new MenuItemImageDetailResponse
                {
                    ImageId = img.ImageId,
                    MenuItemId = img.MenuItemId,
                    MenuItemName = img.MenuItem?.Name,
                    ImageUrl = img.ImageUrl,
                    ImageName = img.ImageName,
                    IsMainImage = img.IsMainImage,
                    DisplayOrder = img.DisplayOrder,
                    CreatedAt = img.CreatedAt,
                    UpdatedAt = img.UpdatedAt
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemImageDetailResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {imageResponses.Count} hình ảnh loại {fileType}",
                    Data = imageResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm hình ảnh theo loại file"
                });
            }
        }

        // Lấy hình ảnh theo khoảng thời gian
        [HttpGet("get-imgs-by-date-range")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetImagesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc"
                    });
                }

                var images = await _menuItemImageRepository.GetImagesByDateRangeAsync(startDate, endDate);

                var imageResponses = images.Select(img => new MenuItemImageDetailResponse
                {
                    ImageId = img.ImageId,
                    MenuItemId = img.MenuItemId,
                    MenuItemName = img.MenuItem?.Name,
                    ImageUrl = img.ImageUrl,
                    ImageName = img.ImageName,
                    IsMainImage = img.IsMainImage,
                    DisplayOrder = img.DisplayOrder,
                    CreatedAt = img.CreatedAt,
                    UpdatedAt = img.UpdatedAt
                }).ToList();

                return Ok(new ApiResponse<List<MenuItemImageDetailResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {imageResponses.Count} hình ảnh trong khoảng thời gian",
                    Data = imageResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy hình ảnh theo khoảng thời gian"
                });
            }
        }

        // Lấy số lượng hình ảnh theo món ăn 
        [HttpGet("menu-item/{menuItemId:int}/count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetImageCountByMenuItemId(int menuItemId)
        {
            try
            {
                var menuItem = await _menuItemRepository.FindMenuItemByIdAsync(menuItemId);
                if (menuItem == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                var count = await _menuItemImageRepository.GetImageCountByMenuItemIdAsync(menuItemId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Lấy số lượng hình ảnh thành công",
                    Data = new { MenuItemId = menuItemId, ImageCount = count }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy số lượng hình ảnh"
                });
            }
        }
    }
}
