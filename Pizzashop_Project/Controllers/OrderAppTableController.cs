using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AccountManagerRole")]

public class OrderAppTableController:Controller
{
    private readonly ITableAndSection _tableSectionService;
    private readonly IOrderAppTableService _orderAppTableService;

    private readonly IUserLoginService _userLoginSerivce;

    private readonly IUserService _userService;
     
     private readonly ICustomerService _customerService;

    public  OrderAppTableController(ITableAndSection tableAndSection, IOrderAppTableService orderAppTableService, IUserLoginService userLoginService, IUserService userService, ICustomerService customerService)
    {
        _tableSectionService = tableAndSection;
        _orderAppTableService = orderAppTableService;
        _userLoginSerivce = userLoginService;
        _userService = userService;
        _customerService = customerService;
    }

    #region  OrderAppTable
    public IActionResult OrderAppTable()
    {
        ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        // orderappTablevm.sectionList = ;
        return View();
    }
    #endregion

    #region getsection List
    public IActionResult GetsectionList(){
        OrderAppTableViewModel orderappTablevm = new();
         orderappTablevm.sectionList = _orderAppTableService.GetSectionList();
         return PartialView("_SectionListPartial", orderappTablevm);
    }
    #endregion

    #region GetTableDetailsBySection
    public IActionResult GetTableDetailsBySection(long SectionId){
        OrderAppTableViewModel orderAppTablevm = new();
        orderAppTablevm.tablesInSection = _orderAppTableService.GetTableDetailsBySection(SectionId);
        return PartialView("_TableListInSectionPartial",orderAppTablevm.tablesInSection);

    }
    #endregion

    #region WaitingTokenDetails
    [HttpPost]
    public async Task<IActionResult> WaitingTokenDetails(OrderAppTableViewModel orderappTablevm){
        bool IscustomerPresentInWaiting =await _orderAppTableService.IsCustomerPresentInWaiting(orderappTablevm.waitingTokenDetailsViewModel.Email);
        if(IscustomerPresentInWaiting){
            return Json( new { success = false, text = "This Customer is Already present In waitingList"});
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        // long customerIdIfPresent = _orderAppTableService.IsCustomerPresent(orderappTablevm.waitingTokenDetailsViewModel.Email);
        // if(customerIdIfPresent == 0){
            bool createCustomer =await _orderAppTableService.AddEditCustomer(orderappTablevm.waitingTokenDetailsViewModel, userId);
            if(!createCustomer){
                return Json(new {success= false, text="Error While Adding Customer. Try Again!"});
            }
        // }
        bool customerAddToWaitingList =await _orderAppTableService.AddEditCustomerToWaitingList(orderappTablevm.waitingTokenDetailsViewModel, userId);
        if(customerAddToWaitingList){
            return Json(new {success= true, text="Customer Added In Waiting List"});
        }
        return Json(new {success= false, text="Error While Adding Customer to waiting List. Try Again!"});
    }
    #endregion

    #region  GetWaitingListAndCustomerDetails
    public IActionResult GetWaitingListAndCustomerDetails(long sectionId){
        OrderAppTableViewModel orderAppTablevm = new();
        orderAppTablevm.waitinglistdetails = _orderAppTableService.GetListOfCustomerWaiting(sectionId);
        return PartialView("_WaitingListOffcanvasPartial", orderAppTablevm);
    }
    #endregion

    #region AssignTable
    [HttpPost]
    public async Task<IActionResult> AssignTable(string Email, int [] TableIds){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool tableAssignStatus =await _orderAppTableService.Assigntable(Email, TableIds, userId);
        // long custId = await _customerService.GetCustomerIdByTableId(TableIds[0]);
        long customerid= await _customerService.GetCustomerIdByTableId(TableIds[0]);
        string custId = _userLoginSerivce.Base64Encode(customerid.ToString());
        if(tableAssignStatus){
            return Json(new{ success = true, text = "Table Assigned ", custId = custId});
        }
        return Json(new { success=false, text="Something Went wrong, Try Again!"});
    }
    #endregion

    #region  GetCustomerIdByTableId
    public async Task<IActionResult> GetCustomerIdByTableId(long tableId){
        long customerId = await _customerService.GetCustomerIdByTableId(tableId);
        string custId = _userLoginSerivce.Base64Encode(customerId.ToString());
        return Json(new { custId = custId });

    }

    #endregion
}
