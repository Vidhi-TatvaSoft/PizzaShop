using System.Threading.Tasks;
using BLL.Service;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pizzashop_Project.Controllers;

public class MenuController : Controller
{

    private readonly MenuService _menuService;

    private readonly UserLoginService _userLoginSerivce;

    // private readonly UserService _userservice;

    public MenuController(MenuService menuService, UserLoginService userLoginSerivce)
    {
        _menuService = menuService;
        _userLoginSerivce = userLoginSerivce;
    }

    #region  Menu get
    public IActionResult Menu(long? catID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.categories = _menuService.GetAllCategories();
        if (catID == null)
        {
            // menudata.itemList = _menuService.GetItemsByCategory(-100).Items;
            ViewBag.catSelect = menudata.categories[0].CategoryId;
            menudata.Pagination = _menuService.GetItemsByCategory(menudata.categories[0].CategoryId, search, pageNumber, pageSize);
        }

        if (catID != null)
        {
            ViewBag.catSelect = catID;
            menudata.Pagination = _menuService.GetItemsByCategory(catID, search, pageNumber, pageSize);
        }

        ViewData["sidebar-active"] = "Menu";
        return View(menudata);
    }
    #endregion

    #region menniItemPagination
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
    #endregion


    #region Add Category 
    public async Task<IActionResult> AddCategory(Category category)
    {
        string email = Request.Cookies["Email"];
        long userId = _userLoginSerivce.GetUserId(email);
        bool addcategoryStatus = await _menuService.AddCategory(category, userId);
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
        string email = Request.Cookies["Email"];
        long userId = _userLoginSerivce.GetUserId(email);
        var catID = category.CategoryId;
        bool editCategoryStatus = await _menuService.EditCategory(category, catID,userId);
        if (editCategoryStatus)
        {
            TempData["SuccessMessage"] = "Category Edited Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to Edit Category. Try Again";
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
    

    #region AddItems
    [HttpPost]
    public async Task<IActionResult> AddItem(AddItemViewModel addItemViewModel)
    {

        // _userService.AddUser(user, Email);
        string email = Request.Cookies["Email"];
        long userId = _userLoginSerivce.GetUserId(email);

        if (addItemViewModel.ItemFormImage != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"{Guid.NewGuid()}_{addItemViewModel.ItemFormImage.FileName}";
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                addItemViewModel.ItemFormImage.CopyTo(stream);
            }
            addItemViewModel.ItemImage = $"/uploads/{fileName}";
        }

        var addItemStatus = await _menuService.AddItem(addItemViewModel,userId);
        if (addItemStatus)
        {
            TempData["SuccessMessage"] = "Item Added SuccessFully.";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Error while ItemAdd. Try Again..";
        return RedirectToAction("Menu");
    }
    #endregion


    public async Task<IActionResult> DeleteItem(long itemID){
         var CategoryDeleteStatus =await _menuService.DeleteItem(itemID);
        if (CategoryDeleteStatus)
        {
            TempData["SuccessMessage"] = "Category Deleted Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to delete Category. Try Again";
        return RedirectToAction("Menu");
    }

    


}