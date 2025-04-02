using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models;

public partial class Tax
{
    public long TaxId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
        [StringLength(20, ErrorMessage = "Tax Name cannot exceed 20 characters.")]
    public string TaxName { get; set; } = null!;

    public string TaxType { get; set; } = null!;

    [Range(0, 999, ErrorMessage = "TaxValue should be Positive and cannot exceed 3 digit")]
    public decimal TaxValue { get; set; }

    public bool? Isenable { get; set; }

    public bool Isdefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? ModifiedBy { get; set; }

    public bool Isdelete { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual ICollection<Taxinvoicemapping> Taxinvoicemappings { get; } = new List<Taxinvoicemapping>();
}
