using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IMenuService
{
    List<Category> GetAllCategories();
    Task<bool> AddCategory(Category category, long userId);
    Task<bool> EditCategory(Category category, long catID, long userId);
    Task<bool> DeleteCategory(long catID);
    PaginationViewModel<ItemsViewModel> GetItemsByCategory(long? catID, string search = "", int pageNumber = 1, int pageSize = 5);
    Task<bool> AddItem(AddItemViewModel addItemvm, long userId);
    AddItemViewModel GetItemByItemID(long itemID);
    Task<bool> EditItem(AddItemViewModel editvm, long userId);
    Task<bool> DeleteItem(long itemID);


}
