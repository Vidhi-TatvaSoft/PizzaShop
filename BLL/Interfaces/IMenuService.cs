using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IMenuService
{

    Task<bool> AddCategory(Category category, long userId);

    Task<bool> EditCategory(Category category, long catID, long userId);

    Task<bool> DeleteCategory(long catID);

    Task<bool> AddItem(AddItemViewModel addItemvm,long userId);
}
