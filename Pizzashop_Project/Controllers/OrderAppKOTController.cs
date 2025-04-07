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
        return View(orderAppKOTViewModel);
    }
}
