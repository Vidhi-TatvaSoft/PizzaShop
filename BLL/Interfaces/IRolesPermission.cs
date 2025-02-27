using DAL.Models;

namespace BLL.Service.Interfaces;

public interface IRolesPermission
{
        public List<Role> GetRoles();

        public List<Permissionmanage> permissionByRole(int id);

         public bool EditPermissionManage(Permissionmanage permissionmanage);
}
