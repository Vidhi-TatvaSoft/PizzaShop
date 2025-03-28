using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Assigntable
{
    public int AssignId { get; set; }

    public long CustomerId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public long TableId { get; set; }

    public long OrderId { get; set; }

    public long NoOfPerson { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
