using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class CoffeeTable
{
    public int TableId { get; set; }

    public int TableNumber { get; set; }

    public string Qrcode { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
