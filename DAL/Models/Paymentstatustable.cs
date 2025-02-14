using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Paymentstatustable
{
    public long PaymentstatusId { get; set; }

    public string Paymentstatus { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
