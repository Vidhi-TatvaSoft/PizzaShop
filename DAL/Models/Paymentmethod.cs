using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Paymentmethod
{
    public long PaymentmethodId { get; set; }

    public string Paymenttype { get; set; } = null!;

    public bool Isdelete { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
