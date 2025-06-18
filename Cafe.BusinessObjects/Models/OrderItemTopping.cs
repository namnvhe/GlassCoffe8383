﻿using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class OrderItemTopping
{
    public int OrderItemToppingId { get; set; }

    public int OrderItemId { get; set; }

    public int ToppingId { get; set; }

    public int Quantity { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual Topping Topping { get; set; } = null!;
}
