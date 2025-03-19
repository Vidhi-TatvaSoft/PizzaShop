using System.Collections.Generic;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class RolesPermissionService : IRolesPermission
{
    private readonly PizzashopDbContext _context;

    public RolesPermissionService(PizzashopDbContext context)
    {
        _context = context;
    }

    public List<Role> GetRoles()
    {
        return _context.Roles.ToList();
    }


    public List<RolesPermissionViewModel> permissionByRole(string name)
    {
        List<Permissionmanage> data = _context.Permissionmanages.Include(x=>x.Role).Include(x=>x.Permission).Where(x => x.Role.RoleName == name).OrderBy(x => x.PermissionId).ToList();
        List<RolesPermissionViewModel> permissions = new();
        for(int i=0;i<data.Count;i++ ){
            RolesPermissionViewModel obj = new();
            obj.PermissionmanageId = data[i].PermissionmanageId;
            obj.rolename = data[i].Role.RoleName;
            obj.Name = data[i].Permission.PermissionsName;
            obj.Canview = data[i].Canview;
            obj.Caneditadd = data[i].Caneditadd;
            obj.Candelete = data[i].Candelete;
            obj.Permissioncheck = data[i].Permissioncheck;
            permissions.Add(obj);
        }
        return permissions;
    }

    public bool EditPermissionManage(RolesPermissionViewModel permissionmanage)
    {
        var data = _context.Permissionmanages.FirstOrDefault(x=>x.PermissionmanageId == permissionmanage.PermissionmanageId);
        if(data == null){
            return false;
        }
        data.Canview = permissionmanage.Canview;
        data.Caneditadd = permissionmanage.Caneditadd;
        data.Candelete = permissionmanage.Candelete;
        data.Permissioncheck = permissionmanage.Permissioncheck;
        _context.Update(data);
        _context.SaveChanges();
        return true;
    }

    public bool EditPermissionManage(Permissionmanage permissionmanage)
    {
        throw new NotImplementedException();
    }

}