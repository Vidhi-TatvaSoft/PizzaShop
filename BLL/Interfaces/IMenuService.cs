using DAL.Models;

namespace BLL.Interfaces;

public interface IMenuService
{

     Task<bool> AddCategory(Category category);

     Task<bool> DeleteCategory(long catID);
}
