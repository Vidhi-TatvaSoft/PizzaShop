using System.Threading.Tasks;
using BLL.Service;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class MenuController : Controller
{

    private readonly MenuService _menuService;

    // private readonly UserService _userservice;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    #region  Menu get
    public IActionResult Menu(long? catID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.categories = _menuService.GetAllCategories();
        if (catID == null)
        {
            // menudata.itemList = _menuService.GetItemsByCategory(-100).Items;
            menudata.Pagination = _menuService.GetItemsByCategory(menudata.categories[0].CategoryId, search, pageNumber, pageSize);
        }

        if (catID != null)
        {
            menudata.Pagination = _menuService.GetItemsByCategory(catID, search, pageNumber, pageSize);
        }
        return View(menudata);
    }
    #endregion

    public IActionResult MenuItemPagination(long? catID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.categories = _menuService.GetAllCategories();

        if (catID != null)
        {
            menudata.Pagination = _menuService.GetItemsByCategory(catID, search, pageNumber, pageSize);
        }
        return PartialView("_ItemListPartal", menudata.Pagination);
    }


    #region Add Category 
    public async Task<IActionResult> AddCategory(Category category)
    {
        bool addcategoryStatus = await _menuService.AddCategory(category);
        if (addcategoryStatus)
        {
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
        if (editCategoryStatus)
        {
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
        var CategoryDeleteStatus = await _menuService.DeleteCategory(id);
        if (CategoryDeleteStatus)
        {
            TempData["SuccessMessage"] = "Category Deleted Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to delete Category. Try Again";
        return RedirectToAction("Menu");
    }
    #endregion

    #region items list

    #endregion



}