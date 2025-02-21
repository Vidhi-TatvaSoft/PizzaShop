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

namespace Pizzashop_Project.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly UserLoginService _userLoginService;

        public UserLoginController(UserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
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
                return RedirectToAction("UsersList", "User");
            }
            return View();
        }

        // POST: UserLogin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                if (userlogin.RememberMe)
                {
                   
                    Response.Cookies.Append("email", userlogin.Email, options);
                    return RedirectToAction("UsersList", "User");
                }
                else{
                     return RedirectToAction("UsersList", "User");
                }
            }
            ViewBag.message = "Enter valid Credentials";
            return View();
        }


        public string GetEmail(string Email){
            ForgotPasswordViewModel forgotPasswordViewModel = new ForgotPasswordViewModel(); 
            forgotPasswordViewModel.Email = Email;
            TempData["Email"] = Email;  
            return Email;
        }
        public IActionResult ForgotPassword()
        {
            
            return View();
        }


        // public ActionResult SendEmail()
        // {
        //     return View();
        // }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgorpassword)
        {
            
                if (ModelState.IsValid)
                {
                    // if(forgorpassword.Email == null){
                    //     ViewBag.message = "Enter Email to reset password";
                    //     return View("ForgotPassword");
                    // }
                    var emailExistStatus = _userLoginService.CheckEmailExist(forgorpassword.Email);
                    if(!emailExistStatus){
                        ViewBag.message = "Email does not exist Enter Existing email toset password";
                        return View("ForgotPassword");
                    }
                    var senderEmail = new MailAddress("test.dotnet@etatvasoft.com", "test.dotnet@etatvasoft.com");
                    var receiverEmail = new MailAddress(forgorpassword.Email, forgorpassword.Email);
                    var password = "P}N^{z-]7Ilp";
                    var sub = "reset Password sub";
                    var resetLink = Url.Action("ResetPassword", "UserLogin", new { Email = forgorpassword.Email }, Request.Scheme);
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

                    return View("VerifyPassword");
                }
            
            
            return View();
        }

        // GET 
        public IActionResult ResetPassword(string Email)
        {
            ResetPasswordViewModel resetpassdata = new ResetPasswordViewModel();
            resetpassdata.Email = Email;
            return View(resetpassdata);
        }

        // POST
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetpassdata){
            if (ModelState.IsValid)
            {   
                
                var emailExistStatus = _userLoginService.CheckEmailExist(resetpassdata.Email);
                if(!emailExistStatus){
                    ViewBag.message = "Email does not exist Enter Existing email toset password";
                    return View("ResetPassword");
                }

                if(resetpassdata.Password != resetpassdata.ConfirmPassword){
                    ViewBag.message = "Password and Confirm Password should be same";
                    return View("ResetPassword");
                }
                var resetStatus = _userLoginService.ResetPassword(resetpassdata);
                if (resetStatus)
                {
                    ViewBag.message = "Password upated Successfully";
                    return RedirectToAction("VerifyPassword");
                }
            }
            // ViewBag.message = "Password not updated";
            return View("ResetPassword");
        }
                       

    }
}
