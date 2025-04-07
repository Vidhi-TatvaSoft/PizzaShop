using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderAppMenuController :Controller
{
    public IActionResult OrderAppMenu()
    {   
        ViewData["orderApp-Active"] = "Menu";
        return View();
    }
}
