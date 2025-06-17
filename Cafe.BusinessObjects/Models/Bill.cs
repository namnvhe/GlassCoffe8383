using System;
using System.Collections.Generic;

namespace Cafe.BusinessObjects.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public int OrderId { get; set; }

    public decimal TotalAmount { get; set; }

    public string Qrcode { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public DateTime GeneratedTime { get; set; }

    public virtual Order Order { get; set; } = null!;
}
