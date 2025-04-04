using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IMenuService
{
    List<Category> GetAllCategories();
    List<Modifiergroup> GetAllModifierGroups();
    public bool IsCategoryNameExist(MenuViewModel menuvm);
    Task<bool> AddCategory(Category category, long userId);

    public bool IsCategoryNameExistForEdit(Category category);
    Task<bool> EditCategory(Category category, long catID, long userId);
    Task<bool> DeleteCategory(long catID);
    PaginationViewModel<ItemsViewModel> GetItemsByCategory(long? catID, string search = "", int pageNumber = 1, int pageSize = 5);
    public bool IsItemNameExist(AddItemViewModel addItemvm);
    public bool IsItemNameExistForEdit(AddItemViewModel addItemvm);
    Task<bool> AddItem(AddItemViewModel addItemvm, long userId);
    public List<Modifier> GetModifiersByGroup(long groupId);
    public string GetModifiersGroupName(long groupId);
    AddItemViewModel GetItemByItemID(long itemID);
    Task<bool> EditItem(AddItemViewModel editvm, long userId);
    Task<bool> DeleteItem(long itemID);
    PaginationViewModel<ModifierViewModel> GetModifiersByModifierGrp(long? modifierGrpID, string search = "", int pageNumber = 1, int pageSize = 5);
    public PaginationViewModel<ModifierViewModel> GetAllModifiers( string search = "", int pageNumber = 1, int pageSize = 5);
    // public bool IsModifierNameExist(AddModifierViewModel addModifiervm); 
    // public bool IsModifierNameExistForEdit(AddModifierViewModel addModifiervm);
     Task<bool> AddModifier(AddModifierViewModel addModifiervm, long userId);
     AddModifierViewModel GetModifierDetailsByModifierId(long modID);
     
    Task<bool> EditModifier(AddModifierViewModel editModifiervm, long userId);
    Task<bool> DeleteModifier(long modID);
    public bool IsModifierGroupNameExist(AddModifierGroupViewModel modifiergrpvm);
    public bool IsModifierGroupNameExistForEdit(AddModifierGroupViewModel modifiergrpvm);
    Task<bool> AddModifierGroup(AddModifierGroupViewModel modifiergrpvm , long userID);
    List<ModifierViewModel> GetModifiersByModifierGrpId(long ModGrpId);
    public Modifiergroup GetModifiergroupByGrpID(long ModGrpId);
     Task<bool> EditModifierGroup(AddModifierGroupViewModel modifiergrpvm, long userID);
     Task<bool> DeleteModifierFromModGrpAfterEdit(long modID,long modGrpID);
     Task<bool> AddModifierToModGrpAfterEdit(long modGrpID,long modifierID,long UserID);
    Task<bool> DeleteModifierGroup(long modGrpid);

}