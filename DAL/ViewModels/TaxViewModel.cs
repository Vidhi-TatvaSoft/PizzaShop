using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class TaxViewModel
{
    public long TaxId { get; set; }
    [Required]
    public string TaxName { get; set; } = null!;

    public string TaxType { get; set; } = null!;

    public decimal TaxValue { get; set; }

    public bool Isenable { get; set; }

    public bool Isdefault { get; set; }

    public bool Isdelete { get; set; }
}
