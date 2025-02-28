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
        return View(roles);
    }

    public IActionResult Permissions(int id)
    {
        var role = _rolesPermission.permissionByRole(id);
        return View(role);
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
        return RedirectToAction("Permissions");
    }


}
