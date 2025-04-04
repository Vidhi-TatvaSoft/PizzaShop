using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Service;
using BLL.Interfaces;

// using BLL.Services;

using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Pizzashop_Project.Authorization;
using Newtonsoft.Json;

namespace Pizzashop_Project.Controllers;
// [Authorize(Roles = "Admin")]
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
    [PermissionAuthorize("Menu.View")]
    public IActionResult Menu(long? catID, long? modifierGrpID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();

        // categories----------------------
        menudata.categories = _menuService.GetAllCategories();
        if (catID == null)
        {
            menudata.Pagination = _menuService.GetItemsByCategory(menudata.categories[0].CategoryId, search, pageNumber, pageSize);
        }

        // modifiers---------------------------
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();
        if (modifierGrpID == null)
        {
            menudata.Paginationmodifiers = _menuService.GetModifiersByModifierGrp(menudata.modifiergroupList[0].ModifierGrpId, search, pageNumber, pageSize);
        }
        menudata.additem=menudata.additem??new AddItemViewModel();
        menudata.additem.ModifierGroupList = menudata.additem.ModifierGroupList ?? new List<ModifierGroupForItem>();
        ViewData["sidebar-active"] = "Menu";
        return View(menudata);
    }
    #endregion

    #region menniItemPagination
    [PermissionAuthorize("Menu.View")]
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
    [PermissionAuthorize("Menu.View")]
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

    #region menu modifiers Pagination
    [PermissionAuthorize("Menu.View")]
    public IActionResult MenuModifierAllPagination(string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();

        menudata.Paginationmodifiers = _menuService.GetAllModifiers(search, pageNumber, pageSize);

        return PartialView("_AddExisingModifierPaginationPartial", menudata.Paginationmodifiers);
    }
    #endregion

    #region menu modifiers Pagination
    [PermissionAuthorize("Menu.View")]
    public IActionResult MenuModifierAllPaginationForEdit(string search = "", int pageNumber = 1, int pageSize = 5)
    {
        MenuViewModel menudata = new();
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();

        menudata.Paginationmodifiers = _menuService.GetAllModifiers(search, pageNumber, pageSize);

        return PartialView("_AddExisingModifierPaginationPartial", menudata.Paginationmodifiers);
    }
    #endregion

    #region menu list of modifier group get
    [PermissionAuthorize("Menu.View")]
    public IActionResult GetAllModifierGroups()
    {
        MenuViewModel menudata = new();
        menudata.modifiergroupList = _menuService.GetAllModifierGroups();
        return PartialView("_ModifierGroupListPartial", menudata);
    }
    #endregion

    #region Add Category 
    [PermissionAuthorize("Menu.EditAdd")]
    public async Task<IActionResult> AddCategory(MenuViewModel menuvm)
    {
        bool CatNamePresent = _menuService.IsCategoryNameExist(menuvm);
        if(CatNamePresent){
            TempData["ErrorMessage"] = "CategoryName Already Present. Try Another Name";
            return RedirectToAction("Menu");
        }
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
    [PermissionAuthorize("Menu.EditAdd")]
    public async Task<IActionResult> EditCategory(MenuViewModel menuvm)
    {
        if(_menuService.IsCategoryNameExistForEdit(menuvm.category)){
            TempData["ErrorMessage"] = "CategoryName Already Present. Try Another Name";
            return RedirectToAction("Menu");
        }
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
    [PermissionAuthorize("Menu.Delete")]
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

    #region get all modifiers from modifier id
    public IActionResult getAllmodifiersByModifierGroupId(long modGrpID)
    {
        MenuViewModel menuvm = new();
        ModifierGroupForItem modGrpDetails = new();
        // modGrpDetails.modifierList = _menuService.getAllmodifiersByModifierGroupId(modGrpID);

        return PartialView("_ModifierGroupInItemPartial", menuvm);
    }
    #endregion

    #region get modifier by mdifier grp for item
    public IActionResult GetModifiersByGroup(string data)
    {
        MenuViewModel mVM = new MenuViewModel();
        List<ModifierGroupForItem> deserializedData = JsonConvert.DeserializeObject<List<ModifierGroupForItem>>(data);

        if (deserializedData != null)
        {
            mVM.additem = mVM.additem ?? new AddItemViewModel();
            mVM.additem.ModifierGroupList = mVM.additem.ModifierGroupList ?? new List<ModifierGroupForItem>();
            var i = 0;
            foreach (ModifierGroupForItem deItems in deserializedData)
            {
                mVM.additem.ModifierGroupList.Add(deItems);
                mVM.additem.ModifierGroupList[i].modifierList = _menuService.GetModifiersByGroup(deItems.ModifierGrpId);
                mVM.additem.ModifierGroupList[i].ModifierGrpName = _menuService.GetModifiersGroupName(deItems.ModifierGrpId);
                i++;
            }
        }
        mVM.categories = _menuService.GetAllCategories();
        mVM.modifiergroupList = _menuService.GetAllModifierGroups();

        return PartialView("_ModifierGroupInItemPartial", mVM);
    }

    #endregion


    #region GetModifierGroupList to fill dropDown od item
    public IActionResult GetModifierGroupList()
    {
        MenuViewModel menuvm = new MenuViewModel();
        menuvm.modifiergroupList = _menuService.GetAllModifierGroups();
        return Json(menuvm.modifiergroupList);
    }

    #endregion

    #region AddItems
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> AddItem(MenuViewModel MenuViewModel)
    {
        if(_menuService.IsItemNameExist(MenuViewModel.additem)){
            return Json(new { success = false, text = "ItemName Already Present. Try Another Name" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        List<ModifierGroupForItem> deserializedData = JsonConvert.DeserializeObject<List<ModifierGroupForItem>>(MenuViewModel.itemData);

        if (deserializedData != null)
        {
            MenuViewModel.additem = MenuViewModel.additem ?? new AddItemViewModel();
            MenuViewModel.additem.ModifierGroupList = MenuViewModel.additem.ModifierGroupList ?? new List<ModifierGroupForItem>();

            foreach (ModifierGroupForItem deItems in deserializedData)
            {
                MenuViewModel.additem.ModifierGroupList.Add(deItems);
            }
        }
        if (MenuViewModel.additem.ItemFormImage != null)
        {

            var extension = MenuViewModel.additem.ItemFormImage.FileName.Split(".");
            if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string fileName = BLL.Common.ImageUpload.UploadImage(MenuViewModel.additem.ItemFormImage, path);
                MenuViewModel.additem.ItemImage = $"/uploads/{fileName}";
            }
            else
            {
                return Json(new{ success = false, text = "Please Upload an Image in JPEG, PNG or JPG format." });
            }
        }

        var addItemStatus = await _menuService.AddItem(MenuViewModel.additem, userId);
        if (addItemStatus)
        {
            // TempData["SuccessMessage"] = "Item Added SuccessFully.";
            return Json(new { success = true, text = "Item Added SuccessFully." });
        }
        // TempData["ErrorMessage"] = "Error while ItemAdd. Try Again..";
        return Json(new { success = false, text = "Error while ItemAdd. Try Again.." });
    }
    #endregion

    #region  edit item Get
    [PermissionAuthorize("Menu.EditAdd")]
    public IActionResult EditItem(long itemID)
    {
        return Json(_menuService.GetItemByItemID(itemID));
    }
    #endregion

    #region EditModifiersByGroup for item
    [HttpPost]
    [PermissionAuthorize("Menu.EditAdd")]
    public IActionResult EditModifiersByGroup(string data)
    {
        MenuViewModel mVM = new MenuViewModel();
        List<ModifierGroupForItem> deserializedData = JsonConvert.DeserializeObject<List<ModifierGroupForItem>>(data);

        if (deserializedData != null)
        {
            mVM.additem = mVM.additem ?? new AddItemViewModel();
            mVM.additem.ModifierGroupList = mVM.additem.ModifierGroupList ?? new List<ModifierGroupForItem>();
            var i = 0;
            try{

            foreach (ModifierGroupForItem deItems in deserializedData)
            {
                mVM.additem.ModifierGroupList.Add(deItems);
                mVM.additem.ModifierGroupList[i].modifierList = _menuService.GetModifiersByGroup(deItems.ModifierGrpId);
                mVM.additem.ModifierGroupList[i].ModifierGrpName = _menuService.GetModifiersGroupName(deItems.ModifierGrpId);
                i++;
            }
            }catch(Exception e){

            }
        }

        mVM.categories = _menuService.GetAllCategories();
        mVM.modifiergroupList = _menuService.GetAllModifierGroups();
        return PartialView("_EditModifierGroupInItemPartial", mVM);
    }
    #endregion

    #region  edititem post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> EditItem([FromForm]MenuViewModel menuvm)
    {
        if(_menuService.IsItemNameExistForEdit(menuvm.additem)){
            return Json(new { success = false, text = "ItemName Already Present. Try Another Name" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
         List<ModifierGroupForItem> deserializedData = JsonConvert.DeserializeObject<List<ModifierGroupForItem>>(menuvm.itemData);

        if (deserializedData != null)
        {
            menuvm.additem = menuvm.additem ?? new AddItemViewModel();
            menuvm.additem.ModifierGroupList = menuvm.additem.ModifierGroupList ?? new List<ModifierGroupForItem>();

            foreach (ModifierGroupForItem deItems in deserializedData)
            {
                menuvm.additem.ModifierGroupList.Add(deItems);
            }
        }
        if (menuvm.additem.ItemFormImage != null)
        {
            var extension = menuvm.additem.ItemFormImage.FileName.Split(".");
            if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string fileName = BLL.Common.ImageUpload.UploadImage(menuvm.additem.ItemFormImage, path);

                menuvm.additem.ItemImage = $"/uploads/{fileName}";
            }
            else
            {
                // TempData["ErrorMessage"] = "Please Upload an Image in JPEG, PNG or JPG format.";
                return Json(new { success = false, text = "Please Upload an Image in JPEG, PNG or JPG format." });
            }
        }
        if (await _menuService.EditItem(menuvm.additem, userId))
        {
            // TempData["SuccessMessage"] = "Item Updated Successfully";
            return Json(new { success = true, text = "Item Updated Successfully" });
        }
        // TempData["ErrorMessage"] = "Failed to Update Item. Try Again!";
        return Json(new { success = false, text = "Failed to Update Item. Try Again!" });

    }
    #endregion

    #region delete item
    [PermissionAuthorize("Menu.Delete")]
    public async Task<IActionResult> DeleteItem(long itemID)
    {
        var CategoryDeleteStatus = await _menuService.DeleteItem(itemID);
        if (CategoryDeleteStatus)
        {
            // TempData["SuccessMessage"] = "Item Deleted Successfully";
            return Json(new { success = true, text = "Item Deleted Successfully" });
        }
        // TempData["ErrorMessage"] = "Failed to delete Item. Try Again";
        return Json(new { success = false, text = "Failed to delete Item. Try Again" });
    }
    #endregion

    #region AddModifier get
    [PermissionAuthorize("Menu.EditAdd")]
    public IActionResult AddModifierModal()
    {
        MenuViewModel menuvm = new MenuViewModel();
        menuvm.modifiergroupList = _menuService.GetAllModifierGroups();
        return PartialView("_AddModifierModal", menuvm);
    }
    #endregion

    #region Add Modifier post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> AddModifier([FromForm] MenuViewModel menuvm)
    {
        // if(_menuService.IsModifierNameExist(menuvm.addModifier)){
        //     return Json(new { success = false, text = "ModifierName Already Present. Try Another Name" });
        // }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        var addItemStatus = await _menuService.AddModifier(menuvm.addModifier, userId);
        if (addItemStatus)
        {
            // TempData["SuccessMessage"] = "Item Added SuccessFully.";
            return Json(new { success = true, text = "Modifier Added Successfully" });
        }
        // TempData["ErrorMessage"] = "Error while ItemAdd. Try Again..";
        return Json(new { success = false, text = "Error while Modifier add. Try Again.." });
    }
    #endregion

    #region GetModifierDetailsByModifierId
    [PermissionAuthorize("Menu.EditAdd")]
    public IActionResult GetModifierDetailsByModifierId(long modID)
    {
        MenuViewModel MenuVM = new MenuViewModel();
        var ModifierGroupList = _menuService.GetAllModifierGroups();
        ViewBag.modifierGroupList = new SelectList(ModifierGroupList, "ModifierGrpId", "ModifierGrpName");
        MenuVM.addModifier = _menuService.GetModifierDetailsByModifierId(modID);
        return PartialView("_UpdateModifierModalPartial", MenuVM);
    }
    #endregion

    #region EditModifier
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> EditModifier([FromForm] MenuViewModel Menuvm)
    {
        // if(_menuService.IsModifierNameExistForEdit(Menuvm.addModifier)){
        //     return Json(new { success = false, text = "ModifierName Already Present. Try Another Name" });
        // }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        var editModifierStatus = await _menuService.EditModifier(Menuvm.addModifier, userId);

        if (editModifierStatus)
        {
            // TempData["SuccessMessage"] = "Modifier Updated successfully";
            return Json(new { success = true, text = "Modifier Updated successfully" });
        }
        // TempData["ErrorMessage"] = "Failed to Update Modifier";
        return Json(new { success = false, text = "Failed to Update Modifier" });
    }

    #endregion

    #region Delete Modifier post
    [Authorize(Roles = "Admin")]
    [PermissionAuthorize("Menu.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteModifier(long modID)
    {
        var isDeleted = await _menuService.DeleteModifier(modID);

        if (isDeleted)
        {
            // TempData["SuccessMessage"] = "Modifier deleted successfully";
            return Json(new { success = true, text = "Modifier deleted successfully" });
        }
        // TempData["ErrorMessage"] = "Modifier cannot be deleted";
        return Json(new { success = false, text = "Modifier cannot be deleted" });
    }
    #endregion

    #region AddModifierGroup post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(MenuViewModel menuvm)
    {
        List<Modifiergroup> modgrplist = _menuService.GetAllModifierGroups();
        if(_menuService.IsModifierGroupNameExist(menuvm.addmodgrpVm)){
            return Json(new {modgrpid = modgrplist[0].ModifierGrpId, success = false, text = "ModifierGroupName Already Present. Try Another Name" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var addModifierGrpStatus = await _menuService.AddModifierGroup(menuvm.addmodgrpVm, userId);
        if (addModifierGrpStatus)
        {
            return Json(new {modgrpid = modgrplist[0].ModifierGrpId, success = true, text = "modifierGroup added successfully" });
        }
        return Json(new {modgrpid = modgrplist[0].ModifierGrpId, success = false, text = "Error while Add Modifier group. Try Again!" });
    }
    #endregion

    #region Edit Modifier Group get
    [PermissionAuthorize("Menu.EditAdd")]
    public IActionResult EditModGrp(long ModGrpId)
    {
        var modifiers = _menuService.GetModifiersByModifierGrpId(ModGrpId);
        var modifierGroup = _menuService.GetModifiergroupByGrpID(ModGrpId);

        return Json(new { modifiers, modifierGroup });

    }
    #endregion

    #region editmod grp post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> EditModifierGroup(MenuViewModel menuvm)
    {
        List<Modifiergroup> modgrplist = _menuService.GetAllModifierGroups();
        if(_menuService.IsModifierGroupNameExistForEdit(menuvm.addmodgrpVm)){
            return Json(new { firstmodgrpid = modgrplist[0].ModifierGrpId, success = false, text = "ModifierGroupName Already Present. Try Another Name" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var editModStatus = await _menuService.EditModifierGroup(menuvm.addmodgrpVm, userId);
        if (editModStatus)
        {
            return Json(new { grpId = menuvm.addmodgrpVm.ModifierGrpId,firstmodgrpid = modgrplist[0].ModifierGrpId, success = true, text = "modifierGroup Updated successfully" });
        }
        else
        {
            return Json(new  { firstmodgrpid = modgrplist[0].ModifierGrpId, success = false, text = "modifierGroup not Updated.Try Again!" });
        }
    }
    #endregion

    //deleteModifierFromModGrpAfterEdit?modGrpID=${modGrpID}&modifierID=${editModTempId[i]}
    #region deleteModifierFromModGrpAfterEdit post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> DeleteModifierFromModGrpAfterEdit(long modGrpID, long modifierID)
    {
        var deleteModStatus = await _menuService.DeleteModifierFromModGrpAfterEdit(modGrpID, modifierID);
        if (deleteModStatus)
        {
            return Json("existing modifier deleted success while edit mod grp");
        }
        return Json("fail to delete existing mod in modgrp while edit");
    }
    #endregion

    //addModifierToModGrpAfterEdit?modGrpID=${modGrpID}&modifierID=${modTempID[i]}
    #region addModifierToModGrpAfterEdit post
    [PermissionAuthorize("Menu.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> addModifierToModGrpAfterEdit(long modGrpID, long modifierID)
    {
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var addModStatus = await _menuService.AddModifierToModGrpAfterEdit(modGrpID, modifierID, userId);
        if (addModStatus)
        {
            return Json("existing modifier added success while edit mod grp");
        }
        return Json("fail to add existing mod in modgrp while edit");
    }
    #endregion


    #region Delete Mod Grp
    [PermissionAuthorize("Menu.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteModGrp(long modGrpid)
    {
        var deletemodgrpStatus = await _menuService.DeleteModifierGroup(modGrpid);
        List<Modifiergroup> modgrplist = _menuService.GetAllModifierGroups();
        if (deletemodgrpStatus)
        {
            return Json(new {modgrpid = modgrplist[0].ModifierGrpId, success = true, text = "modifier group deleted successfully" });
        }
        return Json(new { modgrpid = modgrplist[0].ModifierGrpId, success = false, text = "modifier group not deleted" });
    }
    #endregion


}