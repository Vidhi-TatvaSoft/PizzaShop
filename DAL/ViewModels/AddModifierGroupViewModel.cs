using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class AddModifierGroupViewModel
{
    public long ModifierGrpId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Modifier Group Name cannot exceed 50 characters.")]
    public string ModifierGrpName { get; set; }

    public string? Desciption { get; set; }

    public string tempIds{get;set;}

}
