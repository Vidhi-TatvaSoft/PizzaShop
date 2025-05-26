using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;

public class OrderAppController :Controller
{
    private readonly IUserService _userService;
    private readonly IOrderAppService _orderAppService;

    private readonly IJWTTokenService _jwttokenService;

    public OrderAppController(IUserService userService, IOrderAppService orderAppService, IJWTTokenService jWTTokenService)
    {
        _userService = userService;
        _orderAppService = orderAppService;
        _jwttokenService= jWTTokenService;
    }

    #region MyProfile get
    // [Authorize(Roles = "Admin")]
    public IActionResult ProfilePage()
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
            CountryId = (long)userData[0].CountryId,
            StateId = (long)userData[0].StateId,
            CityId = (long)userData[0].CityId,
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
        ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        return View(userViewModel);
    }
    #endregion

    #region Myprofile post
    // post method
    [HttpPost]
    public async Task<IActionResult> ProfilePage(UserViewModel user)
    {
        ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        if (user.StateId == -1 && user.CityId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("ProfilePage", "OrderApp");
        }
        if (user.StateId == -1)
        {
            TempData["stateErrorMessage"] = "Please select a state";
            return RedirectToAction("ProfilePage", "OrderApp");
        }
        if (user.CityId == -1)
        {
            TempData["cityErrorMessage"] = "Please select a city";
            return RedirectToAction("ProfilePage", "OrderApp");
        }
        var token = Request.Cookies["AuthToken"];
        var userEmail = _jwttokenService.GetClaimValue(token, "email");

        if (user.ProfileImage != null)
        {
            var extension = user.ProfileImage.FileName.Split(".");
            if (extension[extension.Length -1] == "jpg" || extension[extension.Length -1] == "jpeg" || extension[extension.Length -1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                string fileName = BLL.Common.ImageUpload.UploadImage(user.ProfileImage, path);
                user.Image = $"/uploads/{fileName}";
            }else{
                TempData["ErrorMessage"] = "Please Upload an Image in JPEG, PNG or JPG format.";
                return RedirectToAction("ProfilePage", "OrderApp", new { Email = user.Email });
            }
        }
        if ( _userService.IsUserNameExistsForEdit(user.Username, userEmail))
        {
            TempData["ErrorMessage"] = "UserName Already Exists. Try Another Username";
            return RedirectToAction("ProfilePage", "OrderApp", new { Email = userEmail });
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


        return RedirectToAction("OrderAppKOT", "OrderAppKOT");
    }
    #endregion

        #region changepassword get
    public IActionResult ChangePasswordOrderApp()
    {
         ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        return View();
    }
    #endregion

    #region changepassword post
    [HttpPost]
    public IActionResult ChangePasswordOrderApp(ChangePasswordViewModel changePassword)
    {
         ViewData["orderApp-Active"] = "Table";
        ViewData["orderAppDDIcon"] = "fa-table";
        var token = Request.Cookies["AuthToken"];
        var Email = _jwttokenService.GetClaimValue(token, "email");
        if (changePassword.NewPassword == changePassword.ConfirmPassword)
        {
            var changePasswordStatus = _userService.ChangepasswordService(changePassword, Email);
            if (changePasswordStatus)
            {
                TempData["SuccessMessage"] = "Password changed successfully";
                return RedirectToAction("OrderAppTable", "OrderAppTable");
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


}
