using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class MenuItemImage
{
    public int ImageId { get; set; }

    public int MenuItemId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ImageName { get; set; }

    public bool IsMainImage { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;
}
