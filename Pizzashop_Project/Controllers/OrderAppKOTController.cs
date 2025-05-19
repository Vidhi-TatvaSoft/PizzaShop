using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderAppKOTController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IOrderAppKotService _orderAppKotService;
    private readonly IUserService _userService;

    private readonly IUserLoginService _userLoginSerivce;

    public OrderAppKOTController(IMenuService menuService, IOrderAppKotService orderAppKotService,IUserService userService, IUserLoginService userLoginService)
    {
        _menuService = menuService;
        _orderAppKotService = orderAppKotService;
        _userService = userService;
        _userLoginSerivce = userLoginService;
    }
    public IActionResult OrderAppKOT()
    {
        OrderAppKOTViewModel orderAppKOTViewModel = new();
        orderAppKOTViewModel.categoryList = _menuService.GetAllCategories();
        ViewData["orderApp-Active"] = "KOT";
        ViewData["orderAppDDIcon"] = "fa-clipboard";
        return View(orderAppKOTViewModel);
    }

    // public async Task<IActionResult> GetDetailsByCategory(long categoryId, string status){
    //     List<KotCardDetailsViewModel> kotCardDetailsvm = await _orderAppKotService.GetDetailsByCategory(categoryId, status);
    //     return PartialView("_displayCardsPartial", kotCardDetailsvm);
    // }

    public async Task<IActionResult> GetDetailsByCategorypagination(long categoryId, string status, int pageNumber = 1, int pageSize = 5){

        PaginationViewModel<KotCardDetailsViewModel> kotCardDetailsvm = await _orderAppKotService.GetDetailsByCategorypaginationSP(categoryId, status, pageNumber, pageSize);
        return PartialView("_displayCardsPartial", kotCardDetailsvm);
    }

    public async Task<IActionResult> GetDetailsOfCardForSelectedOrder(long orderid,long catid,string status,int pageNumber = 1, int pageSize = 5 ){
        KotCardDetailsViewModel kotPerticularOrderDetailsvm =await _orderAppKotService.GetDetailsOfCardForSelectedOrder(orderid,catid,status, pageNumber,pageSize);
        return PartialView("_DisplayQuantityDetails",kotPerticularOrderDetailsvm);
    }

    // public async Task<IActionResult> GetDetailsOfCardForOrder(long orderid,long catid,string status ){
    //     KotCardDetailsViewModel kotPerticularOrderDetailsvm =await _orderAppKotService.GetDetailsOfCardForSelectedOrder(orderid,catid,status);
    //     return Json(kotPerticularOrderDetailsvm);
    // }

    public async Task<IActionResult> ChangeItemQuantitiesAndStatus(int[] orderdetailIdarr , int [] itemquantityarr, string status){
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        var quantityChangeStatus =await _orderAppKotService.ChangeItemQuantitiesAndStatus(orderdetailIdarr, itemquantityarr, status,userId);
        if(quantityChangeStatus){
            return Json(new { success = true, text = "Item status Updated successfully"});
        }
        return Json(new { success = false, text = "Somethin went wrong! Try Again."});
    }
}
