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
    [Authorize(Roles = "Admin")]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientController(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        // Lấy danh sách tất cả nguyên liệu
        [HttpGet("get-all-ingredients")]
        public async Task<IActionResult> GetAllIngredients()
        {
            try
            {
                var ingredients = await _ingredientRepository.GetAllAsync();

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nguyên liệu thành công",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách nguyên liệu"
                });
            }
        }

        // Lấy thông tin nguyên liệu theo ID
        [HttpGet("get-ingredient-by-id/{ingredientId:int}")]
        public async Task<IActionResult> GetIngredientById(int ingredientId)
        {
            try
            {
                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(ingredientId);

                if (ingredient == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nguyên liệu"
                    });
                }

                var ingredientResponse = new IngredientDetailResponse
                {
                    IngredientId = ingredient.IngredientId,
                    Name = ingredient.Name,
                    IngredientImage = ingredient.IngredientImage,
                    Quantity = ingredient.Quantity,
                    UnitPrice = ingredient.UnitPrice,
                    StockStatus = GetStockStatus(ingredient.Quantity),
                    TotalValue = ingredient.Quantity * ingredient.UnitPrice,
                    DrinkRecipes = ingredient.DrinkRecipes?.Select(dr => new DrinkRecipeResponse
                    {
                        RecipeId = dr.RecipeId,
                        MenuItemId = dr.MenuItemId,
                        MenuItemName = dr.MenuItem?.Name,
                        QuantityMinGram = dr.QuantityMinGram,
                        QuantityMaxGram = dr.QuantityMaxGram
                    }).ToList()
                };

                return Ok(new ApiResponse<IngredientDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin nguyên liệu thành công",
                    Data = ingredientResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin nguyên liệu"
                });
            }
        }

        // Tạo nguyên liệu mới
        [HttpPost("create-new-ingredient")]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kiểm tra tên đã tồn tại
                var existingIngredient = await _ingredientRepository.FindIngredientByNameAsync(request.Name);
                if (existingIngredient != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên nguyên liệu đã tồn tại"
                    });
                }

                var newIngredient = new Ingredient
                {
                    Name = request.Name,
                    IngredientImage = request.IngredientImage ?? "default-ingredient.jpg",
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice
                };

                await _ingredientRepository.SaveIngredientAsync(newIngredient);

                return CreatedAtAction(nameof(GetIngredientById), new { ingredientId = newIngredient.IngredientId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo nguyên liệu thành công",
                        Data = new { IngredientId = newIngredient.IngredientId }
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo nguyên liệu"
                });
            }
        }

        // Cập nhật thông tin nguyên liệu
        [HttpPut("update-ingredient/{ingredientId:int}")]
        public async Task<IActionResult> UpdateIngredient(int ingredientId, [FromBody] UpdateIngredientRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(ingredientId);
                if (ingredient == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nguyên liệu"
                    });
                }

                // Kiểm tra tên trùng (ngoại trừ chính nó)
                var existingIngredient = await _ingredientRepository.FindIngredientByNameAsync(request.Name);
                if (existingIngredient != null && existingIngredient.IngredientId != ingredientId)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Tên nguyên liệu đã tồn tại"
                    });
                }

                ingredient.Name = request.Name;
                ingredient.IngredientImage = request.IngredientImage ?? ingredient.IngredientImage;
                ingredient.UnitPrice = request.UnitPrice;

                await _ingredientRepository.UpdateIngredientAsync(ingredient);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật nguyên liệu thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật nguyên liệu"
                });
            }
        }

        // Xóa nguyên liệu
        [HttpDelete("delete-ingredient/{ingredientId:int}")]
        public async Task<IActionResult> DeleteIngredient(int ingredientId)
        {
            try
            {
                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(ingredientId);
                if (ingredient == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nguyên liệu"
                    });
                }

                // Kiểm tra có công thức nào đang sử dụng không
                if (ingredient.DrinkRecipes != null && ingredient.DrinkRecipes.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không thể xóa nguyên liệu này vì đang được sử dụng trong công thức"
                    });
                }

                await _ingredientRepository.DeleteIngredientAsync(ingredient);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa nguyên liệu thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa nguyên liệu"
                });
            }
        }

        // Lấy danh sách nguyên liệu có sẵn
        [HttpGet("available-ingredients")]
        public async Task<IActionResult> GetAvailableIngredients()
        {
            try
            {
                var ingredients = await _ingredientRepository.GetAvailableIngredientsAsync();

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nguyên liệu có sẵn thành công",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách nguyên liệu có sẵn"
                });
            }
        }

        // Lấy danh sách nguyên liệu sắp hết
        [HttpGet("low-stock-ingredients")]
        public async Task<IActionResult> GetLowStockIngredients()
        {
            try
            {
                var ingredients = await _ingredientRepository.GetLowStockIngredientsAsync();

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nguyên liệu sắp hết thành công",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách nguyên liệu sắp hết"
                });
            }
        }

        // Lấy danh sách nguyên liệu hết hàng
        [HttpGet("out-of-stock-ingredients")]
        public async Task<IActionResult> GetOutOfStockIngredients()
        {
            try
            {
                var ingredients = await _ingredientRepository.GetOutOfStockIngredientsAsync();

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nguyên liệu hết hàng thành công",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách nguyên liệu hết hàng"
                });
            }
        }

        // Cập nhật tồn kho nguyên liệu
        [HttpPatch("update/{ingredientId:int}/stock")]
        public async Task<IActionResult> UpdateStock(int ingredientId, [FromBody] UpdateStockRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(ingredientId);
                if (ingredient == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nguyên liệu"
                    });
                }

                await _ingredientRepository.UpdateStockAsync(ingredientId, request.Quantity);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Cập nhật tồn kho thành công. {(request.Quantity > 0 ? "Nhập" : "Xuất")} {Math.Abs(request.Quantity)} đơn vị"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật tồn kho"
                });
            }
        }

        // Cập nhật giá đơn vị
        [HttpPatch("update/{ingredientId:int}/price")]
        public async Task<IActionResult> UpdateUnitPrice(int ingredientId, [FromBody] UpdatePriceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(ingredientId);
                if (ingredient == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy nguyên liệu"
                    });
                }

                await _ingredientRepository.UpdateUnitPriceAsync(ingredientId, request.UnitPrice);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật giá đơn vị thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật giá đơn vị"
                });
            }
        }

        // Cập nhật tồn kho hàng loạt
        [HttpPatch("bulk-update-stock-ingredients")]
        public async Task<IActionResult> BulkUpdateStock([FromBody] BulkUpdateStockRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _ingredientRepository.BulkUpdateStockAsync(request.IngredientQuantities);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = $"Cập nhật tồn kho hàng loạt thành công cho {request.IngredientQuantities.Count} nguyên liệu"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật tồn kho hàng loạt"
                });
            }
        }

        // Tìm kiếm nguyên liệu theo tên
        [HttpGet("search-ingredient")]
        public async Task<IActionResult> SearchIngredients([FromQuery] string searchTerm)
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

                var ingredients = await _ingredientRepository.SearchIngredientsByNameAsync(searchTerm);

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = $"Tìm thấy {ingredientResponses.Count} nguyên liệu",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tìm kiếm nguyên liệu"
                });
            }
        }

        // Lấy thống kê tồn kho
        [HttpGet("inventory-stats")]
        public async Task<IActionResult> GetInventoryStats()
        {
            try
            {
                var totalValue = await _ingredientRepository.GetTotalInventoryValueAsync();
                var totalCount = await _ingredientRepository.GetTotalIngredientsCountAsync();
                var lowStockIngredients = await _ingredientRepository.GetLowStockIngredientsAsync();
                var outOfStockIngredients = await _ingredientRepository.GetOutOfStockIngredientsAsync();

                var stats = new InventoryStatsResponse
                {
                    TotalIngredients = totalCount,
                    TotalInventoryValue = totalValue,
                    LowStockCount = lowStockIngredients.Count,
                    OutOfStockCount = outOfStockIngredients.Count,
                    AvailableCount = totalCount - outOfStockIngredients.Count
                };

                return Ok(new ApiResponse<InventoryStatsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thống kê tồn kho thành công",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = $"Có lỗi xảy ra khi lấy thống kê tồn kho: {ex.Message}"
                });
            }
        }

        // Lấy nguyên liệu theo công thức
        [HttpGet("ingredient-recipe/{drinkRecipeId:int}")]
        public async Task<IActionResult> GetIngredientsByRecipe(int drinkRecipeId)
        {
            try
            {
                var ingredients = await _ingredientRepository.GetIngredientsByDrinkRecipeAsync(drinkRecipeId);

                var ingredientResponses = ingredients.Select(i => new IngredientResponse
                {
                    IngredientId = i.IngredientId,
                    Name = i.Name,
                    IngredientImage = i.IngredientImage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    StockStatus = GetStockStatus(i.Quantity),
                    TotalValue = i.Quantity * i.UnitPrice,
                    RecipeCount = i.DrinkRecipes?.Count ?? 0
                }).ToList();

                return Ok(new ApiResponse<List<IngredientResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách nguyên liệu theo công thức thành công",
                    Data = ingredientResponses
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách nguyên liệu theo công thức"
                });
            }
        }

        private string GetStockStatus(int quantity)
        {
            if (quantity == 0) return "Hết hàng";
            if (quantity <= 5) return "Sắp hết";
            return "Có sẵn";
        }
    }
}
