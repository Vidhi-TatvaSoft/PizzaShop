using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Table
{
    public long SectionId { get; set; }

    public string TableName { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public long TableId { get; set; }

    public virtual ICollection<Assigntable> Assigntables { get; } = new List<Assigntable>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual Section Section { get; set; } = null!;

    public virtual ICollection<Waitinglist> Waitinglists { get; } = new List<Waitinglist>();
}
