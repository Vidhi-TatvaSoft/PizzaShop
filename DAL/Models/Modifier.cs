using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Modifier
{
    public long ModifierId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    public string ModifierName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public decimal? Rate { get; set; }

    public int Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public long ModifierGrpId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Modifiergroup ModifierGrp { get; set; } = null!;

    public virtual ICollection<Modifierorder> Modifierorders { get; } = new List<Modifierorder>();
}
