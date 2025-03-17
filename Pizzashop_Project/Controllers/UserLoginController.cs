using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLL.Services;
using System.Net.Mail;
using System.Net;
using DAL.ViewModels;
using NuGet.Common;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.AspNetCore.Authorization;
using BLL.Interfaces;

namespace Pizzashop_Project.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly IUserLoginService _userLoginService;
        private readonly IJWTTokenService _jwtTokenService;

        public UserLoginController(IUserLoginService userLoginService,IJWTTokenService jwtTokenService)
        {
            _userLoginService = userLoginService;
            _jwtTokenService = jwtTokenService;
        }

        

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var userData = await _userLoginService.getusers();
            return View(userData);
        }



        // GET: UserLogin/Create
        public IActionResult VerifyPassword()
        {
            // ViewData["RoleId"] = new SelectList(_userLoginService.Roles, "RoleId", "RoleId");
            if (Request.Cookies["email"] != null)
            {
                var email = Request.Cookies["email"];
                // CookieOptions options = new CookieOptions();
                // options.Expires = DateTime.Now.AddMinutes(60);
                // Response.Cookies.Append("profileImage", _userLoginService.GetProfileImage(email), options);
                // Response.Cookies.Append("username", _userLoginService.GetUsername(email), options);

               
                ViewData["sidebar-active"] ="Dashboard";
                return RedirectToAction("Dashboard", "User");
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPassword(UserLoginViewModel userlogin)
        {

            var verifictiontoken = _userLoginService.VerifyUserPassword(userlogin);
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddMinutes(60);
            if (verifictiontoken != null)
            {
                Response.Cookies.Append("AuthToken", verifictiontoken, options);
                Response.Cookies.Append("profileImage", _userLoginService.GetProfileImage(userlogin.Email), options);
                Response.Cookies.Append("username", _userLoginService.GetUsername(userlogin.Email), options);
                if (userlogin.RememberMe)
                {

                    Response.Cookies.Append("email", userlogin.Email, options);
                    ViewBag.ProfileImage = _userLoginService.GetProfileImage(userlogin.Email);
                    TempData["SuccessMessage"] = "Login Successfully";
                    ViewData["sidebar-active"] ="Dashboard";
                    return RedirectToAction("Dashboard", "User");
                }
                else
                {
                    TempData["SuccessMessage"] = "Login Successfully";
                    ViewData["sidebar-active"] ="Dashboard";
                    return RedirectToAction("Dashboard", "User");
                }
            }
            ViewBag.message = "Enter valid Credentials";
            return View();
        }


        public string GetEmail(string Email)
        {
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel();
            forgotPasswordViewModel.Email = Email;
            TempData["Email"] = Email;
            return Email;
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgorpassword)
        {

            if (ModelState.IsValid)
            {
                var emailExistStatus = _userLoginService.CheckEmailExist(forgorpassword.Email);
                if (!emailExistStatus)
                {
                    ViewBag.message = "Email does not exist Enter Existing email to set password";
                    return View("ForgotPassword");
                }
                try
                {
                    var email = forgorpassword.Email;
                    var savedPassword = _userLoginService.GetPassword(email);
                    var senderEmail = new MailAddress("tatva.pca155@outlook.com", "tatva.pca155@outlook.com");
                    var receiverEmail = new MailAddress(forgorpassword.Email, forgorpassword.Email);
                    var password = "P}N^{z-]7Ilp";
                    var sub = "reset Password sub";
                    var resetLink = Url.Action("ResetPassword", "UserLogin", new {resetPassToken=_jwtTokenService.GenerateTokenEmailPassword(email,savedPassword)}, Request.Scheme);
                    var body = $@"     <div style='max-width: 500px; font-family: Arial, sans-serif; border: 1px solid #ddd;'>
                <div style='background: #006CAC; padding: 10px; text-align: center; height:90px; max-width:100%; display: flex; justify-content: center; align-items: center;'>
                    <img src='https://images.vexels.com/media/users/3/128437/isolated/preview/2dd809b7c15968cb7cc577b2cb49c84f-pizza-food-restaurant-logo.png' style='max-width: 50px;' />
                    <span style='color: #fff; font-size: 24px; margin-left: 10px; font-weight: 600;'>PIZZASHOP</span>
                </div>
                <div style='padding: 20px 5px; background-color: #e8e8e8;'>
                    <p>Pizza shop,</p>
                    <p>Please click <a href='{resetLink}' style='color: #1a73e8; text-decoration: none; font-weight: bold;'>here</a>
                        to reset your account password.</p>
                    <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                    <p><strong style='color: orange;'>Important Note:</strong> For security reasons, the link will expire in 24 hours.
                        If you did not request a password reset, please ignore this email or contact our support team immediately.
                    </p>
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
                    TempData["SuccessMessage"] = "Reset Password link sent to your Email";
                    return View("VerifyPassword");

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Email not sent due to Email server Error. Try again after some time";
                    return View("ForgotPassword");
                }
            }
            return View();
        }

        // GET 
        public IActionResult ResetPassword(string resetPassToken)
        {

             var email = _jwtTokenService.GetClaimValue(resetPassToken, "email");
            var newpassword = _jwtTokenService.GetClaimValue(resetPassToken, "password");
            var savedPassword = _userLoginService.GetPassword(email);

            if (savedPassword==newpassword)
            {
                ResetPasswordViewModel reserpassdata = new();
                reserpassdata.Email=_jwtTokenService.GetClaimValue(resetPassToken, "email");
                return View(reserpassdata);
            }
            TempData["ErrorMessage"] = "You have already changed the Password once";
            return RedirectToAction("VerifyPassword","UserLogin");
            // resetpassdata.Email =_userLoginService.Base64Decode(Email) ;
            // return View(resetpassdata);
        }

        // POST
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetpassdata)
        {
                var emailExistStatus = _userLoginService.CheckEmailExist(resetpassdata.Email);
                if (!emailExistStatus)
                {
                    ViewBag.message = "Email does not exist Enter Existing email to set password";
                    return View("ResetPassword");
                }

                if (resetpassdata.Password != resetpassdata.ConfirmPassword)
                {
                    ViewBag.message = "Password and Confirm Password should be same";
                    return View("ResetPassword");
                }
                var resetStatus = _userLoginService.ResetPassword(resetpassdata);
                if (resetStatus)
                {
                    TempData["SuccessMessage"] = "Password Reseted successfully";
                    return RedirectToAction("VerifyPassword");
                }
            // }
            TempData["ErrorMessage"] = "Password not updated. Try again";
            return View("ResetPassword");
        }


    }
}