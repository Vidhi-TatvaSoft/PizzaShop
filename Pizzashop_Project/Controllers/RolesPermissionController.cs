using BLL.Service.Interfaces;
using BLL.Service;
using Microsoft.AspNetCore.Mvc;
using DAL.ViewModels;
using DAL.Models;

namespace Pizzashop_Project.Controllers;

public class RolesPermissionController : Controller
{
    private readonly RolesPermissionService _rolesPermission;

    public RolesPermissionController(RolesPermissionService rolesPermission)
    {
        _rolesPermission = rolesPermission;
    }

    public IActionResult Roles()
    {
        var roles = _rolesPermission.GetRoles();
        ViewData["sidebar-active"] = "Roles";
        return View(roles);
    }


    public IActionResult Permissions(string name)
    {
        List<RolesPermissionViewModel> permissions = _rolesPermission.permissionByRole(name);
        return View(permissions);
    }

    [HttpPost]
    public IActionResult Permissions(List<RolesPermissionViewModel> rolesPermissionViewModel)
    {
        for (int i = 0; i < rolesPermissionViewModel.Count; i++)
        {
            RolesPermissionViewModel rolesPermissionvm = new RolesPermissionViewModel();
            rolesPermissionvm.PermissionmanageId = rolesPermissionViewModel[i].PermissionmanageId;
            rolesPermissionvm.Canview = rolesPermissionViewModel[i].Canview;
            rolesPermissionvm.Caneditadd = rolesPermissionViewModel[i].Caneditadd;
            rolesPermissionvm.Candelete = rolesPermissionViewModel[i].Candelete;
            rolesPermissionvm.Permissioncheck = rolesPermissionViewModel[i].Permissioncheck;
            _rolesPermission.EditPermissionManage(rolesPermissionvm);
        }
        TempData["SuccessMessage"]="Permissions Updated successfully.";
        return RedirectToAction("Permissions", "RolesPermission", new { name = rolesPermissionViewModel[0].rolename });// 3rdpara ma obj create krvopade bcoz
        //  redirectToAction ma 3rd para obj accept kre string nai..nd get method ma name pass krva mate ahiyathi name no ob banavvi moklvu
    }


}
