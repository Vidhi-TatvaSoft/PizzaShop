using System.Net.Sockets;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.Service;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AdminManager")]

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

    #region table and section index 
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
    #endregion

    #region section List to et all sectio
    [PermissionAuthorize("TableSection.View")]
    public IActionResult SectionList(){
        TableSectionViewModel tablesectionvm = new();
        tablesectionvm.sectionList = _tableSectionService.GetAllSections();
        return PartialView("_SectionListPartial",tablesectionvm);
    }
    #endregion

    #region table pagination
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
    #endregion
    

    #region Add section
    [PermissionAuthorize("TableSection.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> AddSection(TableSectionViewModel Tablesection){
        var sectionNamePresent =await _tableSectionService.GetSectionByName(Tablesection.Section);
        if(sectionNamePresent!=null){
            return Json(new { success = false, text = "Section Already Present" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addSectionstatus =await _tableSectionService.AddSection(Tablesection.Section,userId);
        var sectionList =_tableSectionService.GetAllSections();
        if(addSectionstatus){
            return Json(new {sectionID= sectionList[0].SectionId,  success = true, text = "Section Added successfully" });
        }else{          
            return Json(new {sectionID= sectionList[0].SectionId, success = false, text = "Error While Adding Section. Try Again!" });
        }
    }
    #endregion

    #region Edit section
    [PermissionAuthorize("TableSection.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> EditSection(TableSectionViewModel Tablesection){
        var sectionNamePresent =await _tableSectionService.GetSectionByNameForEdit(Tablesection.Section);
        if(sectionNamePresent!=null){
            return Json(new { success = false, text = "Section Already present. Try New SectionName" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool editSectionStatus =await _tableSectionService.EditSection(Tablesection.Section,userId);
        var sectionList =_tableSectionService.GetAllSections();
        if(editSectionStatus){
            return Json(new {sectionID= sectionList[0].SectionId, success = true, text = "Section Updated successfully" });
        }else{          
            return Json(new {sectionID= sectionList[0].SectionId, success = false, text = "Error While Updating Section. Try Again!" });
        }
        // return RedirectToAction("TableAndSection",new{SectionID=Tablesection.Section.SectionId});
    
    }
    #endregion

    #region Delete setion
    [PermissionAuthorize("TableSection.Delete")]
    public async Task<IActionResult> DeleteSection(long id){
        var sectionList =_tableSectionService.GetAllSections();
        bool checkcIfAnyTableOccupied = _tableSectionService.ckeckOccupiedTable(id);
        if(checkcIfAnyTableOccupied){
            return Json(new {sectionID= sectionList[0].SectionId, success = false, text = "Section Can not be deleted because of Occupied Table" });
        }
        bool deleteSectionStatus = await _tableSectionService.DeleteSection(id);
        if(deleteSectionStatus){
            return Json(new {sectionID= sectionList[0].SectionId,success = true, text = "Section Deleted successfully" });
        }else{          
            return Json(new {sectionID= sectionList[0].SectionId, success = false, text = "Error While Deleting Section. Try Again!" });
        }
        // return RedirectToAction("TableAndSection");
    }
    #endregion

    #region get all section in json
    public IActionResult GetSectionList(){
        List<Section> sectionList = _tableSectionService.GetAllSections();
        return Json(sectionList);
    }
    #endregion

    #region Addtable
    [PermissionAuthorize("TableSection.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> AddTable(TableSectionViewModel Tablesection){
        var TableNamePresentInSection =await _tableSectionService.GetTableByNameInSameSection(Tablesection.table);
        if(TableNamePresentInSection!=null){
            return Json(new { success = false, text = "Table Already Present" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool addTablestatus =await _tableSectionService.AddTable(Tablesection.table,userId);
        if(addTablestatus){
            return Json(new { success = true, text = "Table Added successfully" });
        }else{          
            return Json(new { success = false, text = "Error While Adding Table. Try Again!" });
        }
    }
    #endregion

    #region Edittable
    [PermissionAuthorize("TableSection.EditAdd")]
    [HttpPost]
    public async Task<IActionResult> EditTable(TableSectionViewModel Tablesection){
        var TableNamePresentInSection =await _tableSectionService.GetTableByNameInSameSection(Tablesection.table);
        if(TableNamePresentInSection!=null){
            return Json(new { success = false, text = "Table Already Present" });
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool EditTablestatus =await _tableSectionService.EditTable(Tablesection.table,userId);
        if(EditTablestatus){
            return Json(new { success = true, text = "Table Updated successfully" });
        }else{          
            return Json(new { success = false, text = "Error While Updating Table. Try Again!" });
        }
    }
    #endregion

    #region deleettable
    [PermissionAuthorize("TableSection.Delete")]
    [HttpPost]
    public async Task<IActionResult> DeleteTable(long id){
        var table =await _tableSectionService.getTableByTableId(id);
        if(table.Status != "Available"){
            return Json(new { success = false, text = "Table is Occupied. Can't Delete this Table" });
        }
        bool deleteTableStatus = await _tableSectionService.DeleteTable(id);
        if(deleteTableStatus){
            return Json(new { success = true, text = "Tables Deleted Successfully" });
        }
        else{
             return Json(new { success = false, text = "Erro While deleting These Tables. Try Again!" });
        }
       
    }
    #endregion

    
}
