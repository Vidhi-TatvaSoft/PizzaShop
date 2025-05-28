using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
[PermissionAuthorize("AccountManagerRole")]

public class OrderAppWaitingListController : Controller
{
    private readonly IOrderAppWaitingService _orderAppWaitingService;

    private readonly IOrderAppTableService _orderAppTableService;

    private readonly ICustomerService _customerService;

    private readonly IUserLoginService _userLoginSerivce;

    private readonly IUserService _userService;

    public OrderAppWaitingListController(IOrderAppWaitingService orderAppWaitingService, IOrderAppTableService orderAppTableService,ICustomerService customerService, IUserLoginService userLoginService, IUserService userService)
    {
        _orderAppWaitingService = orderAppWaitingService;
        _orderAppTableService = orderAppTableService;
        _customerService = customerService;
        _userLoginSerivce = userLoginService;
        _userService = userService;
    }

    #region index
    public IActionResult OrderAppWaitingList()
    {
        ViewData["orderApp-Active"] = "WaitingList";
        ViewData["orderAppDDIcon"] = "fa-clock";
        return View();
    }
    #endregion

    #region getAllSection
    public async Task<IActionResult> GetAllSection()
    {
        OrderAppWaitingListViewModel orderAppWaitingvm = new();
        orderAppWaitingvm.sectionList = await _orderAppWaitingService.GetAllSection();
        return PartialView("_SectionListWLPartial", orderAppWaitingvm.sectionList);
    }
    #endregion

    #region GetWaitingListBySection
    public IActionResult GetWaitingListBySection(long sectionId)
    {
        OrderAppWaitingListViewModel orderAppWaitingvm = new();
        orderAppWaitingvm.waitingList =  _orderAppWaitingService.GetWaitingListBySection(sectionId);
        return PartialView("_waitingListTableBySection", orderAppWaitingvm.waitingList);
    }
    #endregion

    #region get customer which contains the email
    public IActionResult GetCustomerByEmail(string Email){
        List<Customer> customers = _customerService.GetCustomerByEmail(Email);
        return Json(customers);
    }
    #endregion

    #region AddEditWaitingToken
    [HttpPost]
    public async Task<IActionResult> AddEditWaitingToken(OrderAppWaitingListViewModel waitingListvm)
    {
        if (waitingListvm.waitingTokenDetailsViewModel.waitingId == 0)
        {
            bool IscustomerPresentInWaiting = await _orderAppTableService.IsCustomerPresentInWaiting(waitingListvm.waitingTokenDetailsViewModel.Email);
            if (IscustomerPresentInWaiting)
            {
                return Json(new { success = false, text = "This Customer is Already present In waitingList" });
            }
            
        }
        else
        {
            bool IsCustomerPresentInWaitingUpdate = await _orderAppTableService.IsCustomerPresentInWaitingUpdate(waitingListvm.waitingTokenDetailsViewModel.Email, waitingListvm.waitingTokenDetailsViewModel.waitingId);
            if (IsCustomerPresentInWaitingUpdate)
            {
                return Json(new { success = false, text = "This Customer is Already present In waitingList" });
            }
        }
        bool isCustomerAlreadyAssigned = _orderAppTableService.IsCustomerAlreadyAssigned(waitingListvm.waitingTokenDetailsViewModel.Email);
        if (isCustomerAlreadyAssigned)
        {
            return Json(new { success = false, text = "Table already Assigned to this customer" });
        }

        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        // long customerIdIfPresent = _orderAppTableService.IsCustomerPresent(waitingListvm.waitingTokenDetailsViewModel.Email);
        
        bool createCustomer = await _orderAppTableService.AddEditCustomer(waitingListvm.waitingTokenDetailsViewModel, userId);
        if (!createCustomer)
        {
            return Json(new { success = false, text = "Something went wrong. Try Again!" });
        }
        
        bool customerAddEditToWaitingList = await _orderAppTableService.AddEditCustomerToWaitingList(waitingListvm.waitingTokenDetailsViewModel, userId);
        if (customerAddEditToWaitingList)
        {
            if (waitingListvm.waitingTokenDetailsViewModel.waitingId == 0)
            {
                return Json(new { success = true, text = "Customer Added In Waiting List" });
            }
            else
            {
                return Json(new { success = true, text = "Customer Updated In Waiting List" });
            }
        }
        return Json(new { success = false, text = "Something went wrong. Try Again!" });
    }
    #endregion

    #region GetDetailsByWaitingId
    public IActionResult GetDetailsByWaitingId(long waitingId)
    {
        OrderAppWaitingListViewModel waitinglistvm = new();
        waitinglistvm.waitingTokenDetailsViewModel = _orderAppWaitingService.GetWaitingTokenDetailsById(waitingId);
        return Json(waitinglistvm.waitingTokenDetailsViewModel);
    }
    #endregion

    #region DeleteWaitingToken
    [HttpPost]
    public async Task<IActionResult> DeleteWaitingToken(long waitingId){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool waitingTokenDeleteStatus =await _orderAppWaitingService.DeleteWaitingToken(waitingId,userId);
        if(waitingTokenDeleteStatus){
            return Json(new { success = true, text = "Waiting Token Deleted Successfully" });
        }
        return Json(new {success=false , text = "error While Deleting Waiting Token. Try Again!"});
    }
    #endregion


    #region GetTableBySection
    public IActionResult GetTableBySection(long sectionID){
        List<TableViewModel> tableListBySection = _orderAppWaitingService.GetTableBySection(sectionID);
        return Json(tableListBySection);
    }
    #endregion
    

    #region AssignTable
    [HttpPost]
    public async Task<IActionResult> AssignTable(int[] tablesArr, long waitingId, long sectionId){
        if(waitingId == 0 || sectionId ==0 || tablesArr==null){
            return Json(new { success=false, text="Something Went Wrong.Try Again!"});
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var custId = _orderAppWaitingService.GetCustmerIdByWaitingId(waitingId);
        var customerId = _userLoginSerivce.Base64Encode(custId.ToString());
        bool assignedStatus =await _orderAppWaitingService.AssignTable(tablesArr,waitingId,sectionId,userId);

        if(assignedStatus && custId!=0){
            
            return Json(new { success = true, text= "TableS assigned" ,customerId});
        }
        return Json(new { success=false, text="Something Went Wrong.Try Again!"});
    }
    #endregion
}
