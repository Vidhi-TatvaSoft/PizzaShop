using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DAL.ViewModels;

public class AddModifierViewModel
{
    public long ModifierId { get; set; }
    [Required]
    public string ModifierName { get; set; } = null!;

    public long ModifierGrpId { get; set; }

    public string? Unit { get; set; }
    public decimal? Rate { get; set; }
    public int Quantity { get; set; }
    public string Description{get;set;}
}
