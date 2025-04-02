using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Modifiergroup
{
    public long ModifierGrpId { get; set; }
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Modifier group Name cannot exceed 50 characters.")]
    public string ModifierGrpName { get; set; } = null!;

    public string? Desciption { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Itemmodifiergroupmapping> Itemmodifiergroupmappings { get; } = new List<Itemmodifiergroupmapping>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Modifier> Modifiers { get; } = new List<Modifier>();
}
