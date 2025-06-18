using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class DrinkRecipe
{
    public int RecipeId { get; set; }

    public int MenuItemId { get; set; }

    public int IngredientId { get; set; }

    public int QuantityMinGram { get; set; }

    public int QuantityMaxGram { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;
}
