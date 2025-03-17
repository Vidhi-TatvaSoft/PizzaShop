using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IRolesPermission
{
        List<Role> GetRoles();

        List<RolesPermissionViewModel> permissionByRole(string name);
        bool EditPermissionManage(RolesPermissionViewModel permissionmanage);
       
        
}