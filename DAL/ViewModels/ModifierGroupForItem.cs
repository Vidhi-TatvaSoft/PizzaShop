using DAL.Models;

namespace DAL.ViewModels;

public class ModifierGroupForItem
{
    public long ModifierGrpId { get; set; }

    public string ModifierGrpName { get; set; }

    public int min{get;set;}

    public int max{get;set;}

    public List<Modifier> modifierList{get;set;}

}
