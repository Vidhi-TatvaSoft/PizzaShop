using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class OrderAppTableController:Controller
{
    public IActionResult OrderAppTable()
    {
        ViewData["orderApp-Active"] = "Table";
        return View();
    }
}
