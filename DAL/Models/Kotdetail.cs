using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Kotdetail
{
    public long KotdetailId { get; set; }

    public long KotId { get; set; }

    public long OrderdetailId { get; set; }

    public int PendingItem { get; set; }

    public int ReadyItem { get; set; }

    public bool Isdelete { get; set; }

    public virtual Kot Kot { get; set; } = null!;

    public virtual Orderdetail Orderdetail { get; set; } = null!;
}
