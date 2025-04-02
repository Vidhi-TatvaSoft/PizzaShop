using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class ModifiersForItemInInvoiceOrderDetails
{
    public long ModifierId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Modifier Name cannot exceed 50 characters.")]
    public string ModifierName { get; set; }

    [Range(0, 999, ErrorMessage = "Rate should be Positive and cannot exceed 3 digit")]
    public decimal? Rate { get; set; }

    [Range(0, 99, ErrorMessage = "Quantity should be Positive and cannot exceed 2 digit")]
    public int Quantity { get; set; }

    public decimal TotalOfModifierByQuantity { get; set; }
    
    public bool Isdelete { get; set; }
}
