using BLL.Interfaces;
using BLL.Service;
using Microsoft.AspNetCore.Mvc;
using DAL.ViewModels;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
// [Authorize(Roles = "Admin")]
public class RolesPermissionController : Controller
{
    private readonly IRolesPermission _rolesPermission;

    public RolesPermissionController(IRolesPermission rolesPermission)
    {
        _rolesPermission = rolesPermission;
    }


    #region get all  roles
    [PermissionAuthorize("Role.View")]
    public IActionResult Roles()
    {
        var roles = _rolesPermission.GetRoles();
        ViewData["sidebar-active"] = "Roles";
        return View(roles);
    }
    #endregion

    #region  get all permissions
    [PermissionAuthorize("Role.View")]
    public IActionResult Permissions(string name)
    {
        List<RolesPermissionViewModel> permissions = _rolesPermission.permissionByRole(name);
        return View(permissions);
    }
    #endregion

    #region permissions post to edit
    [PermissionAuthorize("Role.EditAdd")]
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
    #endregion


}