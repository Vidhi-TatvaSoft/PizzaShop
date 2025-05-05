using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Taxinvoicemapping
{
    public long TaxinvoicemappingId { get; set; }

    public long InvoiceId { get; set; }

    public long TaxId { get; set; }

    public bool Isdelete { get; set; }

    public string TaxName { get; set; } = null!;

    public decimal TaxAmount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Tax Tax { get; set; } = null!;
}
