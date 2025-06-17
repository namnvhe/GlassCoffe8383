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
    public class DrinkRecipeController : ControllerBase
    {
        private readonly IDrinkRecipeRepository _drinkRecipeRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILogger<DrinkRecipeController> _logger;

        public DrinkRecipeController(
            IDrinkRecipeRepository drinkRecipeRepository,
            IMenuItemRepository menuItemRepository,
            IIngredientRepository ingredientRepository,
            ILogger<DrinkRecipeController> logger)
        {
            _drinkRecipeRepository = drinkRecipeRepository;
            _menuItemRepository = menuItemRepository;
            _ingredientRepository = ingredientRepository;
            _logger = logger;
        }

        // ==================== CUSTOMER ENDPOINTS ====================

        /// <summary>
        /// Lấy công thức theo món ăn - Customer
        /// </summary>
        [HttpGet("menu-item/{menuItemId:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetRecipesByMenuItemId(int menuItemId)
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

                var recipes = await _drinkRecipeRepository.GetRecipesByMenuItemIdAsync(menuItemId);

                var recipeResponses = recipes.Select(r => new DrinkRecipeResponse
                {
                    RecipeId = r.RecipeId,
                    MenuItemId = r.MenuItemId,
                    MenuItemName = r.MenuItem?.Name,
                    IngredientId = r.IngredientId,
                    IngredientName = r.Ingredient?.Name,
                    QuantityMinGram = r.QuantityMinGram,
                    QuantityMaxGram = r.QuantityMaxGram,
                    QuantityRange = $"{r.QuantityMinGram}-{r.QuantityMaxGram}g"
                }).ToList();

                return Ok(new ApiResponse<List<DrinkRecipeResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy công thức món ăn thành công",
                    Data = recipeResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipes by menu item ID: {MenuItemId}", menuItemId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy công thức món ăn"
                });
            }
        }

        /// <summary>
        /// Lấy công thức theo nguyên liệu - Customer
        /// </summary>
        [HttpGet("ingredient/{ingredientId:int}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetRecipesByIngredientId(int ingredientId)
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

                var recipes = await _drinkRecipeRepository.GetRecipesByIngredientIdAsync(ingredientId);

                var recipeResponses = recipes.Select(r => new DrinkRecipeResponse
                {
                    RecipeId = r.RecipeId,
                    MenuItemId = r.MenuItemId,
                    MenuItemName = r.MenuItem?.Name,
                    IngredientId = r.IngredientId,
                    IngredientName = r.Ingredient?.Name,
                    QuantityMinGram = r.QuantityMinGram,
                    QuantityMaxGram = r.QuantityMaxGram,
                    QuantityRange = $"{r.QuantityMinGram}-{r.QuantityMaxGram}g"
                }).ToList();

                return Ok(new ApiResponse<List<DrinkRecipeResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách món ăn sử dụng nguyên liệu thành công",
                    Data = recipeResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipes by ingredient ID: {IngredientId}", ingredientId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách món ăn sử dụng nguyên liệu"
                });
            }
        }

        // ==================== ADMIN ENDPOINTS ====================

        /// <summary>
        /// Lấy danh sách tất cả công thức - Chỉ Admin
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRecipes()
        {
            try
            {
                var recipes = await _drinkRecipeRepository.GetAllAsync();

                var recipeResponses = recipes.Select(r => new DrinkRecipeDetailResponse
                {
                    RecipeId = r.RecipeId,
                    MenuItemId = r.MenuItemId,
                    MenuItemName = r.MenuItem?.Name,
                    MenuItemPrice = r.MenuItem?.Price ?? 0,
                    IngredientId = r.IngredientId,
                    IngredientName = r.Ingredient?.Name,
                    IngredientUnitPrice = r.Ingredient?.UnitPrice ?? 0,
                    QuantityMinGram = r.QuantityMinGram,
                    QuantityMaxGram = r.QuantityMaxGram,
                    QuantityRange = $"{r.QuantityMinGram}-{r.QuantityMaxGram}g",
                    EstimatedCost = CalculateIngredientCost(r.QuantityMaxGram, r.Ingredient?.UnitPrice ?? 0)
                }).ToList();

                return Ok(new ApiResponse<List<DrinkRecipeDetailResponse>>
                {
                    IsSuccess = true,
                    Message = "Lấy danh sách tất cả công thức thành công",
                    Data = recipeResponses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all recipes");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách công thức"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin công thức theo ID - Chỉ Admin
        /// </summary>
        [HttpGet("{recipeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecipeById(int recipeId)
        {
            try
            {
                var recipe = await _drinkRecipeRepository.FindRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy công thức"
                    });
                }

                var recipeResponse = new DrinkRecipeDetailResponse
                {
                    RecipeId = recipe.RecipeId,
                    MenuItemId = recipe.MenuItemId,
                    MenuItemName = recipe.MenuItem?.Name,
                    MenuItemPrice = recipe.MenuItem?.Price ?? 0,
                    IngredientId = recipe.IngredientId,
                    IngredientName = recipe.Ingredient?.Name,
                    IngredientUnitPrice = recipe.Ingredient?.UnitPrice ?? 0,
                    QuantityMinGram = recipe.QuantityMinGram,
                    QuantityMaxGram = recipe.QuantityMaxGram,
                    QuantityRange = $"{recipe.QuantityMinGram}-{recipe.QuantityMaxGram}g",
                    EstimatedCost = CalculateIngredientCost(recipe.QuantityMaxGram, recipe.Ingredient?.UnitPrice ?? 0)
                };

                return Ok(new ApiResponse<DrinkRecipeDetailResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin công thức thành công",
                    Data = recipeResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe by ID: {RecipeId}", recipeId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy thông tin công thức"
                });
            }
        }

        /// <summary>
        /// Tạo công thức mới - Chỉ Admin
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRecipe([FromBody] CreateDrinkRecipeRequest request)
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

                // Kiểm tra Ingredient có tồn tại
                var ingredient = await _ingredientRepository.FindIngredientByIdAsync(request.IngredientId);
                if (ingredient == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Nguyên liệu không tồn tại"
                    });
                }

                // Kiểm tra công thức đã tồn tại
                var existingRecipe = await _drinkRecipeRepository.FindRecipeByMenuItemAndIngredientAsync(
                    request.MenuItemId, request.IngredientId);
                if (existingRecipe != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Công thức cho món ăn và nguyên liệu này đã tồn tại"
                    });
                }

                // Validate quantity range
                if (request.QuantityMinGram >= request.QuantityMaxGram)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Số lượng tối thiểu phải nhỏ hơn số lượng tối đa"
                    });
                }

                var newRecipe = new DrinkRecipe
                {
                    MenuItemId = request.MenuItemId,
                    IngredientId = request.IngredientId,
                    QuantityMinGram = request.QuantityMinGram,
                    QuantityMaxGram = request.QuantityMaxGram
                };

                await _drinkRecipeRepository.SaveRecipeAsync(newRecipe);

                return CreatedAtAction(nameof(GetRecipeById), new { recipeId = newRecipe.RecipeId },
                    new ApiResponse<object>
                    {
                        IsSuccess = true,
                        Message = "Tạo công thức thành công",
                        Data = new { RecipeId = newRecipe.RecipeId }
                    });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when creating recipe");
                return BadRequest(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recipe");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi tạo công thức"
                });
            }
        }

        /// <summary>
        /// Cập nhật công thức - Chỉ Admin
        /// </summary>
        [HttpPut("{recipeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRecipe(int recipeId, [FromBody] UpdateDrinkRecipeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var recipe = await _drinkRecipeRepository.FindRecipeByIdAsync(recipeId);
                if (recipe == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy công thức"
                    });
                }

                // Validate quantity range
                if (request.QuantityMinGram >= request.QuantityMaxGram)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Số lượng tối thiểu phải nhỏ hơn số lượng tối đa"
                    });
                }

                recipe.QuantityMinGram = request.QuantityMinGram;
                recipe.QuantityMaxGram = request.QuantityMaxGram;

                await _drinkRecipeRepository.UpdateRecipeAsync(recipe);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật công thức thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe: {RecipeId}", recipeId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật công thức"
                });
            }
        }

        /// <summary>
        /// Cập nhật số lượng nguyên liệu - Chỉ Admin
        /// </summary>
        [HttpPatch("{recipeId:int}/quantity")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRecipeQuantity(int recipeId, [FromBody] UpdateRecipeQuantityRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var recipe = await _drinkRecipeRepository.FindRecipeByIdAsync(recipeId);
                if (recipe == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy công thức"
                    });
                }

                // Validate quantity range
                if (request.QuantityMinGram >= request.QuantityMaxGram)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Số lượng tối thiểu phải nhỏ hơn số lượng tối đa"
                    });
                }

                await _drinkRecipeRepository.UpdateRecipeQuantityAsync(recipeId, request.QuantityMinGram, request.QuantityMaxGram);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Cập nhật số lượng nguyên liệu thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe quantity: {RecipeId}", recipeId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi cập nhật số lượng nguyên liệu"
                });
            }
        }

        /// <summary>
        /// Xóa công thức - Chỉ Admin
        /// </summary>
        [HttpDelete("{recipeId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRecipe(int recipeId)
        {
            try
            {
                var recipe = await _drinkRecipeRepository.FindRecipeByIdAsync(recipeId);
                if (recipe == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy công thức"
                    });
                }

                await _drinkRecipeRepository.DeleteRecipeAsync(recipe);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa công thức thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe: {RecipeId}", recipeId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa công thức"
                });
            }
        }

        /// <summary>
        /// Xóa tất cả công thức của một món ăn - Chỉ Admin
        /// </summary>
        [HttpDelete("menu-item/{menuItemId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRecipesByMenuItemId(int menuItemId)
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

                await _drinkRecipeRepository.DeleteRecipesByMenuItemIdAsync(menuItemId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Xóa tất cả công thức của món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipes by menu item ID: {MenuItemId}", menuItemId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi xóa công thức"
                });
            }
        }

        // ==================== STATISTICS ENDPOINTS ====================

        /// <summary>
        /// Lấy thống kê công thức theo món ăn - Chỉ Admin
        /// </summary>
        [HttpGet("menu-item/{menuItemId:int}/count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecipeCountByMenuItem(int menuItemId)
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

                var count = await _drinkRecipeRepository.GetRecipeCountByMenuItemAsync(menuItemId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Lấy số lượng công thức thành công",
                    Data = new { MenuItemId = menuItemId, RecipeCount = count }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe count by menu item: {MenuItemId}", menuItemId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy số lượng công thức"
                });
            }
        }

        /// <summary>
        /// Lấy thống kê công thức theo nguyên liệu - Chỉ Admin
        /// </summary>
        [HttpGet("ingredient/{ingredientId:int}/count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecipeCountByIngredient(int ingredientId)
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

                var count = await _drinkRecipeRepository.GetRecipeCountByIngredientAsync(ingredientId);

                return Ok(new ApiResponse<object>
                {
                    IsSuccess = true,
                    Message = "Lấy số lượng công thức sử dụng nguyên liệu thành công",
                    Data = new { IngredientId = ingredientId, RecipeCount = count }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe count by ingredient: {IngredientId}", ingredientId);
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy số lượng công thức"
                });
            }
        }

        /// <summary>
        /// Lấy tổng quan thống kê công thức - Chỉ Admin
        /// </summary>
        [HttpGet("statistics/overview")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRecipeOverviewStatistics()
        {
            try
            {
                var allRecipes = await _drinkRecipeRepository.GetAllAsync();

                var overview = new RecipeOverviewStatisticsResponse
                {
                    TotalRecipes = allRecipes.Count,
                    TotalMenuItems = allRecipes.Select(r => r.MenuItemId).Distinct().Count(),
                    TotalIngredients = allRecipes.Select(r => r.IngredientId).Distinct().Count(),
                    AverageIngredientsPerMenuItem = allRecipes.Count > 0 ?
                        (decimal)allRecipes.Count / allRecipes.Select(r => r.MenuItemId).Distinct().Count() : 0,
                    MostUsedIngredient = allRecipes
                        .GroupBy(r => r.Ingredient?.Name)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key ?? "N/A",
                    ComplexMenuItem = allRecipes
                        .GroupBy(r => r.MenuItem?.Name)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key ?? "N/A"
                };

                return Ok(new ApiResponse<RecipeOverviewStatisticsResponse>
                {
                    IsSuccess = true,
                    Message = "Lấy tổng quan thống kê công thức thành công",
                    Data = overview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe overview statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy tổng quan thống kê công thức"
                });
            }
        }

        // ==================== HELPER METHODS ====================

        private decimal CalculateIngredientCost(int quantityGram, decimal unitPrice)
        {
            // Assuming unit price is per 100g, adjust calculation as needed
            return (quantityGram / 100m) * unitPrice;
        }
    }
}
