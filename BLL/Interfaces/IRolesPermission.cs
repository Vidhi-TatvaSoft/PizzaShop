using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service.Interfaces;

public interface IRolesPermission
{
        public List<Role> GetRoles();

        public List<RolesPermissionViewModel> permissionByRole(string name);

         public bool EditPermissionManage(Permissionmanage permissionmanage);

        
}
