using System.ComponentModel.DataAnnotations;

namespace Cafe.BusinessObjects.Models.Request
{
    public class CreateDrinkRecipeRequest
    {
        [Required(ErrorMessage = "ID món ăn là bắt buộc")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "ID nguyên liệu là bắt buộc")]
        public int IngredientId { get; set; }

        [Required(ErrorMessage = "Số lượng tối thiểu là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối thiểu phải lớn hơn 0")]
        public int QuantityMinGram { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        public int QuantityMaxGram { get; set; }
    }

    public class UpdateDrinkRecipeRequest
    {
        [Required(ErrorMessage = "Số lượng tối thiểu là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối thiểu phải lớn hơn 0")]
        public int QuantityMinGram { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        public int QuantityMaxGram { get; set; }
    }

    public class UpdateRecipeQuantityRequest
    {
        [Required(ErrorMessage = "Số lượng tối thiểu là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối thiểu phải lớn hơn 0")]
        public int QuantityMinGram { get; set; }

        [Required(ErrorMessage = "Số lượng tối đa là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        public int QuantityMaxGram { get; set; }
    }
}
