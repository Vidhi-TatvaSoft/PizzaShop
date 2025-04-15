using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AccountManagerRole")]

public class OrderAppWaitingListController :Controller
{
    public IActionResult OrderAppWaitingList()
    {
        ViewData["orderApp-Active"] = "WaitingList";
        ViewData["orderAppDDIcon"] = "fa-clock";

        return View();
    }
}
