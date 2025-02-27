using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Permissionmanage
{
    public long PermissionmanageId { get; set; }

    public long PermissionId { get; set; }

    public long RoleId { get; set; }

    public bool Canview { get; set; }

    public bool Caneditadd { get; set; }

    public bool Candelete { get; set; }

    public bool Permissioncheck { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
