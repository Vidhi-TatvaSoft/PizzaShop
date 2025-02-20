using BLL.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pizzashop_Project.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }


    // public IActionResult Index()
    // {

    //     // var userData = await _userService.getuser();
    //     return View();
    // }


    public IActionResult MyProfile()
    {
       var token  =  Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        // var data = userData[0].Userlogin.Email;

        return View(userData[0]);
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("email");
        return RedirectToAction("VerifyPassword", "UserLogin");
    }

    //get method 
    public IActionResult EditProfile()
    {
        var token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        return View(userData[0]);
    }

    //post method
    // [HttpPost]
    // public IActionResult EditProofile(User user)

    
}
