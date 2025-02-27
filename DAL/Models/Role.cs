using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Role
{
    public long RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual ICollection<Permissionmanage> Permissionmanages { get; } = new List<Permissionmanage>();

    public virtual ICollection<Userlogin> Userlogins { get; } = new List<Userlogin>();

    public virtual ICollection<User> Users { get; } = new List<User>();
}
