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

namespace Pizzashop_Project.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly UserLoginService _userLoginService;

        public UserLoginController(UserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        public async Task<IActionResult> Index()
        {
            
            var userData = await _userLoginService.getusers();
            return View(userData);
        }



        // GET: UserLogin/Create
        public IActionResult VerifyPassword()
        {
            // ViewData["RoleId"] = new SelectList(_userLoginService.Roles, "RoleId", "RoleId");
            if(Request.Cookies["email"] != null){
                return RedirectToAction("Index","UserLogin");
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

            var verifictionStatus = _userLoginService.VerifyUserPassword(userlogin);
            if (verifictionStatus)
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(60);
                Response.Cookies.Append("email",userlogin.Email, options);
                return RedirectToAction("Index", "UserLogin");
            }
            ViewBag.message = "Enter valid Credentials";
            return View();
        }



        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public String GetEmail(String Email)
        {
            TempData["email"] = Email;
            return Email;
        }



        // public ActionResult SendEmail()
        // {
        //     return View();
        // }

        [HttpPost]  
public ActionResult SendEmail(string receiver, string subject, string message) {  
    try {  
        if (ModelState.IsValid) {  
            var senderEmail = new MailAddress("test.dotnet@etatvasoft.com", "Jamil");  
            var receiverEmail = new MailAddress("castulo30@theirer.com", "Receiver");  
            var password = "P}N^{z-]7Ilp";  
            var sub = "reset Password";  
            var body = "reset password";  
            var smtp = new SmtpClient {  
                Host = "smtp.gmail.com",  
                    Port = 587,  
                    EnableSsl = true,  
                    DeliveryMethod = SmtpDeliveryMethod.Network,  
                    UseDefaultCredentials = false,  
                    Credentials = new NetworkCredential(senderEmail.Address, password)  
            };  
            using(var mess = new MailMessage(senderEmail, receiverEmail) {  
                Subject = sub,  
                    Body = body  
            }) {  
                smtp.Send(mess);  
            }  

            return View("VerifyPassword");  
        }  
    } catch (Exception) {  
        ViewBag.Error = "Some Error";  
    }  
    return View();  
}  
    }
}
