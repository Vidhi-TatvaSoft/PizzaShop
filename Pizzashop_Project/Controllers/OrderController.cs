using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderController :Controller
{
    public readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public IActionResult Orders()
    {
        var orders = _orderService.GetAllOrders();
        ViewData["sidebar-active"] = "Orders";
        return View(orders);
    }

    public IActionResult PaginatedOrdersData(string search, string sortColumn, string sortDirection, string Status,string timePeriod,string startDate,string endDate, int pageNumber = 1, int pageSize = 5)
    {
        var orders = _orderService.GetAllOrders(search, sortColumn, sortDirection, pageNumber, pageSize,Status,timePeriod,startDate,endDate);
        return PartialView("_OrderListPartial", orders);
    }
}
