using Microsoft.AspNetCore.Authorization;

namespace Pizzashop_Project.Authorization;

public class PermissionAuthorizeAttribute : AuthorizeAttribute

{
    public PermissionAuthorizeAttribute(string permission) : base()
    {
        Policy = $"{permission}";
    }

}