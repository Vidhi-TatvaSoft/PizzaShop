using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderAppTableController:Controller
{
    private readonly ITableAndSection _tableSectionService;
    private readonly IOrderAppTableService _orderAppTableService;

    private readonly IUserLoginService _userLoginSerivce;

    private readonly IUserService _userService;

    public  OrderAppTableController(ITableAndSection tableAndSection, IOrderAppTableService orderAppTableService, IUserLoginService userLoginService, IUserService userService)
    {
        _tableSectionService = tableAndSection;
        _orderAppTableService = orderAppTableService;
        _userLoginSerivce = userLoginService;
        _userService = userService;
    }
    public IActionResult OrderAppTable()
    {
        OrderAppTableViewModel orderappTablevm = new();
        ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        // orderappTablevm.sectionList = ;
        orderappTablevm.sectionList = _orderAppTableService.GetSectionList();
        return View(orderappTablevm);
    }

    public IActionResult GetTableDetailsBySection(long SectionId){
        OrderAppTableViewModel orderAppTablevm = new();
        orderAppTablevm.tablesInSection = _orderAppTableService.GetTableDetailsBySection(SectionId);
        return PartialView("_TableListInSectionPartial",orderAppTablevm.tablesInSection);

    }

    public async Task<IActionResult> WaitingTokenDetails(OrderAppTableViewModel orderappTablevm){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);

        long customerIdIfPresent = _orderAppTableService.IsCustomerPresent(orderappTablevm.waitingTokenDetailsViewModel.Email);
        if(customerIdIfPresent == 0){
            bool createCustomer =await _orderAppTableService.AddCustomer(orderappTablevm.waitingTokenDetailsViewModel, userId);
            if(!createCustomer){
                return Json(new {success= false, text="Error While Adding Customer. Try Again!"});
            }
        }
        bool customerAddToWaitingList =await _orderAppTableService.AddCustomerToWaitingList(orderappTablevm.waitingTokenDetailsViewModel, userId);
        if(customerAddToWaitingList){
            return Json(new {success= true, text="Customer Added In Waiting List"});
        }
        return Json(new {success= false, text="Error While Adding Customer to waiting List. Try Again!"});
    }
}
