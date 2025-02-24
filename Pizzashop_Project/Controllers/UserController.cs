using System.Threading.Tasks;
using BLL.Service;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pizzashop_Project.Controllers;

public class UserController : Controller
{
    private readonly UserService _userService;
    private readonly JWTTokenService _jwttokenService;

    public UserController(UserService userService, JWTTokenService jwttokenService)
    {
        _userService = userService;
        _jwttokenService = jwttokenService;
    }


    [Authorize(Roles = "Admin")]
    // public IActionResult Index()
    // {

    //     // var userData = await _userService.getuser();
    //     return View();
    // }

    [Authorize(Roles = "Admin")]
    public IActionResult MyProfile()
    {
        var token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        var Countries = _userService.GetCountry();
        var States = _userService.GetState(userData[0].CountryId);
        var Cities = _userService.GetCity(userData[0].StateId);
        ViewBag.Countries = new SelectList(Countries, "CountryId", "CountryName");
        ViewBag.States = new SelectList(States, "StateId", "StateName");
        ViewBag.Cities = new SelectList(Cities, "CityId", "CityName");
        // var data = userData[0].Userlogin.Email;

        return View(userData[0]);
    }

    // post method
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult MyProfile(User user)
    {
        if (user.StateId == -1 && user.CityId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("MyProfile", "User");
        }
        if (user.StateId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            return RedirectToAction("MyProfile", "User");
        }
        if (user.CityId == -1)
        {
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("MyProfile", "User");
        }
        var token = Request.Cookies["AuthToken"];
        var userEmail = _jwttokenService.GetClaimValue(token, "email");
        _userService.UpdateUser(user, userEmail);
        return RedirectToAction("MyProfile", "User");
    }

    public IActionResult AddUser()
    {


        var Roles = _userService.GetRole();
        var Countries = _userService.GetCountry();
        var States = _userService.GetState(-1);
        var Cities = _userService.GetCity(-1);
        ViewBag.Roles = new SelectList(Roles, "RoleId", "RoleName");
        ViewBag.Countries = new SelectList(Countries, "CountryId", "CountryName");
        ViewBag.States = new SelectList(States, "StateId", "StateName");
        ViewBag.Cities = new SelectList(Cities, "CityId", "CityName");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(UserViewModel user)
    {
        
        if (user.StateId == -1 && user.CityId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("AddUser", "User");
        }
        if (user.StateId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            return RedirectToAction("AddUser", "User");
        }
        if (user.CityId == -1)
        {
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("AddUser", "User");
        }
        var token = Request.Cookies["AuthToken"];
        var Email = _jwttokenService.GetClaimValue(token, "email");

        if (! await _userService.AddUser(user, Email))
        {
            ViewBag.Message = "Email already exists";
            return View();
        }
        // _userService.AddUser(user, Email);
        if (user.ProfileImage != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"{Guid.NewGuid()}_{user.ProfileImage.FileName}";
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                user.ProfileImage.CopyTo(stream);
            }
            user.Image = $"/uploads/{fileName}";
        }
        return RedirectToAction("UsersList", "User");
        // return View();
    }

    
    public IActionResult EditUser()
    {
        var token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        var Roles = _userService.GetRole();
        var Countries = _userService.GetCountry();
        var States = _userService.GetState(userData[0].CountryId);
        var Cities = _userService.GetCity(userData[0].StateId);
        ViewBag.Roles = new SelectList(Roles, "RoleId", "RoleName");
        ViewBag.Countries = new SelectList(Countries, "CountryId", "CountryName");
        ViewBag.States = new SelectList(States, "StateId", "StateName");
        ViewBag.Cities = new SelectList(Cities, "CityId", "CityName");
        // var data = userData[0].Userlogin.Email;

        return View(userData[0]);
    }


    [HttpPost]
    public async Task<IActionResult> EditUser(UserViewModel user)
    {
        if (user.StateId == -1 && user.CityId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("AddUser", "User");
        }
        if (user.StateId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            return RedirectToAction("AddUser", "User");
        }
        if (user.CityId == -1)
        {
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("AddUser", "User");
        }
        var token = Request.Cookies["AuthToken"];
        var Email = _jwttokenService.GetClaimValue(token, "email");

        if (! await _userService.AddUser(user, Email))
        {
            ViewBag.Message = "Email already exists";
            return View();
        }
        // _userService.AddUser(user, Email);
        if (user.ProfileImage != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = $"{Guid.NewGuid()}_{user.ProfileImage.FileName}";
            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                user.ProfileImage.CopyTo(stream);
            }
            user.Image = $"/uploads/{fileName}";
        }
        return RedirectToAction("UsersList", "User");
        // return View();
    }




    public IActionResult ChangePassword()
    {
        return View();
    }


    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel changePassword)
    {

        var token = Request.Cookies["AuthToken"];
        var Email = _jwttokenService.GetClaimValue(token, "email");
        if (changePassword.NewPassword == changePassword.ConfirmPassword)
        {
            var changePasswordStatus = _userService.ChangepasswordService(changePassword, Email);
            if (changePasswordStatus)
            {
                return RedirectToAction("MyProfile", "User");
            }
            else
            {
                ViewBag.Message = "Old Password is incorrect";
                return View();
            }
        }
        else
        {
            ViewBag.Message = "New Password and Confirm Password does not match";
            return View();
        }
    }
    public IActionResult deleteUser(int id)
    {
        var user = _userService.deleteUser(id);
        return RedirectToAction("UsersList", "User");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("email");
        return RedirectToAction("VerifyPassword", "UserLogin");
    }

    public IActionResult UsersList()
    {
        var token = Request.Cookies["AuthToken"];
        var Email = _jwttokenService.GetClaimValue(token, "email");
        var users = _userService.getuser(Email);
        return View(users);
    }

    // [HttpGet]
    // public IActionResult GetCountries()
    // {
    //     var countries = _userService.GetCountry();
    //     return Json(new SelectList(countries, "Id", "Name"));
    // }
    [HttpGet]
    public IActionResult GetStates(long? countryId)
    {

        var states = _userService.GetState(countryId);
        return Json(new SelectList(states, "StateId", "StateName"));
    }
    [HttpGet]
    public IActionResult GetCities(long? stateId)
    {
        var cities = _userService.GetCity(stateId);
        return Json(new SelectList(cities, "CityId", "CityName"));
    }

}
