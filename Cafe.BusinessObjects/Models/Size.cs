using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal ExtraPrice { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
