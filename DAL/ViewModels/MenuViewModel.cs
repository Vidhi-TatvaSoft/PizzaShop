using DAL.Models;


namespace DAL.ViewModels;

public class MenuViewModel
{
    public List<Category> categories {get; set;}

    public Category category{get; set;}

    // public List<ItemsViewModel> itemList{get;set;}

    // public ItemsViewModel item {get;set;}

    public PaginationViewModel<ItemsViewModel> Pagination { get; set; }

    public AddItemViewModel additem {get; set;}

    public List<Modifiergroup> modifiergroupList {get;set;}

    public Modifiergroup modifiergroup{get;set;}

     public ModifierViewModel modifier{get;set;}

    public AddModifierViewModel addModifier {get; set;}

    public PaginationViewModel<ModifierViewModel> Paginationmodifiers { get; set; }

    public AddModifierGroupViewModel addmodgrpVm{get;set;}

    public string itemData{get;set;}

    // public List<ModifierViewModel> ModifiersList {get;set;}

    // public List<ModifierGroupForItem> modifierGroupForItemList{get;set;}
    // public Modifier modifier{get;set;}


}