using System.Net.Sockets;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Service;
using BLL.Services;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;

public class TableAndSectionController : Controller
{
    private readonly ITableAndSection _tableSectionService;
    private readonly IUserService _userService;
    private readonly IUserLoginService _userLoginSerivce;

    public TableAndSectionController(ITableAndSection tableAndSection,IUserService userService, IUserLoginService userLoginService)
    {
        _tableSectionService = tableAndSection;
        _userService = userService;
        _userLoginSerivce = userLoginService;
    }

    [PermissionAuthorize("TableSection.View")]
    public IActionResult TableAndSection(long? SectionID, string search = "", int pageNumber = 1, int pageSize = 5){
        TableSectionViewModel tablesectionvm = new();

        // categories----------------------
        tablesectionvm.sectionList = _tableSectionService.GetAllSections();
        if (SectionID == null)
        {
            ViewBag.selectedSection = tablesectionvm.sectionList[0].SectionId;
            tablesectionvm.TableList = _tableSectionService.GetTableBySection(tablesectionvm.sectionList[0].SectionId, search, pageNumber, pageSize);
        }
        else{
            ViewBag.selectedSection = SectionID;
            tablesectionvm.TableList = _tableSectionService.GetTableBySection(SectionID, search, pageNumber, pageSize);

        }
       
         ViewData["sidebar-active"] = "TableAndSection";
        return View(tablesectionvm);
    }

    [PermissionAuthorize("TableSection.View")]
     public IActionResult TablePagination(long? SectionID, string search = "", int pageNumber = 1, int pageSize = 5)
    {
         TableSectionViewModel tablesectionvm = new();

        tablesectionvm.sectionList = _tableSectionService.GetAllSections();

        if (SectionID != null)
        {   
            ViewBag.selectedSection = SectionID;
            tablesectionvm.TableList = _tableSectionService.GetTableBySection(SectionID, search, pageNumber, pageSize);
        }
        return PartialView("_TablesListPartial",  tablesectionvm.TableList);
    }

    [PermissionAuthorize("TableSection.EditAdd")]
    public async Task<IActionResult> AddSection(TableSectionViewModel Tablesection){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addSectionstatus =await _tableSectionService.AddSection(Tablesection.Section,userId);
        if(addSectionstatus){
            TempData["SuccessMessage"]="Section Added Successfully";
            return RedirectToAction("TableAndSection");
        }
        else{
            TempData["ErrorMessage"]="Error While Adding Section. Try Again!";
            return RedirectToAction("TableAndSection");
        }
    }

    [PermissionAuthorize("TableSection.EditAdd")]
    public async Task<IActionResult> EditSection(TableSectionViewModel Tablesection){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool editSectionStatus =await _tableSectionService.EditSection(Tablesection.Section,userId);
        if(editSectionStatus){
            TempData["SuccessMessage"]="Section Updated Successfully";
        }
        else{
            TempData["ErrorMessage"]="Error While Updating Section. Try Again!";

        }
        return RedirectToAction("TableAndSection",new{SectionID=Tablesection.Section.SectionId});
    
    }

    [PermissionAuthorize("TableSection.Delete")]
    public async Task<IActionResult> DeleteSection(long id){
        bool deleteSectionStatus = await _tableSectionService.DeleteSection(id);
        if(deleteSectionStatus){
            TempData["SuccessMessage"]="Section Deleted Successfully";
        }
        else{
            TempData["ErrorMessage"]="Error While Deleting Section. Try Again!";
        }
        return RedirectToAction("TableAndSection");
    }

    [PermissionAuthorize("TableSection.EditAdd")]
    public async Task<IActionResult> AddTable(TableSectionViewModel Tablesection){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addTablestatus =await _tableSectionService.AddTable(Tablesection.table,userId);
        if(addTablestatus){
            TempData["SuccessMessage"]="Table Added Successfully";
        }
        else{
            TempData["ErrorMessage"]="Error While Adding Table. Try Again!";

        }
        return RedirectToAction("TableAndSection",new{SectionID=Tablesection.table.SectionId});
    }

    public async Task<IActionResult> EditTable(TableSectionViewModel Tablesection){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addTablestatus =await _tableSectionService.EditTable(Tablesection.table,userId);
        if(addTablestatus){
            TempData["SuccessMessage"]="Table Ipdate Successfully";
        }
        else{
            TempData["ErrorMessage"]="Error While Updating Table. Try Again!";
        }
        return RedirectToAction("TableAndSection",new{SectionID=Tablesection.table.SectionId});
    }

    [PermissionAuthorize("TableSection.Delete")]
    public async Task<IActionResult> Deletetable(long id){
        var table =await _tableSectionService.getTableByTableId(id);
        bool deleteTableStatus = await _tableSectionService.DeleteTable(id);
        if(deleteTableStatus){
            return Json(new { success = true, text = "Tables Deleted Successfully" });
        }
        else{
             return Json(new { success = false, text = "Erro While deleting These Tables. Try Again!" });
        }
       
    }

    
}
