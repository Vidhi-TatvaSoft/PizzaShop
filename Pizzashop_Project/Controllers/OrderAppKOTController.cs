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

    public OrderAppKOTController(IMenuService menuService, IOrderAppKotService orderAppKotService)
    {
        _menuService = menuService;
        _orderAppKotService = orderAppKotService;
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
        PaginationViewModel<KotCardDetailsViewModel> kotCardDetailsvm = await _orderAppKotService.GetDetailsByCategorypagination(categoryId, status, pageNumber, pageSize);
        return PartialView("_displayCardsPartial", kotCardDetailsvm);
    }

    public async Task<IActionResult> GetDetailsOfCardForSelectedOrder(long orderid,long catid,string status ){
        KotCardDetailsViewModel kotPerticularOrderDetailsvm =await _orderAppKotService.GetDetailsOfCardForSelectedOrder(orderid,catid,status);
        return PartialView("_DisplayQuantityDetails",kotPerticularOrderDetailsvm);
    }

    // public async Task<IActionResult> GetDetailsOfCardForOrder(long orderid,long catid,string status ){
    //     KotCardDetailsViewModel kotPerticularOrderDetailsvm =await _orderAppKotService.GetDetailsOfCardForSelectedOrder(orderid,catid,status);
    //     return Json(kotPerticularOrderDetailsvm);
    // }

    public async Task<IActionResult> ChangeItemQuantitiesAndStatus(int[] orderdetailIdarr , int [] itemquantityarr, string status){
        var quantityChangeStatus =await _orderAppKotService.ChangeItemQuantitiesAndStatus(orderdetailIdarr, itemquantityarr, status);
        if(quantityChangeStatus){
            return Json(new { success = true, text = "Item status Updated successfully"});
        }
        return Json(new { success = false, text = "Somethin went wrong! Try Again."});
    }
}
