using System;
using System.Collections.Generic;

namespace ECommerceAPI.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentType { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
