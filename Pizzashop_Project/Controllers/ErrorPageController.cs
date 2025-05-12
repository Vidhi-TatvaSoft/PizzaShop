using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

public class ErrorPageController :Controller
{
    [Route("ErrorPage/pageNotFoundError")]
    public IActionResult pageNotFoundError()
    {
        return View();
    }

    [Route("ErrorPage/Unauthorize")]
     public IActionResult Unauthorize()
    {
        return View();
    }

    [Route("ErrorPage/Forbidden")]
     public IActionResult Forbidden()
    {
        return View();
    }

    [Route("ErrorPage/InternalServerError")]
     public IActionResult InternalServerError()
    {
        return View();
    }

    [Route("ErrorPage/HandleError/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        switch (statusCode)
        {
            case 400:
                return View("BadRequest");
            case 401:
                return View("Unauthorize"); //dn
            case 403:
                return View("Forbidden");  //dn
            case 404:
                return View("pageNotFoundError"); //dn
            case 500:
                return View("InternalServerError");
            default:
                return View("GenericError");
        }
    }

    [AllowAnonymous]
    public IActionResult HandleErrorWithToaster(string message)
    {
        TempData["ErrorMessage"] = message;

        string referer = Request.Headers["Referer"].ToString();
        // string referer = Url.Action("GenericError","Error");

        if (string.IsNullOrEmpty(referer))
        {
            referer = Url.Action("VerifyPassword", "UserLogin") ?? "/"; // or any safe fallback page
        }

        return Redirect(referer);
    }
}
