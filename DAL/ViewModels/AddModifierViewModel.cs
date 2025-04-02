using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DAL.ViewModels;

public class AddModifierViewModel
{
    public long ModifierId { get; set; }
    [Required]
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "ModifierName cannot exceed 50 characters.")]
    public string ModifierName { get; set; } = null!;

    public long ModifierGrpId { get; set; }

    public string? Unit { get; set; }

    [Range(0, 999, ErrorMessage = "Rate should be Positive and cannot exceed 3 digit")]
    public decimal? Rate { get; set; }

    [Range(0, 99, ErrorMessage = "Quantity should be Positive and cannot exceed 2 digit")]
    public int Quantity { get; set; }
    public string Description{get;set;}
}
