using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Itemmodifiergroupmapping
{
    public long ItemmodifiergroupmappingId { get; set; }

    public long ItemId { get; set; }

    public long? ModifierGrpId { get; set; }

    public int Minvalue { get; set; }

    public int Maxvalue { get; set; }

    public bool Isdelete { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Modifiergroup? ModifierGrp { get; set; }
}
