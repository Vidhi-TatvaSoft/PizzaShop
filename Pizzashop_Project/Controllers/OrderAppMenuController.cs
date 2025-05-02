using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.ViewModels;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
[PermissionAuthorize("AccountManagerRole")]
public class OrderAppMenuController :Controller
{
    private readonly IMenuService _menuService;

    private readonly IOrderAppMenuService _orderAppMenuService ;


    public OrderAppMenuController(IMenuService menuService, IOrderAppMenuService orderAppMenuService)
    {
        _menuService = menuService;
        _orderAppMenuService = orderAppMenuService;
    }

    public IActionResult OrderAppMenu(long customerId = 0)
    {  
        OrderAppMenuViewModel menuvm= new();
        menuvm.categoryList =  _menuService.GetAllCategories(); 
        ViewData["orderApp-Active"] = "Menu";
        ViewData["orderAppDDIcon"] = "fa-burger";
        
        ViewData["customerId"]=customerId;
        if(customerId != 0){
        //    menuvm.orderdetails= GetOrderDetailsBycustId(customerId);
        }
        
        return View(menuvm);
    }


    #region ItemsByCategorySelectedPartial
    public IActionResult GetItemByCategory(long categoryId,string searchText =""){
        OrderAppMenuViewModel menuvm = new();
        menuvm.itemsListByCategory = _orderAppMenuService.GetItemByCategory(categoryId,searchText);
        return PartialView("_ItemsByCategorySelectedPartial",menuvm.itemsListByCategory);
    }
    #endregion

    // #region SearchItem
    // public IActionResult SearchItem(string searchText,long categoryID){
    //     OrderAppMenuViewModel menuvm = new();
    //     menuvm.itemsListByCategory = _orderAppMenuService.GetItemByCategory(categoryID,searchText);
    //     return PartialView("_ItemsByCategorySelectedPartial",menuvm.itemsListByCategory);
    // }
    // #endregion

    #region FavouriteItemManage
    public async Task<IActionResult> FavouriteItemManage(long itemId, bool IsFavourite){
        bool status =await _orderAppMenuService.FavouriteItemManage(itemId,IsFavourite);
        if(status ){
            if(IsFavourite){
            return Json( new{ success=true, text="Item Added to Favourite Items"});
            }else{
                return Json(new {success=true , text="Item Removed from Favourite Items"});
            }
        }else{
            return Json( new{ success=false, text="Something Went Wrong! Try Again!"});
        }
    }
    #endregion

    #region GetModifiersByItemId
    public IActionResult GetModifiersByItemId(long itemId){
        OrderAppMenuViewModel menuvm = new();
        menuvm.modifirsByItemList = _orderAppMenuService.GetModifiersByItemId(itemId);
        return PartialView("_ModifiersByItemModalPartial",menuvm);

    }
    #endregion

    #region GetOrderDetailsBycustId
    public IActionResult GetOrderDetailsBycustId(long customerId){
        OrderDetaIlsInvoiceViewModel orderDetailvm =  new();
        orderDetailvm = _orderAppMenuService.GetOrderDetailsByCustomerId(customerId);
        return PartialView("_MenuItemsWithOrderDetails",orderDetailvm);
    }
    #endregion

    #region UpdateOrderDetailPartialView
    public async Task<IActionResult> UpdateOrderDetailPartialView(string ItemList, string orderDetails){
        List<List<int>>? itemList = JsonConvert.DeserializeObject<List<List<int>>>(ItemList);
        OrderDetaIlsInvoiceViewModel? orderDetailvm = JsonConvert.DeserializeObject<OrderDetaIlsInvoiceViewModel>(orderDetails);
        OrderDetaIlsInvoiceViewModel orderDetailsvm =await _orderAppMenuService.UpdateOrderDetailPartialView(itemList,orderDetailvm );

        return PartialView("_MenuItemsWithOrderDetails",orderDetailsvm);
    }
    #endregion

    #region RemoveItemfromOrderDetailPartialView
    public async Task<IActionResult> RemoveItemfromOrderDetailPartialView(string ItemList, int count, string orderDetails){
        List<List<int>>? itemList = JsonConvert.DeserializeObject<List<List<int>>>(ItemList);
        OrderDetaIlsInvoiceViewModel? orderDetailvm = JsonConvert.DeserializeObject<OrderDetaIlsInvoiceViewModel>(orderDetails);
        OrderDetaIlsInvoiceViewModel orderDetailsvm =await _orderAppMenuService.RemoveItemfromOrderDetailPartialView(itemList, count ,orderDetailvm);

        return PartialView("_MenuItemsWithOrderDetails",orderDetailsvm);
    }
    #endregion

    #region SaveOrderDetails
    public async Task<IActionResult> SaveOrderDetails(string orderDetailIds, string orderDetails){
        List<int>? orderDetailId = JsonConvert.DeserializeObject<List<int>>(orderDetailIds);
        OrderDetaIlsInvoiceViewModel? orderDetailvm = JsonConvert.DeserializeObject<OrderDetaIlsInvoiceViewModel>(orderDetails);
        OrderDetaIlsInvoiceViewModel orderDetailsvm =await _orderAppMenuService.SaveOrderDetails(orderDetailId,orderDetailvm);

        return PartialView("_MenuItemsWithOrderDetails",orderDetailsvm);
    }
    #endregion

    #region SaveRatings
    public async Task<IActionResult> SaveRatings(long customerId, int foodreview, int serviceReview,int ambienceReview, string reviewtext ){
        long ratingId =await _orderAppMenuService.SaveRatings(customerId,foodreview,serviceReview,ambienceReview,reviewtext );
        return Json(ratingId);
    }
    #endregion

    #region CompleteOrder
    public async Task<IActionResult> CompleteOrder(string orderDetails, long paymentmethodId){
        OrderDetaIlsInvoiceViewModel? orderDetailvm = JsonConvert.DeserializeObject<OrderDetaIlsInvoiceViewModel>(orderDetails);
        OrderDetaIlsInvoiceViewModel orderDetailsvm =await _orderAppMenuService.CompleteOrder(orderDetailvm,paymentmethodId);
        return PartialView("_MenuItemsWithOrderDetails",orderDetailsvm);
    }
    #endregion

    #region CompleteOrderValidation
    public async Task<IActionResult> CompleteOrderValidation(string orderDetailIds,string orderDetails){
        List<int>? orderDetailId = JsonConvert.DeserializeObject<List<int>>(orderDetailIds);
        OrderDetaIlsInvoiceViewModel? orderDetailvm = JsonConvert.DeserializeObject<OrderDetaIlsInvoiceViewModel>(orderDetails);
        bool IsAllItemReady=await _orderAppMenuService.IsAllItemReady(orderDetailId,orderDetailvm);
        if(IsAllItemReady){
            return Json(new {success=true});
        }else{
            return Json( new{success=false});
        }
    }
    #endregion
}
