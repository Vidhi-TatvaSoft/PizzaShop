using DAL.Models;

namespace DAL.ViewModels;

public class RolesPermissionViewModel
{
     public long PermissionmanageId { get; set; }

     public string Name { get; set; }

     public bool Canview { get; set; }

    public bool Caneditadd { get; set; }

    public bool Candelete { get; set; }

    public bool Permissioncheck { get; set; }


}
