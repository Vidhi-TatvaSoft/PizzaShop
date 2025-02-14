using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int? Phoneno { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public virtual ICollection<Assigntable> Assigntables { get; } = new List<Assigntable>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Invoice> Invoices { get; } = new List<Invoice>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<Rating> Ratings { get; } = new List<Rating>();

    public virtual ICollection<Waitinglist> Waitinglists { get; } = new List<Waitinglist>();
}
