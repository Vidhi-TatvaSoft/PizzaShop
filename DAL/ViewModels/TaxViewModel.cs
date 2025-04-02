using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class TaxViewModel
{
    public long TaxId { get; set; }
    [Required]
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(20, ErrorMessage = "Tax Name cannot exceed 20 characters.")]
    public string TaxName { get; set; } = null!;

    public string TaxType { get; set; } = null!;

    [Range(0, 999, ErrorMessage = "Tax Value should be Positive and cannot exceed 3 digit")]
    public decimal TaxValue { get; set; }

    public bool Isenable { get; set; }

    public bool Isdefault { get; set; }

    public bool Isdelete { get; set; }
}
