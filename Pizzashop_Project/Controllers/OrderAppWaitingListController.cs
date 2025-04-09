using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderAppWaitingListController :Controller
{
    public IActionResult OrderAppWaitingList()
    {
        ViewData["orderApp-Active"] = "WaitingList";
        ViewData["orderAppDDIcon"] = "fa-clock";

        return View();
    }
}
