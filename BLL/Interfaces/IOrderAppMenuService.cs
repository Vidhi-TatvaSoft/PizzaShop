using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppMenuService
{
    
    public List<ItemsViewModel> GetItemByCategory(long categoryId, string searchText = "");

    Task<bool> FavouriteItemManage(long itemId, bool IsFavourite);

    public List<ModifierGroupForItem> GetModifiersByItemId(long itemId);
}
