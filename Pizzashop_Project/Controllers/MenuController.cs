using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Service;
using BLL.Service.Interfaces;

// using BLL.Services;

using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pizzashop_Project.Controllers;
[Authorize(Roles = "Admin")]
public class MenuController : Controller
{

    private readonly IMenuService _menuService;

    private readonly IUserLoginService _userLoginSerivce;

    private readonly IUserService _userService;

    // private readonly UserService _userservice;

    public MenuController(IMenuService menuService, IUserLoginService userLoginSerivce, IUserService userService)
    {
        _menuService = menuService;
        _userLoginSerivce = userLoginSerivce;
        _userService = userService;
    }

    #region  Menu get
    public IActionResult Menu(long? catID, long? modifierGrpID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();

        // categories----------------------
        menudata.categories = _menuService.GetAllCategories();
        if (catID == null)
        {
            // menudata.itemList = _menuService.GetItemsByCategory(-100).Items;
            // ViewBag.catSelect = menudata.categories[0].CategoryId;
            menudata.Pagination = _menuService.GetItemsByCategory(menudata.categories[0].CategoryId, search, pageNumber, pageSize);
        }

        // if (catID != null)
        // {
        //     // ViewBag.catSelect = catID;
        //     menudata.Pagination = _menuService.GetItemsByCategory(catID, search, pageNumber, pageSize);
        // }

        // modifiers---------------------------
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();
        if (modifierGrpID == null)
        {
            menudata.Paginationmodifiers = _menuService.GetModifiersByModifierGrp(menudata.modifiergroupList[0].ModifierGrpId, search, pageNumber, pageSize);
        }
        // if(modifierGrpID != null)
        // {
        //     menudata.Paginationmodifiers = _menuService.GetModifiersByModifierGrp(modifierGrpID,search,pageNumber,pageSize);
        // }

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

    #region menu modifiers Pagination
    public IActionResult MenuModifierPagination(long? modifierGrpID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();

        if (modifierGrpID != null)
        {
            menudata.Paginationmodifiers = _menuService.GetModifiersByModifierGrp(modifierGrpID, search, pageNumber, pageSize);
        }
        return PartialView("_ModifierListPartial", menudata.Paginationmodifiers);
    }
    #endregion


    #region Add Category 
    public async Task<IActionResult> AddCategory(MenuViewModel menuvm)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addcategoryStatus = await _menuService.AddCategory(menuvm.category, userId);
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
    public async Task<IActionResult> EditCategory(MenuViewModel menuvm)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var catID = menuvm.category.CategoryId;
        bool editCategoryStatus = await _menuService.EditCategory(menuvm.category, catID, userId);
        if (editCategoryStatus)
        {
            TempData["SuccessMessage"] = "Category Updated Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to Update Category. Try Again";
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
    public async Task<IActionResult> AddItem(MenuViewModel addItemViewModel)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        if (addItemViewModel.additem.ItemFormImage != null)
        {

            var extension = addItemViewModel.additem.ItemFormImage.FileName.Split(".");
            if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileName = $"{Guid.NewGuid()}_{addItemViewModel.additem.ItemFormImage.FileName}";
                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    addItemViewModel.additem.ItemFormImage.CopyTo(stream);
                }
                addItemViewModel.additem.ItemImage = $"/uploads/{fileName}";
            }
            else
            {
                TempData["ErrorMessage"] = "Please Upload an Image in JPEG, PNG or JPG format.";
                return RedirectToAction("AddItem", "Menu");
            }
        }

        var addItemStatus = await _menuService.AddItem(addItemViewModel.additem, userId);
        if (addItemStatus)
        {
            TempData["SuccessMessage"] = "Item Added SuccessFully.";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Error while ItemAdd. Try Again..";
        return RedirectToAction("Menu");
    }
    #endregion


    #region  edit item Get
    public IActionResult EditItem(long itemID)
    {
        return Json(_menuService.GetItemByItemID(itemID));
    }
    #endregion


    #region  edititem post
    [HttpPost]
    public async Task<IActionResult> EditItem(MenuViewModel menuvm)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        if (menuvm.additem.ItemFormImage != null)
        {
            var extension = menuvm.additem.ItemFormImage.FileName.Split(".");
            if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileName = $"{Guid.NewGuid()}_{menuvm.additem.ItemFormImage.FileName}";
                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    menuvm.additem.ItemFormImage.CopyTo(stream);
                }
                menuvm.additem.ItemImage = $"/uploads/{fileName}";
            }
            else
            {
                TempData["ErrorMessage"] = "Please Upload an Image in JPEG, PNG or JPG format.";
                return RedirectToAction("AddItem", "Menu");
            }
        }
        if (await _menuService.EditItem(menuvm.additem, userId))
        {
            TempData["SuccessMessage"] = "Item Updated Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to Update Item. Try Again!";
        return RedirectToAction("Menu");

    }
    #endregion

    #region delete item
    public async Task<IActionResult> DeleteItem(long itemID)
    {
        var CategoryDeleteStatus = await _menuService.DeleteItem(itemID);
        if (CategoryDeleteStatus)
        {
            TempData["SuccessMessage"] = "Category Deleted Successfully";
            return RedirectToAction("Menu");
        }
        TempData["ErrorMessage"] = "Failed to delete Category. Try Again";
        return RedirectToAction("Menu");
    }
    #endregion

    #region AddModifier get
    public IActionResult AddModifierModal()
    {
        MenuViewModel menuvm=new MenuViewModel();
         menuvm.modifiergroupList = _menuService.GetAllModifierGroups();
        return PartialView("_AddModifierModal", menuvm);
    }
    #endregion


    #region Add Modifier post
    [HttpPost]
    public async Task<IActionResult> AddModifier([FromForm] MenuViewModel menuvm)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        var addItemStatus = await _menuService.AddModifier(menuvm.addModifier, userId);
        if (addItemStatus)
        {
            TempData["SuccessMessage"] = "Item Added SuccessFully.";
            return Json("done");
        }
        TempData["ErrorMessage"] = "Error while ItemAdd. Try Again..";
        return Json(" not done");
    }
    #endregion

}