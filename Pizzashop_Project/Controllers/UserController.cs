using System.Net;
using System.Net.Mail;
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
    private readonly IWebHostEnvironment _env;


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

    #region MyProfile get
    [Authorize(Roles = "Admin")]
    public IActionResult MyProfile()
    {
        var token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        UserViewModel userViewModel = new UserViewModel()
        {
            UserId = userData[0].UserId,
            UserloginId = userData[0].UserloginId,
            RoleId = userData[0].RoleId,
            FirstName = userData[0].FirstName,
            LastName = userData[0].LastName,
            Username = userData[0].Username,
            Email = userData[0].Userlogin.Email,
            Phone = userData[0].Phone,
            Image = userData[0].ProfileImage,
            CountryId = userData[0].CountryId,
            StateId = userData[0].StateId,
            CityId = userData[0].CityId,
            Address = userData[0].Address,
            Zipcode = userData[0].Zipcode
        };
        var Countries = _userService.GetCountry();
        var States = _userService.GetState(userData[0].CountryId);
        var Cities = _userService.GetCity(userData[0].StateId);
        ViewBag.Countries = new SelectList(Countries, "CountryId", "CountryName");
        ViewBag.States = new SelectList(States, "StateId", "StateName");
        ViewBag.Cities = new SelectList(Cities, "CityId", "CityName");
        // var data = userData[0].Userlogin.Email;

        return View(userViewModel);
    }
    #endregion

    #region Myprofile post
    // post method
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult MyProfile(UserViewModel user)
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

        if (user.ProfileImage != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

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

        _userService.UpdateProfile(user, userEmail);
        CookieOptions options = new CookieOptions();
        options.Expires = DateTime.Now.AddMinutes(60);
        if (user.Image != null)
        {
            Response.Cookies.Append("profileImage", user.Image, options);
        }
        Response.Cookies.Append("username", user.Username, options);
        TempData["SuccessMessage"] = "Profile updated successfully";
        return RedirectToAction("MyProfile", "User");
    }
    #endregion


    #region Adduser get
    [Authorize(Roles = "Admin")]
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
    #endregion


    #region  addUser post
    [HttpPost]
    [Authorize(Roles = "Admin")]
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

        // _userService.AddUser(user, Email);
        if (user.ProfileImage != null)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

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

        try
        {
            if (!await _userService.AddUser(user))
            {
                ViewBag.Message = "Email already exists";
                return View();
            }
            var senderEmail = new MailAddress("tatva.pca155@outlook.com", "tatva.pca155@outlook.com");
            var receiverEmail = new MailAddress(user.Email, user.Email);
            var password = "P}N^{z-]7Ilp";
            var sub = "Add user";
            var resetLink = Url.Action("ResetPassword", "UserLogin", new { Email = user.Email }, Request.Scheme);
            var body = $@"     <div style='max-width: 500px; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
                <div style='background: #006CAC; padding: 10px; text-align: center; height:90px; max-width:100%; display: flex; justify-content: center; align-items: center;'>
                    <img src='https://images.vexels.com/media/users/3/128437/isolated/preview/2dd809b7c15968cb7cc577b2cb49c84f-pizza-food-restaurant-logo.png' style='max-width: 50px;' />
                    <span style='color: #fff; font-size: 24px; margin-left: 10px; font-weight: 600;'>PIZZASHOP</span>
                </div>
                <div style='padding: 20px 5px; background-color: #e8e8e8;'>
                    <p>Welcome to Pizza shop,</p>
                    <p>Please Find the details below to login to your account:</p><br>
                    <h3>Login details</h3>
                    <p>Email: {user.Email}</p>
                    <p>Password: {user.Password}</p><br>
                    <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                    
                </div>
                </div>";
            var smtp = new SmtpClient
            {
                Host = "mail.etatvasoft.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body,
                IsBodyHtml = true

            })
            {
                await smtp.SendMailAsync(mess);
            }
            TempData["SuccessMessage"] = "User added successfully. Check your email for login details";


        }
        catch (Exception ex)
        {
            ViewBag.Message = ex.Message;
        }


        return RedirectToAction("UsersList", "User");
        // return View();
    }
    #endregion


    #region  EditUser get
    // GET
    [Authorize(Roles = "Admin")]
    public IActionResult EditUser(string Email)
    {
        // var token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmailWithoutToken(Email);
        UserViewModel userViewModel = new UserViewModel()
        {
            UserId = userData[0].UserId,
            UserloginId = userData[0].UserloginId,
            RoleId = userData[0].RoleId,
            FirstName = userData[0].FirstName,
            LastName = userData[0].LastName,
            Username = userData[0].Username,
            Email = userData[0].Userlogin.Email,
            Phone = userData[0].Phone,
            Image = userData[0].ProfileImage,
            CountryId = userData[0].CountryId,
            StateId = userData[0].StateId,
            CityId = userData[0].CityId,
            Address = userData[0].Address,
            Zipcode = userData[0].Zipcode
        };
        var Roles = _userService.GetRole();
        var Countries = _userService.GetCountry();
        var States = _userService.GetState(userData[0].CountryId);
        var Cities = _userService.GetCity(userData[0].StateId);
        ViewBag.Roles = new SelectList(Roles, "RoleId", "RoleName");
        ViewBag.Countries = new SelectList(Countries, "CountryId", "CountryName");
        ViewBag.States = new SelectList(States, "StateId", "StateName");
        ViewBag.Cities = new SelectList(Cities, "CityId", "CityName");
        // var data = userData[0].Userlogin.Email;

        return View(userViewModel);
    }
    #endregion


    #region EditUser post
    [HttpPost]
    [Authorize(Roles = "Admin")]
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
        // var token = Request.Cookies["AuthToken"];
        var Email = user.Email;
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

        await _userService.EditUser(user, Email);
        TempData["SuccessMessage"] = "User updated successfully";
        return RedirectToAction("UsersList", "User");
        // return View();
    }
    #endregion


    #region cangepassword get
    public IActionResult ChangePassword()
    {
        return View();
    }
    #endregion

    #region changepassword post
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
                TempData["SuccessMessage"] = "Password changed successfully";
                return RedirectToAction("UsersList", "User");
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
    #endregion


    #region deleteUser 
    public async Task<IActionResult> deleteUser(string Email)
    {
        var deleteStatus = await _userService.deleteUser(Email);
        if (!deleteStatus)
        {
            ViewBag.Message = "User not deleted";
            return RedirectToAction("UsersList", "User");

        }
        TempData["SuccessMessage"] = "User deleted successfully";
        return RedirectToAction("UsersList", "User");
    }
    #endregion

    #region Logout
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        Response.Cookies.Delete("email");
        Response.Cookies.Delete("profileImage");
        Response.Cookies.Delete("username");
        return RedirectToAction("VerifyPassword", "UserLogin");
    }
    #endregion

    #region Userlist
    // [Authorize(Roles = "Admin")]
    public IActionResult UsersList()
    {
        var users = _userService.GetUserList();
        return View(users);
    }
    #endregion


    #region PaginatedData
    public IActionResult PaginatedData(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5)
    {
        ViewBag.email = Request.Cookies["email"];
        var users = _userService.GetUserList(search, sortColumn, sortDirection, pageNumber, pageSize);
        return PartialView("_UserListPartial", users);
    }
    #endregion
    // [HttpGet]
    // public IActionResult GetCountries()
    // {
    //     var countries = _userService.GetCountry();
    //     return Json(new SelectList(countries, "Id", "Name"));
    // }

    #region Getstates
    [HttpGet]
    public IActionResult GetStates(long? countryId)
    {

        var states = _userService.GetState(countryId);
        return Json(new SelectList(states, "StateId", "StateName"));
    }
    #endregion


    #region Getcities
    [HttpGet]
    public IActionResult GetCities(long? stateId)
    {
        var cities = _userService.GetCity(stateId);
        return Json(new SelectList(cities, "CityId", "CityName"));
    }
    #endregion

}
