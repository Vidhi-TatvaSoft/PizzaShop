using System.Collections.Generic;
using BLL.Service.Interfaces;
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

    public List<Permissionmanage> permissionByRole(int id)
    {
        return _context.Permissionmanages.Include(x=>x.Role).Where(x => x.RoleId == id).OrderBy(x => x.PermissionId).ToList();
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
