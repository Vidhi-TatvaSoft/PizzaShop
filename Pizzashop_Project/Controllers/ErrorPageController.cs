using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class ErrorPageController :Controller
{
    public IActionResult pageNotFoundError()
    {
        return View();
    }
}
