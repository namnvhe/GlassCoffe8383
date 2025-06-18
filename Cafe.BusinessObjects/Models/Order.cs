using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int TableId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderTime { get; set; }

    public string Status { get; set; } = null!;

    public int? DiscountId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual CoffeeTable Table { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
