using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AccountManagerRole")]

public class OrderAppWaitingListController :Controller
{
    private readonly IOrderAppWaitingService _orderAppWaitingService;

    private readonly IOrderAppTableService _orderAppTableService;

    private readonly IUserLoginService _userLoginSerivce;

    private readonly IUserService _userService;

    public OrderAppWaitingListController(IOrderAppWaitingService orderAppWaitingService, IOrderAppTableService orderAppTableService, IUserLoginService userLoginService, IUserService userService){
        _orderAppWaitingService = orderAppWaitingService;
        _orderAppTableService = orderAppTableService;
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
    public IActionResult GetAllSection(){
        OrderAppWaitingListViewModel orderAppWaitingvm = new();
        orderAppWaitingvm.sectionList = _orderAppWaitingService.GetAllSection();
        return PartialView("_SectionListWLPartial",orderAppWaitingvm.sectionList);
    }
    #endregion

    #region GetWaitingListBySection
    public IActionResult GetWaitingListBySection(long sectionId){
        OrderAppWaitingListViewModel orderAppWaitingvm = new();
        orderAppWaitingvm.waitingList = _orderAppWaitingService.GetWaitingListBySection(sectionId);
        return PartialView("_waitingListTableBySection",orderAppWaitingvm.waitingList);
    }
    #endregion

    #region AddWaitingToken
    [HttpPost]
    public async Task<IActionResult> AddWaitingToken(OrderAppWaitingListViewModel waitingListvm){
        bool IscustomerPresentInWaiting =await _orderAppTableService.IsCustomerPresentInWaiting(waitingListvm.waitingTokenDetailsViewModel.Email);
        if(IscustomerPresentInWaiting){
            return Json( new { success = false, text = "This Customer is Already present In waitingList"});
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        long customerIdIfPresent = _orderAppTableService.IsCustomerPresent(waitingListvm.waitingTokenDetailsViewModel.Email);
        if(customerIdIfPresent == 0){
            bool createCustomer =await _orderAppTableService.AddCustomer(waitingListvm.waitingTokenDetailsViewModel, userId);
            if(!createCustomer){
                return Json(new {success= false, text="Error While Adding Customer. Try Again!"});
            }
        }
        bool customerAddEditToWaitingList =await _orderAppTableService.AddCustomerToWaitingList(waitingListvm.waitingTokenDetailsViewModel, userId);
        if(customerAddEditToWaitingList){
            if(waitingListvm.waitingTokenDetailsViewModel.waitingId == 0){
                return Json(new {success= true, text="Customer Added In Waiting List"});
            }else{
                return Json(new {success= true, text="Customer Updated In Waiting List"});
            }      
        }
        return Json(new {success= false, text="Error While Adding/updating Customer to waiting List. Try Again!"});
    }
    #endregion
}
