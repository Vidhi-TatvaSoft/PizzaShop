using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace DAL.ViewModels;

public class ModifierGroupForItem
{
    public long ModifierGrpId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Modifier Group Name cannot exceed 50 characters.")]
    public string ModifierGrpName { get; set; }

    public int min{get;set;}

    public int max{get;set;}

    public List<Modifier> modifierList{get;set;}

    public long TypeId{get;set;}

}
