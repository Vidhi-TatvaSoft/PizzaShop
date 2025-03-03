using System.Threading.Tasks;
using BLL.Service;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class MenuController : Controller
{

    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

#region  Menu get
    public IActionResult Menu()
    {
        MenuViewModel menudata = new();
        menudata.categories =  _menuService.GetAllCategories();
        return View(menudata);
    }
#endregion


#region Add Category 
    public async Task<IActionResult> AddCategory(Category category)
    {
        bool addcategoryStatus = await _menuService.AddCategory(category);
        if(addcategoryStatus){
            TempData["SuccessMessage"] = "Category Added Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to add Category. Try Again";
        return RedirectToAction("Menu");
    }
    #endregion

    #region EditCategory
    public async Task<IActionResult> EditCategory(Category category)
    {
        var catID = category.CategoryId;
        bool editCategoryStatus = await _menuService.EditCategory(category, catID);
        if(editCategoryStatus){
            TempData["SuccessMessage"] = "Category Added Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to add Category. Try Again";
        return RedirectToAction("Menu");

    }
    #endregion


    #region delete category
    public async Task<IActionResult> DeleteCategory(long id)
    {   
        var CategoryDeleteStatus =await _menuService.DeleteCategory(id);
        if(CategoryDeleteStatus){
            TempData["SuccessMessage"] = "Category Deleted Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to delete Category. Try Again";
        return RedirectToAction("Menu");
    }
    #endregion
}