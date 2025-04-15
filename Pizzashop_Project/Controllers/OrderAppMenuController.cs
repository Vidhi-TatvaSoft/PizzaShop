using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AccountManagerRole")]
public class OrderAppMenuController :Controller
{
    public IActionResult OrderAppMenu()
    {   
        ViewData["orderApp-Active"] = "Menu";
        ViewData["orderAppDDIcon"] = "fa-burger";
        return View();
    }
}
