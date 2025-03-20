using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;

public class TaxAndFeesController: Controller
{
    private readonly ITaxAndFees _taxAndFeeService;
    private readonly IUserLoginService _userLoginSerivce;
    private readonly IUserService _userService;

    public TaxAndFeesController(ITaxAndFees taxAndFees,IUserLoginService userLoginService, IUserService userService){
        _taxAndFeeService=taxAndFees;
        _userLoginSerivce = userLoginService;
        _userService=userService;
    }

    public async Task<IActionResult> TaxAndFees(){
        TaxANdFeesViewModel taxFeesvm= new();
        taxFeesvm.TaxList = _taxAndFeeService.GetTaxes();
        ViewData["sidebar-active"] = "TaxAndFees";
        return View(taxFeesvm);

    }


    #region PaginatedData
    [PermissionAuthorize("User.View")]
    //    [Authorize(Roles = "Admin")]
    public IActionResult PaginatedTax(string search = "",  int pageNumber = 1, int pageSize = 5)
    {
        TaxANdFeesViewModel taxFeesvm = new();
        taxFeesvm.TaxList = _taxAndFeeService.GetTaxes(search,  pageNumber, pageSize);
        return PartialView("_TaxListPartial", taxFeesvm.TaxList);
    }
    #endregion

    #region AddTax
    public async Task<IActionResult> AddTax(TaxANdFeesViewModel taxFeesvm){

        var taxNamePresent =await _taxAndFeeService.GetTaxByName(taxFeesvm.taxViewModel);
        if(taxNamePresent!=null){
            TempData["ErroMessage"]="Tax Already present";
            return RedirectToAction("TaxAndFees");
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool taxAddStatus =await  _taxAndFeeService.AddTax(taxFeesvm.taxViewModel,userId);
        if(taxAddStatus){
            TempData["SuccessMessage"]="Tax Added Successfully";
            return RedirectToAction("TaxAndFees");
        }else{
            TempData["ErrorMessage"]="Error whwile Adding Tax. Try Again!";
            return RedirectToAction("TaxAndFees");
        }
    }
    #endregion

    #region GetTaxDetailsById
    public async Task<IActionResult> GetTaxDetailsById(long taxID){
        TaxViewModel taxvm =await _taxAndFeeService.GetTaxDetailsById(taxID);
        return Json(taxvm);
    }
    #endregion
    
    #region EditTax
    public async Task<IActionResult> EditTax(TaxANdFeesViewModel taxfeesvm){
        var taxNamePresent =await _taxAndFeeService.GetTaxByNameForEdit(taxfeesvm.taxViewModel);
        if(taxNamePresent!=null){
            TempData["ErroMessage"]="TaxName Already present";
            return RedirectToAction("TaxAndFees");
        }
        string token = Request.Cookies["AuthToken"];
        var userData = _userService.getUserFromEmail(token);
        long userId = _userLoginSerivce.GetUserId(userData[0].Userlogin.Email);
        bool taxAddStatus =await _taxAndFeeService.EditTax(taxfeesvm.taxViewModel,userId);
        if(taxAddStatus){
            TempData["SuccessMessage"]="Tax Updated Successfully";
            return RedirectToAction("TaxAndFees");
        }else{
            TempData["ErrorMessage"]="Error whwile Updating Tax. Try Again!";
            return RedirectToAction("TaxAndFees");
        }
    }
    #endregion

    #region deleteTax
    public async Task<IActionResult> deleteTax(long id){
        bool deleteTaxStatus = await _taxAndFeeService.DeleteTax(id);
        if(deleteTaxStatus){
            TempData["SuccessMessage"]="Tax Deleted Successfully";
        }
        else{
            TempData["ErrorMessage"]="Error While Deleting Tax. Try Again!";
        }
        return RedirectToAction("TaxAndFees");
    }
    #endregion

}
