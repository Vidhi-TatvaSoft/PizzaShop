using BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class MenuController : Controller
{

    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult ItemsModifiers()
    {
        return View();
    }
}
